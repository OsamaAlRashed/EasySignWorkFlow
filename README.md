# EasySignWorkFlow

EasySignWorkFlow is a C# library designed to provide a flexible and easy-to-use workflow management system. It allows you to define and manage state transitions, perform actions during transitions, and set custom rules for workflows. With features like state validation, transition conditions, and customizable behaviors, EasySignWorkFlow is perfect for implementing approval processes, request management, or any workflow-based application.

## Features

- **State Management**: Define states and manage transitions seamlessly.
- **Flexible Conditions**: Add conditional logic to transitions with `If` and `IfAsync` methods.
- **Custom Actions**: Execute custom logic during transitions.
- **Extensibility**: Easily extend and customize workflows with user-defined states and transitions.
- **Date Providers**: Use `DateTimeProvider` to control how dates are handled (UTC or local).
- **Database Integration**: Works well with EF Core to persist requests and their states.

---

## Installation

To install the library, add the following package to your project:

```bash
Install-Package EasySignWorkFlow
```

Or use the .NET CLI:

```bash
dotnet add package EasySignWorkFlow
```

---

## Getting Started

### Define Your Request

Create a class that implements the `IRequest<TKey, TStatus>` interface:

```csharp
public class TestRequest : IRequest<Guid, TestStatus>
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public bool Flag { get; set; }

    public List<State<Guid, TestStatus>> States { get; } = new();
    public State<Guid, TestStatus>? CurrentState { get; set; }
}
```

### Configure Your Workflow

Use the `FlowMachine` class to define states and transitions:

```csharp
public class TestRequestService
{
    private readonly FlowMachine<TestRequest, Guid, TestStatus> _flowMachine;
    private readonly DemoDBContext _context;

    public TestRequestService(DemoDBContext context)
    {
        _context = context;

        _flowMachine = FlowMachine<TestRequest, Guid, TestStatus>
            .Create(TestStatus.Draft, (request, current, next) =>
            {
                Console.WriteLine($"Transitioning from {current} to {next}");
                return Task.CompletedTask;
            });

        _flowMachine.SetCancelState(TestStatus.Canceled)
                    .SetRefuseState(TestStatus.Refused)
                    .SetDateTimeProvider(DateTimeProvider.Now);

        // Define transitions
        _flowMachine.When(TestStatus.Draft)
            .If(request => request.Flag)
            .Set(TestStatus.WaitingForManager1)
            .SetNextUsers((request, _) => new List<Guid> { Guid.NewGuid() })
            .OnExecute((request, current, next, nextUserIds) =>
            {
                Console.WriteLine($"{request.Title} moved from {current} to {next}");
            });

        _flowMachine.When(TestStatus.WaitingForManager1)
            .If(request => !request.Flag)
            .Set(TestStatus.Accepted)
            .OnExecute((request, current, next, nextUserIds) =>
            {
                Console.WriteLine($"{request.Title} moved from {current} to {next}");
            });
    }

    public string PrintWorkflow() => _flowMachine.ToString();
}
```

### Workflow in Action

Execute the workflow by calling the `FireAsync` method:

```csharp
var request = new TestRequest
{
    Id = Guid.NewGuid(),
    Title = "Sample Request",
    Flag = true
};

var isSuccess = await _flowMachine.FireAsync(request, TestStatus.Draft);
if (isSuccess)
{
    Console.WriteLine("Transition successful.");
}
else
{
    Console.WriteLine("Transition failed.");
}
```

---

## Example Use Case

### Database Integration

You can integrate EasySignWorkFlow with EF Core to persist requests and their states:

```csharp
public class DemoDBContext : DbContext
{
    public DemoDBContext(DbContextOptions<DemoDBContext> options) : base(options) { }

    public DbSet<TestRequest> TestRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestRequest>().OwnsOne("CurrentState", x => x.CurrentState);
        modelBuilder.Entity<TestRequest>().OwnsMany("States", x => x.States);
        base.OnModelCreating(modelBuilder);
    }
}
```

### Querying States

Fetch requests based on their current state:

```csharp
public async Task<List<TestRequest>> GetDraftRequests()
{
    return await _context.TestRequests
        .Where(r => r.CurrentState.Status == TestStatus.Draft)
        .ToListAsync();
}
```

---

## Key Classes

### `IRequest<TKey, TStatus>`

Defines a request with a collection of states and a current state.

### `FlowMachine<TRequest, TKey, TStatus>`

The core class for defining and managing workflows.

### `Transition<TRequest, TKey, TStatus>`

Handles individual state transitions, including conditions and custom actions.

### `State<TKey, TStatus>`

Represents a state in the workflow, including metadata like date and user.

---

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

---

## License

This project is licensed under the MIT License. See the LICENSE file for details.
