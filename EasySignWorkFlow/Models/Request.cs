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
        => Statuses.Count > 0 ? Statuses[^1] : null;

    [NotMapped]
    public virtual TStatus? CurrentStatus
        => CurrentState?.Status;

    [NotMapped]
    public virtual State<TKey, TStatus>? PreviousState
        => Statuses.Count > 1 ? Statuses[^2] : null;

    [NotMapped]
    public virtual TStatus? PreviousStatus 
        => Statuses.Count > 1 ? Statuses[^2].Status : null;

    [NotMapped]
    public virtual DateTime? LastSignDate
        => CurrentState?.DateSigned;

    [NotMapped]
    public virtual TKey? LastSignBy
        => CurrentState is null ? default : CurrentState.SignedBy; // Todo

    internal void Add(State<TKey, TStatus> state) => _statuses.Add(state);

    internal void Clear() => _statuses.Clear();

    public override string ToString()
    {
        StringBuilder stringBuilder = new();

        for (int i = 0; i < Statuses.Count; i++)
        {
            stringBuilder.Append($"Step ({i}): {Statuses[i].Status.ToString()}");

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
