using EasySignWorkFlow.Models;

namespace EasySignWorkFlow;

public sealed class Transition<TRequest, TKey, TStatus>
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
    where TRequest : Request<TKey, TStatus>
{
    private readonly FlowMachine<TRequest, TKey, TStatus> _flow;
    
    private Func<TRequest, Task<bool>> _predicate = (_) => Task.FromResult(true);

    private Func<TRequest, TStatus, TStatus, IEnumerable<TKey>, Task>? _onExecuteAsync;

    private Func<TRequest, TStatus, Task<IEnumerable<TKey>>>? _nextUsersGetter;
    
    private readonly TStatus _current;

    internal TStatus? Next { get; private set; }

    public Transition(FlowMachine<TRequest, TKey, TStatus> flow, TStatus current)
    {
        _flow = flow;
        _current = current;
    }

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

    public void OnExecute(Action<TRequest, TStatus, TStatus, IEnumerable<TKey>> action) 
        => _onExecuteAsync = 
        (request, status, nextStatus, nextUserIds) =>
        {
            action(request, status, nextStatus, nextUserIds);
            return Task.CompletedTask;
        };

    public void OnExecuteAsync(Func<TRequest, TStatus, TStatus, IEnumerable<TKey>, Task> action) => _onExecuteAsync = action;

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
        if(_onExecuteAsync is null || !Next.HasValue)
            return;

        await _onExecuteAsync.Invoke(request, current, Next.Value, _nextUsersGetter is null ? new List<TKey>() : await _nextUsersGetter(request, current))!;
    }
        
    internal async Task<IEnumerable<TKey>> GetNextUserAsync(TRequest request, TStatus current)
    {
        if(_nextUsersGetter is null)
            return Enumerable.Empty<TKey>();

        return await _nextUsersGetter.Invoke(request, current)!;
    }
}