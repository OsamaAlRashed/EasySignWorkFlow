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

    public virtual TStatus? CurrentStatus { get; private set; }
    public virtual DateTime? LastSignDate { get; private set; }
    public virtual TKey? LastSignBy { get; private set; }

    [NotMapped]
    public virtual State<TKey, TStatus>? PreviousState
        => Statuses.Count > 1 ? Statuses[^2] : null;

    [NotMapped]
    public virtual State<TKey, TStatus>? CurrentState
    => _statuses
        .OrderByDescending(x => x.DateSigned)
        .FirstOrDefault();

    [NotMapped]
    public virtual TStatus? PreviousStatus
        => Statuses.Count > 1 ? Statuses[^2].Status : null;
    

    internal void Add(State<TKey, TStatus> state) => _statuses.Add(state);

    internal void Clear() => _statuses.Clear();

    internal void UpdateCurrentState(State<TKey, TStatus> state)
    {
        CurrentStatus = state.Status;
        LastSignBy = state.SignedBy;
        LastSignDate = state.DateSigned;
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
