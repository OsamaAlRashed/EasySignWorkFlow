using EasySignWorkFlow.Models;
namespace EasySignWorkFlow.EFCore;

public abstract class EFRequest<TKey, TStatus> : Request<TKey, TStatus>
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
{
    public new State<TKey, TStatus>? CurrentState { get; private set; }
}
