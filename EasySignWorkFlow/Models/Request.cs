    using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EasySignWorkFlow.Models;

public abstract class Request<TKey, TStatus>
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
{
    private readonly List<State<TKey, TStatus>> _statuses = new();

    public IReadOnlyList<State<TKey, TStatus>> Statuses 
        => _statuses.AsReadOnly();

    [NotMapped]
    public virtual State<TKey, TStatus>? CurrentState
    {
        get
        {
            return _statuses
                .OrderByDescending(x => x.DateSigned)
                .FirstOrDefault();
        }
        private set { }
    }

    internal void Add(State<TKey, TStatus> state) => _statuses.Add(state);

    internal void Clear() => _statuses.Clear();

    public virtual void UpdateCurrentState(State<TKey, TStatus> state) 
    {
        CurrentState = new State<TKey, TStatus>(state.Status, state.DateSigned, state.SignedBy, state.Note);
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new();

        for (int i = 0; i < Statuses.Count; i++)
        {
            stringBuilder.Append($"Step ({i + 1}): {Statuses[i].Status.ToString()}");

            if (Statuses[i].DateSigned is not null)
            {
                stringBuilder.Append($" at {Statuses[i].DateSigned}");
            }

            if (Statuses[i].SignedBy is not null)
            {
                stringBuilder.Append($" by {Statuses[i].SignedBy}");
            }

            if (!string.IsNullOrEmpty(Statuses[i].Note))
            {
                stringBuilder.Append($" with note: {Statuses[i].Note}");
            }

            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }
}
