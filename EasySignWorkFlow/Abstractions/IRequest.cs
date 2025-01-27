using EasySignWorkFlow.Models;
namespace EasySignWorkFlow.Abstractions;

public interface IRequest<TKey, TStatus>
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
{
    public List<State<TKey, TStatus>> Statuses { get; }
    public State<TKey, TStatus>? CurrentState { get; set; }
    internal void Add(State<TKey, TStatus> state) => Statuses.Add(state);
    internal void Clear() => Statuses.Clear();
    internal void Remove(State<TKey, TStatus> state) => Statuses.Remove(state);
    internal void UpdateCurrentState(State<TKey, TStatus> state) => CurrentState = new State<TKey, TStatus>(state.Status, state.DateSigned, state.SignedBy, state.Note);
}
