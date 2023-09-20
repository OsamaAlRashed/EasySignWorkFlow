using Demo.Enums;
using EasySignWorkFlow.Models;

namespace Demo.Models;

public class TestRequest : Request<Guid, TestStatus>
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public bool Flag { get; set; }
}
