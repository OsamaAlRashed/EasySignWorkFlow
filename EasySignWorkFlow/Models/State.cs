namespace EasySignWorkFlow.Models;

public sealed class State<TKey, TStatus>
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
{
    public State(TStatus status, DateTime? dateSigned, TKey? signedBy, string? note)
    {
        Status = status;
        DateSigned = dateSigned;
        SignedBy = signedBy;
        Note = note;
    }

    public State(TStatus status)
    {
        Status = status;
    }

    public TStatus Status { get; private set; }
    public DateTime? DateSigned { get; private set; }
    public TKey? SignedBy { get; private set; }
    public string? Note { get; private set; }
    public bool IsSigned => DateSigned.HasValue;
    public string StatusName => Status.ToString();

    public override bool Equals(object? obj)
    {
        if (obj is State<TKey, TStatus> other)
        {
            return other.Status.Equals(Status);    
        }

        return false;
    }

    public override int GetHashCode() 
        => HashCode.Combine(Status);
}
