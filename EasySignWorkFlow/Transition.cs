using EasySignWorkFlow.Models;

namespace EasySignWorkFlow;

public sealed class Transition<TRequest, TKey, TStatus>
    where TKey : IEquatable<TKey>
    where TStatus : Enum
    where TRequest : Request<TKey, TStatus>
{
    private Func<TRequest, Task<bool>> _predicate = (_) => Task.FromResult(true);

    private Func<TRequest, TStatus, TStatus, Task>? _onExecuteAsync;

    private Func<TRequest, TStatus, Task<IEnumerable<TKey>>>? _nextUsersGetter;

    private readonly FlowMachine<TRequest, TKey, TStatus> _flow;

    private readonly TStatus _current;

    public Transition(FlowMachine<TRequest, TKey, TStatus> flow, TStatus current)
    {
        _flow = flow;
        _current = current;
    }
    
    public TStatus? Next { get; private set; }

    public Transition<TRequest, TKey, TStatus> If(Func<TRequest, bool> predicate)
    {
        _predicate = request => Task.FromResult(predicate(request));
        return this;
    }
    
    public Transition<TRequest, TKey, TStatus> IfAsync(Func<TRequest, Task<bool>> predicate)
    {
        _predicate = predicate;
        return this;
    }

    public Transition<TRequest, TKey, TStatus> Set(TStatus next)
    {
        Next = next;

        _flow.Map[_current].Add(this);

        return this;
    }

    public Transition<TRequest, TKey, TStatus> OnExecute(Action<TRequest, TStatus, TStatus> action)
    {
        _onExecuteAsync = (request, status, nextStatus) =>
        {
            action(request, status, nextStatus);
            return Task.CompletedTask;
        };

        return this;
    }

    public Transition<TRequest, TKey, TStatus> OnExecuteAsync(Func<TRequest, TStatus, TStatus, Task> action)
    {
        _onExecuteAsync = action;
        return this;
    }

    public Transition<TRequest, TKey, TStatus> SetNextUsersAsync(Func<TRequest, TStatus, Task<IEnumerable<TKey>>> func)
    {
        _nextUsersGetter = func;

        return this;
    }

    public Transition<TRequest, TKey, TStatus> SetNextUsers(Func<TRequest, TStatus, IEnumerable<TKey>> func)
    {
        _nextUsersGetter = (request, status) =>
        {
            return Task.FromResult(func(request, status));
        };

        return this;
    }

    internal async ValueTask<bool> ValidAsync(TRequest request) 
        => await _predicate(request);

    internal async Task ExecuteAsync(TRequest request, TStatus current)
    {
        if(_onExecuteAsync is null)
            return;

        await _onExecuteAsync.Invoke(request, current, Next)!;
    }
        
    internal async Task<IEnumerable<TKey>> GetNextUserAsync(TRequest request, TStatus current)
    {
        if(_nextUsersGetter is null)
            return Enumerable.Empty<TKey>();

        return await _nextUsersGetter.Invoke(request, current)!;
    }
}