public class FlowMachine2<TStatus>
{
    public Dictionary<TStatus, List<MyState<TStatus>>> Map { get; private set; }

    private Func<TStatus, TStatus, Task> _onTransaction;
    private TStatus _current;

    private FlowMachine2(TStatus current)
    {
        Map = new Dictionary<TStatus, List<MyState<TStatus>>>();
    }

    private FlowMachine2(TStatus current, IEqualityComparer<TStatus> equalityComparer)
    {
        Map = new Dictionary<TStatus, List<MyState<TStatus>>>(equalityComparer);
    }

    public static FlowMachine2<TStatus> Create(TStatus current)
    {
        return new(current);
    }

    public static FlowMachine2<TStatus> Create(TStatus current, IEqualityComparer<TStatus> equalityComparer)
    {
        return new(current, equalityComparer);
    }

    public FlowMachine2<TStatus> OnTransition(Action<TStatus, TStatus> action)
    {
        _onTransaction = (current, next) =>
        {
            action(current, next);
            return Task.CompletedTask;
        };
        return this;
    }

    public FlowMachine2<TStatus> OnTransitionAsync(Func<TStatus, TStatus, Task> action)
    {
        _onTransaction = action;
        return this;
    }

    public MyState<TStatus> When(TStatus currentStatus)
    {
        var stat = new MyState<TStatus>();
        if (Map.TryGetValue(currentStatus, out var a))
        {
            a.Add(stat);
        }

        return stat;
    }


    public async ValueTask<bool> FireAsync(TStatus next = default)
    {
        var onlyOn = true;
        var states = Map[_current];
        MyState<TStatus>? nextMyState = null; 
        foreach (var state in states)
        {
            if (await state.CanAsync())
            {
                if (next is not null && state.Next!.Equals(next))
                {
                    nextMyState = state;
                    break;
                }
                if (onlyOn && next is null)
                {
                    onlyOn = false;
                    nextMyState = state;
                }
                else if (nextMyState is not null && next is null)
                {
                    throw new Exception("must set to ant next will go");
                } 
            
            }
        }

        if (nextMyState is not null)
        {
            await _onTransaction(_current, nextMyState.Next);
            await nextMyState.ExecuteAsync(_current);
            _current = nextMyState.Next;
            return true;
        }

        return false;
    }


    public bool Fire(TStatus next = default)
    {
        return FireAsync(next).GetAwaiter().GetResult();
    }

   

  
}