using Demo.Enums;
using Demo.Models;
using Demo.Models.DBContext;
using EasySignWorkFlow;
using EasySignWorkFlow.Extensions;
using EasySignWorkFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace Demo.Services
{
    public class CashRequestService
    {
        private readonly FlowMachine<CashRequest, Guid, CashRequestStatus> _flowMachine;

        private readonly FlowMachine2<CashRequest, CashRequestStatus,State<Guid, CashRequestStatus>> _flowMachine2;

        private readonly DemoDBContext _context;

        public CashRequestService(DemoDBContext context)
        {
            _context = context;
            _flowMachine = new FlowMachine<CashRequest, Guid, CashRequestStatus>();
            _flowMachine = _flowMachine
                .StartWith(CashRequestStatus.Draft)
                .Then(CashRequestStatus.WaitingForFinanceAccountantApproval)
                .SetAcceptState(CashRequestStatus.Accepted);

                _flowMachine2.Config(CashRequestStatus.Draft,new (a => true))

               
        }

        private CashRequestStatus Draft(CashRequest cashRequest)
        {
            CashRequestStatus result;
            if (cashRequest.RequestType == CashRequestType.Exchange)
            {
                if (cashRequest.DirId == 10)
                {
                    return CashRequestStatus.WaitingForFinanceManagerApproval;
                }

                return CashRequestStatus.WaitingForFinanceAccountantApproval;
            }

            if (cashRequest.RequestType == CashRequestType.Transfer)
            {
                return CashRequestStatus.WaitingForFinanceAccountantApproval;
            }
            
            return CashRequestStatus.WaitingForFinanceAccountantApproval;
        }

        public async Task<bool> Approve(Guid id, string note)
        {
            var request = await _context.CashRequests.Where(x => x.Id == id).FirstOrDefaultAsync();

            request?.Approve(_flowMachine, x =>
            {
            });

            await _context.SaveChangesAsync();

            return true;
        } 

    }
}
