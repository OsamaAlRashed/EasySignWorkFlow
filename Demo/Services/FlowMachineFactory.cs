using Demo.Enums;
using Demo.Models;
using Demo.Models.DBContext;
using EasySignWorkFlow;

namespace Demo.Services;

public class FlowMachineFactory
{
    private readonly DemoDBContext _context;

    public FlowMachineFactory(DemoDBContext context)
    {
        _context = context;
    }

    public FlowMachine<CashRequest, Guid, CashRequestStatus> BuildCashRequest()
    {
        var flowMachine = new FlowMachine<CashRequest, Guid, CashRequestStatus>(CashRequestStatus.Draft);

        flowMachine.When(CashRequestStatus.Draft)
            .Set(CashRequestStatus.WaitingForFinanceAccountantApproval)
            .SetResponsible((_, _) => Enumerable.Range(0, 10).Select(x => Guid.NewGuid()))
            .OnExecute((request, current, next) =>
            {
                Console.WriteLine($"{request.Id} : {current} to {next}");
            });

        flowMachine.When(CashRequestStatus.WaitingForFinanceAccountantApproval)
            .Set(CashRequestStatus.WaitingForCashierOfficerApproval);

        flowMachine.When(CashRequestStatus.WaitingForCashierOfficerApproval)
            .Set(CashRequestStatus.WaitingForCashierOfficerApproval)
            .SetResponsible((_, _) => Enumerable.Range(0, 10).Select(x => Guid.NewGuid()))
            .OnExecute((request, current, next) =>
            {
                Console.WriteLine($"{request.Id} : {current} to {next}");
            });

        return flowMachine;
    }
}