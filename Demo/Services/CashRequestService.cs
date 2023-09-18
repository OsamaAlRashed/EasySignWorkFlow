using Demo.Enums;
using Demo.Models;
using Demo.Models.DBContext;
using EasySignWorkFlow;
using EasySignWorkFlow.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


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


        public async Task<bool> Approve(Guid id, string note)
        {
            var request = await _context.CashRequests.Where(x => x.Id == id).FirstAsync();
            var cashRequestFlowMachine = _flowMachineFactory.BuildCashRequest(request, Guid.NewGuid(), note);
            await cashRequestFlowMachine.FireAsync();
            await _context.SaveChangesAsync();

            return true;
        } 
        public async Task<bool> Cansel(Guid id, string note)
        {
            var request = await _context.CashRequests.Where(x => x.Id == id).FirstAsync();
            var cashRequestFlowMachine = _flowMachineFactory.BuildCashRequest(request, Guid.NewGuid(), note);
            await cashRequestFlowMachine.FireAsync(CashRequestStatusC.Canceled);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}