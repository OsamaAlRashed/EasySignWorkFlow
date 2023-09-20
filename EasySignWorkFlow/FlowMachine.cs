using EasySignWorkFlow.Models;

namespace EasySignWorkFlow;

public class FlowMachine<TRequest, TKey, TStatus> 
    where TRequest : Request<TKey, TStatus>
    where TStatus : Enum
    where TKey : IEquatable<TKey>
{
    public Dictionary<TStatus, List<MyState<TRequest, TKey, TStatus>>> Map { get; private set; }

    private Func<TRequest, TStatus, TStatus, Task>? _onTransaction;

    internal TStatus? RefuseStatus { get; }
    internal TStatus? CancelStatus { get; }
    internal TStatus InitStatus { get; }

    public FlowMachine(TStatus initStatus, TStatus? refuseStatus = default,TStatus? cancelStatus = default)
    {
        InitStatus= initStatus;
        CancelStatus = cancelStatus;
        RefuseStatus = refuseStatus;

        Map = new Dictionary<TStatus, List<MyState<TRequest, TKey, TStatus>>>
        {
            { initStatus, new List<MyState<TRequest, TKey, TStatus>>() }
        };
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

    public MyState<TRequest, TKey, TStatus> When(TStatus currentStatus)
    {
        var myState = new MyState<TRequest, TKey, TStatus>();

        if (!Map.ContainsKey(currentStatus))
        {
            Map[currentStatus] = new();
        }

        return myState;
    }

    public async ValueTask<bool> FireAsync(TRequest request, Func<TRequest, TStatus> current)
    {
        return await FireAsync(request, current(request));
    }

    public async ValueTask<bool> FireAsync(TRequest request, TStatus current)
    {
        foreach (var state in Map[current])
        {
            if (await state.Can(request))
            {
                await _onTransaction(request, current, state.Next);
                await state.DoOnSetAsync(request, current);

                return true;
            }

        }

        return false;
    }

    public bool Fire(TRequest request, Func<TRequest, TStatus> current)
    {
        return FireAsync(request, current).GetAwaiter().GetResult();
    }

    public bool Fire(TRequest request, TStatus current)
    {
        return FireAsync(request, current).GetAwaiter().GetResult();
    }
}