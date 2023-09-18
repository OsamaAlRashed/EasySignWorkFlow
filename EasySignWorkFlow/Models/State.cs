namespace EasySignWorkFlow.Models;

public  class State<TKey, TStatus>
    where TKey : IEquatable<TKey>
    where TStatus : Enum
{
    
    public State() { }

    public State(TStatus status, DateTime dateSigned, TKey signedBy, string note = "")
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

    public TStatus Status { get; }
    public DateTime? DateSigned { get; }
    public TKey SignedBy { get; private set; }
    public string? Note { get;private set; }
    public bool IsSigned => DateSigned.HasValue;

    
    public State<TKey, TStatus> New(TKey signedBy, string note) => new(Status, DateTime.Now, signedBy, note);
    public override bool Equals(object? obj)
    {
        
        if (obj is  State<TKey, TStatus> other)
        {
            return other.Status.Equals(Status);    
        }

        return false;

    }
    

    public override int GetHashCode()
    {
        return HashCode.Combine(Status);
    }
}
