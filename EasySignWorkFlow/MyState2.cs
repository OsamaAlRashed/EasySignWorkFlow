public class MyState<TStatus>
{
    private Func<Task<bool>> _predicate = () => Task.FromResult(true);

    public TStatus Next { get; private set; }

    private Func<TStatus, TStatus, Task>? _onExecuteAsync;

    public MyState< TStatus> If(Func<bool> predicate)
    {
        this._predicate =  () => Task.FromResult(predicate());
        return this;
    }
    public MyState< TStatus> IfAsync(Func< Task<bool>> predicate)
    {
        this._predicate = predicate;
        return this;
    }

    public MyState< TStatus> Set(TStatus next)
    {
        Next = next;
        return this;
    }

    public MyState< TStatus> OnExecute(Action<TStatus, TStatus> action)
    {
        _onExecuteAsync = (status, arg3) =>
        {
            action(status, arg3);
            return Task.CompletedTask;
        };
        return this;
    }

    public MyState< TStatus> OnExecuteAsync(Func< TStatus, TStatus, Task> action)
    {
        _onExecuteAsync = action;
        return this;
    }

    internal async ValueTask<bool> CanAsync()
    {
        return await _predicate();
    }

    internal async Task ExecuteAsync(TStatus current) =>
        await _onExecuteAsync?.Invoke(current, Next)!;
}