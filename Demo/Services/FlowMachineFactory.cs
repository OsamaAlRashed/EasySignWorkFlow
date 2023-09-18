using Demo.Enums;
using Demo.Models;
using Demo.Models.DBContext;
using EasySignWorkFlow.Models;
using Microsoft.EntityFrameworkCore;

public class FlowMachineFactory
{
    private readonly DemoDBContext _context;

    public FlowMachineFactory(DemoDBContext context)
    {
        _context = context;
    }


    public FlowMachine2<State<Guid, CashRequestStatus>> BuildCashRequest(CashRequest request,
        Guid signedBy,
        string note)
    {
        var flowMachine2 = FlowMachine2<State<Guid, CashRequestStatus>>
            .Create(request.CurrentStatus2, CashRequestStatusCEqualityComparer.Instance);

        flowMachine2.OnTransition((current, next) => { request.AddState(next.New(signedBy, note)); });

        flowMachine2.When(CashRequestStatusC.Draft)
            .If(() => request.RequestType == CashRequestType.Exchange)
            .Set(CashRequestStatusC.Draft)
            .OnExecuteAsync(async (current, next) =>
            {
                Console.WriteLine($"{request.Id} : {current.Status} to {next.Status}");
                await _context.CashRequests.FirstOrDefaultAsync();
            });

        flowMachine2.When(CashRequestStatusC.Draft)
            .If(() => request.RequestType == CashRequestType.Payment)
            .Set(CashRequestStatusC.WaitingForFinanceAccountantApproval)
            .OnExecute((current, next) => Console.WriteLine($"{request.Id} : {current.Status} to {next.Status}"));    
        
        
        flowMachine2.When(CashRequestStatusC.Draft)
            .If(() => request.RequestType == CashRequestType.Payment)
            .Set(CashRequestStatusC.WaitingForFinanceManagerApproval)
            .OnExecute((current, next) => Console.WriteLine($"{request.Id} : {current.Status} to {next.Status}"));

        return flowMachine2;
    }
}