public class FlowMachine2<TRequest, TStatus> where TStatus : notnull
{
    public Dictionary<TStatus, List<MyState<TRequest, TStatus>>> Map { get; private set; }

    private Func<TRequest, TStatus, TStatus, Task> _onTransaction;

    private FlowMachine2()
    {
        Map = new Dictionary<TStatus, List<MyState<TRequest, TStatus>>>();
    }

    private FlowMachine2(IEqualityComparer<TStatus> equalityComparer)
    {
        Map = new Dictionary<TStatus, List<MyState<TRequest, TStatus>>>(equalityComparer);
    }

    public static FlowMachine2<TRequest, TStatus> Create()
    {
        return new();
    }    
    public static FlowMachine2<TRequest, TStatus> Create(IEqualityComparer<TStatus> equalityComparer)
    {
        return new(equalityComparer);
    }

    public FlowMachine2<TRequest, TStatus> SetTransaction(Action<TRequest, TStatus, TStatus> action)
    {
        _onTransaction = (request, current, next) =>
        {
            action(request, current, next);
            return Task.CompletedTask;
        };
        return this;
    }

    public FlowMachine2<TRequest, TStatus> SetTransactionAsync(Func<TRequest, TStatus, TStatus, Task> action)
    {
        _onTransaction = action;
        return this;
    }

    public MyState<TRequest, TStatus> When(TStatus currentStatus)
    {
        var stat = new MyState<TRequest, TStatus>(currentStatus);
        if (Map.TryGetValue(currentStatus, out var a))
        {
            a.Add(stat);
        }

        return stat;
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
            }

            return true;
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