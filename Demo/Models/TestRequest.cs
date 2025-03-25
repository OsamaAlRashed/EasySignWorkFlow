using Demo.Enums;
using EasySignWorkFlow.Abstractions;
using EasySignWorkFlow.Models;

namespace Demo.Models;

public class TestRequest : IRequest<Guid, TestStatus>
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public bool Flag { get; set; }

    public List<State<Guid, TestStatus>> States { get; } = [];
    public State<Guid, TestStatus>? CurrentState { get; set; }
}
