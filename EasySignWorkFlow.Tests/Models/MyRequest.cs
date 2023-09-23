using EasySignWorkFlow.Models;

namespace EasySignWorkFlow.Tests.Models;

public class MyRequest : Request<Guid, MyRequestStatus>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Value { get; set; }
}

public enum MyRequestStatus
{
    Draft,
    WaitingForManager1,
    WaitingForManager2,
    Accepted,
    Refused,
    Canceled,
}
