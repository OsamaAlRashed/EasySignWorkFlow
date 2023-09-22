using EasySignWorkFlow.Models;

namespace EasySignWorkFlow;

public class FlowMachine<TRequest, TKey, TStatus> 
    where TRequest : Request<TKey, TStatus>
    where TStatus : Enum
    where TKey : IEquatable<TKey>
{
    public Dictionary<TStatus, List<Transaction<TRequest, TKey, TStatus>>> Map { get; private set; }

    private Func<TRequest, TStatus, TStatus, Task>? _onTransaction;

    internal TStatus? RefuseStatus { get; private set; }
    internal TStatus? CancelStatus { get; private set; }
    internal TStatus InitStatus { get; private set; }

    public FlowMachine(TStatus initStatus)
    {
        InitStatus = initStatus;

        Map = new Dictionary<TStatus, List<Transaction<TRequest, TKey, TStatus>>>
        {
            { initStatus, new List<Transaction<TRequest, TKey, TStatus>>() }
        };
    }

    public FlowMachine<TRequest, TKey, TStatus> SetCancelState(TStatus status)
    {
        CancelStatus = status;

        return this;
    }

    public FlowMachine<TRequest, TKey, TStatus> SetRefuseState(TStatus status)
    {
        RefuseStatus = status;

        return this;
    }

    public FlowMachine<TRequest, TKey, TStatus> SetTransaction(Action<TRequest, TStatus, TStatus> action)
    {
        _onTransaction = (request, current, next) =>
        {
            action(request, current, next);
            return Task.CompletedTask;
        };

        return this;
    }

    public FlowMachine<TRequest, TKey, TStatus> SetTransactionAsync(Func<TRequest, TStatus, TStatus, Task> action)
    {
        _onTransaction = action;
        return this;
    }

    public Transaction<TRequest, TKey, TStatus> When(TStatus currentStatus)
    {
        var transaction = new Transaction<TRequest, TKey, TStatus>(this, currentStatus);

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
            if (await transaction.ValidAsync(request))
            {
                await _onTransaction!(request, current, transaction.Next);
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