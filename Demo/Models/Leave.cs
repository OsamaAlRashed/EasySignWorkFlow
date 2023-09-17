using Demo.Enums;
using EasySignWorkFlow.Models;

namespace Demo.Models;

public class Leave : Request<Guid, LeaveStatus>
{
    public Guid Id { get; set; }
}
