namespace EasySignWorkFlow;

public class MyState<TRequest, TKey, TStatus>
    where TKey : IEquatable<TKey>
{
    private Func<TRequest, Task<bool>> _predicate = (_) => Task.FromResult(true);

    private Func<TRequest, TStatus, TStatus, Task>? _onExecuteAsync;

    private Func<TRequest, TStatus, Task<IEnumerable<TKey>>>? _funcAsync;

    public TStatus Next { get; private set; }

    public MyState<TRequest, TKey, TStatus> If(Func<TRequest, bool> predicate)
    {
        this._predicate = request => Task.FromResult(predicate(request));
        return this;
    }
    
    public MyState<TRequest, TKey, TStatus> IfAsync(Func<TRequest, Task<bool>> predicate)
    {
        _predicate = predicate;
        return this;
    }

    public MyState<TRequest, TKey, TStatus> Set(TStatus next, Action<TRequest, TStatus, TStatus>? action = null)
    {
        Next = next;
        return this;
    }

    public MyState<TRequest, TKey, TStatus> OnExecute(Action<TRequest, TStatus, TStatus> action)
    {
        _onExecuteAsync = (request, status, nextStatus) =>
        {
            action(request, status, nextStatus);
            return Task.CompletedTask;
        };

        return this;
    }

    public MyState<TRequest, TKey, TStatus> SetResponsibleAsync(Func<TRequest, TStatus, Task<IEnumerable<TKey>>> func)
    {
        _funcAsync = func;

        return this;
    }

    public MyState<TRequest, TKey, TStatus> SetResponsible(Func<TRequest, TStatus, IEnumerable<TKey>> func)
    {
        _funcAsync = (request, status) =>
        {
            return Task.FromResult(func(request, status));
        };

        return this;
    }

    public MyState<TRequest, TKey, TStatus> OnExecuteAsync(Func<TRequest, TStatus, TStatus, Task> action)
    {
        _onExecuteAsync = action;
        return this;
    }

    internal async ValueTask<bool> Can(TRequest request)
    {
        return await _predicate(request);
    }

    internal async Task DoOnSetAsync(TRequest request, TStatus current) =>
        await _onExecuteAsync?.Invoke(request, current, Next)!;

    internal async Task ExecuteAsync(TRequest request, TStatus current) =>
        await _onExecuteAsync?.Invoke(request, current, Next)!;
}