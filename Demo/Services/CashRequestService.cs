using Demo.Enums;
using Demo.Models;
using Demo.Models.DBContext;
using EasySignWorkFlow;
using EasySignWorkFlow.Extensions;
using EasySignWorkFlow.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


public class FlowMachineFactory
{
    private readonly DemoDBContext _demoDbContext;

    private FlowMachine2<CashRequest, State<Guid, CashRequestStatus>> _flowMachine2;

    public FlowMachineFactory(DemoDBContext demoDbContext)
    {
        _demoDbContext = demoDbContext;
        Build();
    }

    public async Task Approve(CashRequest request, string note)
    {
        BuildTransaction(note);
        await _flowMachine2.FireAsync(request, request.CurrentStatus2);
    }

    private void Build()
    {
        _flowMachine2 = FlowMachine2<CashRequest, State<Guid, CashRequestStatus>>.Create(CashRequestStatusCEqualityComparer.Instance);

        _flowMachine2.When(CashRequestStatusC.Draft)
            .If(request => request.RequestType == CashRequestType.Exchange)
            .Set(CashRequestStatusC.Draft)
            .OnSetAsync(async (request, current, next) =>
                request.DirId = (await _demoDbContext.CashRequests.FirstAsync()).SectorId);

        _flowMachine2.When(CashRequestStatusC.Draft)
            .IfAsync(async request => request.RequestType == (await _demoDbContext.CashRequests.FirstAsync()).RequestType)
            .Set(CashRequestStatusC.WaitingForFinanceAccountantApproval)
            .OnSetAsync(async (request, current, next) =>
                request.DirId = (await _demoDbContext.CashRequests.FirstAsync()).SectorId);
    }

    private void BuildTransaction(string note)
    {
        _flowMachine2.SetTransaction((request, current, next) =>
        {
            next.SetSignedBy(Guid.NewGuid());
            next.SetNote(note);
            request.AddState(next);
        });
    }
}

namespace Demo.Services
{
    public class CashRequestService
    {
        private readonly FlowMachine<CashRequest, Guid, CashRequestStatus> _flowMachine;

        private readonly FlowMachineFactory _flowMachineFactory;

        private readonly DemoDBContext _context;

        public CashRequestService(DemoDBContext context, FlowMachineFactory flowMachineFactory)
        {
            _context = context;
            _flowMachineFactory = flowMachineFactory;
            _flowMachine = new FlowMachine<CashRequest, Guid, CashRequestStatus>();
            _flowMachine = _flowMachine
                .StartWith(CashRequestStatus.Draft)
                .Then(CashRequestStatus.WaitingForFinanceAccountantApproval)
                .SetAcceptState(CashRequestStatus.Accepted);
        }


        public async Task<bool> Handel(Guid id, string note)
        {
            var request = await _context.CashRequests.Where(x => x.Id == id).FirstAsync();
            await _flowMachineFactory.Approve(request, note);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}