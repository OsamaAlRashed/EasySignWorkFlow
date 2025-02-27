using EasySignWorkFlow.Abstractions;
using EasySignWorkFlow.Enums;
using EasySignWorkFlow.Models;
using System.Text;

namespace EasySignWorkFlow;

public sealed class FlowMachine<TRequest, TKey, TStatus> 
    where TRequest : IRequest<TKey, TStatus>
    where TStatus : struct, Enum
    where TKey : IEquatable<TKey>
{
    public Dictionary<TStatus, List<Transition<TRequest, TKey, TStatus>>> Map { get; private set; }

    private Func<TRequest, TStatus, TStatus, Task>? _onTransaction;
    internal DateTimeProvider DateProvider { get; private set; }
    internal TStatus? RefuseStatus { get; private set; }
    internal TStatus? CancelStatus { get; private set; }
    internal TStatus InitStatus { get; private set; }
    internal Func<TRequest, TStatus, TStatus, Task>? OnInitAsync { get; private set; }
    internal Func<TRequest, TStatus, TStatus, Task>? OnRefuseAsync { get; private set; }
    internal Func<TRequest, TStatus, TStatus, Task>? OnCancelAsync { get; private set; }

    private FlowMachine(TStatus initStatus)
    {
        Map = new Dictionary<TStatus, List<Transition<TRequest, TKey, TStatus>>>
        {
            { initStatus, new List<Transition<TRequest, TKey, TStatus>>() }
        };

        _onTransaction = (request, current, next) =>
        {
            var newState = new State<TKey, TStatus>(next, GetCurrentDateTime(), default, string.Empty);
            request.Add(newState);
            request.UpdateCurrentState(newState);
            return Task.CompletedTask;
        };
    }

    public static FlowMachine<TRequest, TKey, TStatus> Create(TStatus initStatus, Func<TRequest, TStatus, TStatus, Task>? onInitAsync = null)
    {
        FlowMachine<TRequest, TKey, TStatus> flowMachine = new(initStatus)
        {
            OnInitAsync = onInitAsync
        };

        return flowMachine;
    }

    public FlowMachine<TRequest, TKey, TStatus> SetCancelState(TStatus status, Func<TRequest, TStatus, TStatus, Task>? onCancelAsync = null)
    {
        CancelStatus = status;

        OnCancelAsync = onCancelAsync;

        return this;
    }

    public FlowMachine<TRequest, TKey, TStatus> SetDateTimeProvider(DateTimeProvider dateProvider)
    {
        DateProvider = dateProvider;

        return this;
    }

    public FlowMachine<TRequest, TKey, TStatus> SetRefuseState(TStatus status, Func<TRequest, TStatus, TStatus, Task>? onRefuseAsync = null)
    {
        RefuseStatus = status;

        OnRefuseAsync = onRefuseAsync;

        return this;
    }

    public FlowMachine<TRequest, TKey, TStatus> SetTransition(Action<TRequest, TStatus, TStatus> action)
    {
        _onTransaction = (request, current, next) =>
        {
            action(request, current, next);
            return Task.CompletedTask;
        };

        return this;
    }

    public FlowMachine<TRequest, TKey, TStatus> SetTransitionAsync(Func<TRequest, TStatus, TStatus, Task> action)
    {
        _onTransaction = action;
        return this;
    }

    public Transition<TRequest, TKey, TStatus> When(TStatus currentStatus)
    {
        var transaction = new Transition<TRequest, TKey, TStatus>(this, currentStatus);

        if (!Map.ContainsKey(currentStatus))
        {
            Map[currentStatus] = new();
        }

        return transaction;
    }

    public async ValueTask<bool> FireAsync(TRequest request, TStatus current)
    {
        foreach (var transaction in Map[current])
        {
            if (!transaction.Next.HasValue)
            {
                continue;
            }

            if (await transaction.ValidAsync(request))
            {
                await _onTransaction!(request, current, transaction.Next.Value);
                await transaction.ExecuteAsync(request, current);

                return true;
            }
        }

        return false;
    }

    public async ValueTask<bool> FireAsync(TRequest request, Func<TRequest, TStatus> current)
        => await FireAsync(request, current(request));

    public bool Fire(TRequest request, Func<TRequest, TStatus> current) 
        => FireAsync(request, current).AsTask().GetAwaiter().GetResult();

    public bool Fire(TRequest request, TStatus current) 
        => FireAsync(request, current).AsTask().GetAwaiter().GetResult();

    internal DateTime GetCurrentDateTime()
    {
        if (DateProvider == DateTimeProvider.UtcNow)
            return DateTime.UtcNow;

        return DateTime.Now;
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new();

        stringBuilder.AppendLine($"Initial Status: {InitStatus.ToString()}");
        foreach (var item in Map)
        {
            stringBuilder.AppendLine($"Transitions of {item.Key.ToString()}:");
            foreach (var transition in item.Value)
            {
                stringBuilder.AppendLine($"{item.Key.ToString()} -----> {transition.Next!.ToString()}");
            }
        }

        var finalStatuses = Map.Values
            .SelectMany(x => x)
            .Select(x => (TStatus)x.Next!)
            .Distinct()
            .Except(Map.Keys);            ;

        stringBuilder.AppendLine($"Final statuses:");
        foreach (var finalStatus in finalStatuses)
        {
            stringBuilder.AppendLine($"{finalStatus.ToString()}");
        }

        if(CancelStatus is not null)
        {
            stringBuilder.AppendLine($"Cancel Status: {CancelStatus.ToString()}");
        }

        if(RefuseStatus is not null)
        {
            stringBuilder.AppendLine($"Refuse Status: {RefuseStatus.ToString()}");
        }

        return stringBuilder.ToString();
    }
}