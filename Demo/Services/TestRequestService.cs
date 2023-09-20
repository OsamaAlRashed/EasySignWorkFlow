using Demo.Enums;
using Demo.Models;
using Demo.Models.DBContext;
using EasySignWorkFlow;
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

            _flowMachine = new FlowMachine<TestRequest, Guid, TestStatus>(
                TestStatus.Draft,
                TestStatus.Refused,
                TestStatus.Canceled);

            _flowMachine.When(TestStatus.Draft)
                .Set(TestStatus.WaitingForManager)
                .OnExecute((request, status, next) =>
                {
                    Console.WriteLine($"move {request.Title} from {status} to {status}.");
                });

            _flowMachine.When(TestStatus.WaitingForManager)
                .Set(TestStatus.Accepted);
        }

        public async Task<List<TestRequest>> Get()
        {
            return await _context.TestRequests.ToListAsync();
        }

        public async Task Add()
        {
            var request = new TestRequest()
            {
                Title = "Test",
                Flag = true,
            };

            request.OnCreate(_flowMachine, Guid.NewGuid());
            
            _context.TestRequests.Add(request);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> Approve(Guid id, Guid signedBy, string note)
        {
            var request = await _context.TestRequests.FindAsync(id);

            var result = request!.Approve(_flowMachine, signedBy, note, (request) =>
            {
                request.Title = request.Title + " " + request.CurrentStatus;
            });

            await _context.SaveChangesAsync();

            return result;
        }
    }
}
