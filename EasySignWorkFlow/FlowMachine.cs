using EasySignWorkFlow.Enums;
using EasySignWorkFlow.Models;

namespace EasySignWorkFlow;

public sealed class FlowMachine<TRequest, TKey, TStatus> 
    where TRequest : Request<TKey, TStatus>
    where TStatus : struct, Enum
    where TKey : IEquatable<TKey>
{
    public Dictionary<TStatus, List<Transition<TRequest, TKey, TStatus>>> Map { get; private set; }

    private Func<TRequest, TStatus, TStatus, Task>? _onTransaction;
    internal DateTimeProvider DateProvider { get; private set; }
    internal TStatus? RefuseStatus { get; private set; }
    internal TStatus? CancelStatus { get; private set; }
    internal TStatus InitStatus { get; private set; }

    public FlowMachine(TStatus initStatus)
    {
        InitStatus = initStatus;

        Map = new Dictionary<TStatus, List<Transition<TRequest, TKey, TStatus>>>
        {
            { initStatus, new List<Transition<TRequest, TKey, TStatus>>() }
        };
    }

    public FlowMachine<TRequest, TKey, TStatus> SetCancelState(TStatus status)
    {
        CancelStatus = status;

        return this;
    }

    public FlowMachine<TRequest, TKey, TStatus> SetDateTimeProvider(DateTimeProvider dateProvider)
    {
        DateProvider = dateProvider;

        return this;
    }

    internal DateTime GetCurrentDateTime()
    {
        if (DateProvider == DateTimeProvider.UtcNow)
            return DateTime.UtcNow;

        return DateTime.Now;
    }

    public FlowMachine<TRequest, TKey, TStatus> SetRefuseState(TStatus status)
    {
        RefuseStatus = status;

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
}