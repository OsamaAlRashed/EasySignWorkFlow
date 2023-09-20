using Demo.Enums;
using Demo.Models;
using Demo.Models.DBContext;
using EasySignWorkFlow;
using EasySignWorkFlow.Extensions;
using Microsoft.EntityFrameworkCore;
namespace Demo.Services;

public class CashRequestService
{
    private readonly FlowMachineFactory _flowMachineFactory;

    private readonly DemoDBContext _context;

    public CashRequestService(DemoDBContext context, FlowMachineFactory flowMachineFactory)
    {
        _context = context;
        _flowMachineFactory = flowMachineFactory;
    }
}