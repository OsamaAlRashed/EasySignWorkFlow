namespace EasySignWorkFlow.Models;

public  class State<TKey, TStatus>
    where TKey : IEquatable<TKey>
    where TStatus : Enum
{
    protected State() { }

    public State(TStatus status, DateTime dateSigned, TKey signedBy, string note = "")
    {
        Status = status;
        DateSigned = dateSigned;
        SignedBy = signedBy;
        Note = note;
    }

    public TStatus Status { get; }
    public DateTime? DateSigned { get; }
    public TKey SignedBy { get; }
    public string? Note { get; }
    public bool IsSigned => DateSigned.HasValue;
}
