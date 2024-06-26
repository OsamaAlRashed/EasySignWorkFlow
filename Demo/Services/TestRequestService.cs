﻿using Demo.Enums;
using Demo.Models;
using Demo.Models.DBContext;
using EasySignWorkFlow;
using EasySignWorkFlow.Enums;
using EasySignWorkFlow.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Demo.Services
{
    public class TestRequestService
    {
        private readonly FlowMachine<TestRequest, Guid, TestStatus> _flowMachine;

        private readonly DemoDBContext _context;

        public TestRequestService(DemoDBContext context)
        {
            _context = context;

            _flowMachine = FlowMachine<TestRequest, Guid, TestStatus>
                .Create(TestStatus.Draft, (request, current, next) =>
                {
                    var x = _context.TestRequests.ToList();

                    return Task.CompletedTask;
                });

            _flowMachine.SetCancelState(TestStatus.Canceled).SetRefuseState(TestStatus.Refused);

            _flowMachine.SetDateTimeProvider(DateTimeProvider.Now);

            _flowMachine.When(TestStatus.Draft)
                .If(request => request.Flag)
                .Set(TestStatus.WaitingForManager1)
                .SetNextUsers((request, _) => Enumerable.Range(0, 1).Select(x => Guid.NewGuid()).ToList())
                .OnExecute((request, current, next, nextUserIds) =>
                {
                    Console.WriteLine($"{request.Title} moved from {current} to {next}");
                });

            _flowMachine.When(TestStatus.Draft)
                .If(request => !request.Flag)
                .Set(TestStatus.WaitingForManager2)
                .SetNextUsers((request, _) => Enumerable.Range(0, 2).Select(x => Guid.NewGuid()).ToList())
                .OnExecute((request, current, next, nextUserIds) =>
                {
                    Console.WriteLine($"{request.Title} moved from {current} to {next}");
                });

            _flowMachine.When(TestStatus.WaitingForManager1)
                .If(request => !request.Flag)
                .Set(TestStatus.Accepted)
                .OnExecute((request, current, next, nextUserIds) =>
                {
                    Console.WriteLine($"{request.Title} moved from {current} to {next}");
                });

            _flowMachine.When(TestStatus.WaitingForManager2)
                .If(request => !request.Flag)
                .Set(TestStatus.Accepted)
                .OnExecute((request, current, next, nextUserIds) =>
                {
                    Console.WriteLine($"{request.Title} moved from {current} to {next}");
                });
        }

        public async Task<List<TestRequest>> Get() => await _context.TestRequests.ToListAsync();

        public async Task<List<TestStatus>> GetByStatus() 
            => await _context.TestRequests
            .Where(x => x.CurrentState.Status == TestStatus.Draft)
            .Select(x => x.CurrentState.Status)
            .ToListAsync();

        public string Print() => _flowMachine.ToString();

        public async Task<string> Print(Guid id)
        {
            var request = await _context.TestRequests.FirstOrDefaultAsync(x => x.Id == id);
            return request!.ToString();
        }

        public async Task<TestRequest?> Get(Guid id) => await _context.TestRequests.FirstOrDefaultAsync(x => x.Id == id);
        
        public async Task Clear()
        {
            var requests = await Get();
            _context.TestRequests.RemoveRange(requests);

            await _context.SaveChangesAsync();
        }

        public async Task<Guid> Add()
        {
            var request = new TestRequest()
            {
                Title = "Test",
                Flag = true,
            };

            request.OnCreate(_flowMachine, Guid.NewGuid());
            
            _context.TestRequests.Add(request);

            await _context.SaveChangesAsync();

            return request.Id;
        }

        public async Task<bool> Approve(Guid id, Guid signedBy, string note)
        {
            var request = await _context.TestRequests.FirstOrDefaultAsync(x => x.Id == id);

            if(request == null)
                return false;

            var result = request.Approve(_flowMachine, signedBy, note, (request) =>
            {
                request.Flag = !request.Flag;
            });

            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<bool> Refuse(Guid id, Guid signedBy, string note)
        {
            var request = await _context.TestRequests.FirstOrDefaultAsync(x => x.Id == id);
            
            if (request == null)
                return false;

            var result = request.Refuse(_flowMachine, signedBy, note, (request) =>
            {
                request.Title += " (Refused)";
            });

            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<bool> Reset(Guid id, Guid signedBy, string note)
        {
            var request = await _context.TestRequests.FirstOrDefaultAsync(x => x.Id == id);

            if (request == null)
                return false;

            var result = request.Reset(_flowMachine, signedBy, note, (request) =>
            {
                request.Title = request.Title + " - " + request.CurrentState.Status;
            });

            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<bool> Cancel(Guid id, Guid signedBy, string note)
        {
            var request = await _context.TestRequests.FirstOrDefaultAsync(x => x.Id == id);

            if (request == null)
                return false;

            var result = request.Cancel(_flowMachine, signedBy, note, (request) =>
            {
                request.Title = request.Title + " " + request.CurrentState.Status;
            });

            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<bool> Undo(Guid id, Guid signedBy)
        {
            var request = await _context.TestRequests.FirstOrDefaultAsync(x => x.Id == id);

            if (request == null)
                return false;

            var result = request.Undo<TestRequest, Guid, TestStatus>(signedBy, (request) =>
            {
                request.Title = request.Title + " " + request.CurrentState!.Status;
            });

            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<IEnumerable<Guid>> GetNextUsers(Guid id)
        {
            var request = await _context.TestRequests.FirstOrDefaultAsync(x => x.Id == id);

            if (request == null)
                return Enumerable.Empty<Guid>();

            return await request.GetNextUsersAsync(_flowMachine);
        }
    }
}
