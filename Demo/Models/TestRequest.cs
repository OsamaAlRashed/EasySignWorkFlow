using Demo.Enums;
using EasySignWorkFlow.EFCore;

namespace Demo.Models;

public class TestRequest : EFRequest<Guid, TestStatus>
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public bool Flag { get; set; }
}
