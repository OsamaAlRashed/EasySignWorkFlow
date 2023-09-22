﻿using System.ComponentModel.DataAnnotations.Schema;

namespace EasySignWorkFlow.Models;

public abstract class Request<TKey, TStatus>
    where TKey : IEquatable<TKey>
    where TStatus : Enum
{
    private readonly List<State<TKey, TStatus>> _statuses = new();

    public IReadOnlyList<State<TKey, TStatus>> Statuses => _statuses.AsReadOnly();

    [NotMapped]
    public virtual TStatus? CurrentStatus => _statuses
        .OrderByDescending(x => x.DateSigned)
        .Select(x => x.Status).FirstOrDefault();  
    
    [NotMapped]
    public virtual State<TKey, TStatus> CurrentState => _statuses
        .OrderByDescending(x => x.DateSigned)
        .First();

    [NotMapped]
    public virtual TStatus? PreviousStatus 
        => Statuses.Count > 1 ? Statuses[^2].Status : default;

    [NotMapped]
    public virtual DateTime? LastSignDate => Statuses
        .OrderByDescending(x => x.DateSigned)
        .Select(x => x.DateSigned).FirstOrDefault();

    [NotMapped]
    public virtual TKey? LastSignBy => Statuses
        .OrderByDescending(x => x.DateSigned)
        .Select(x => x.SignedBy).FirstOrDefault();

    internal void AddState(State<TKey, TStatus> state) => _statuses.Add(state);

    internal void Reset() => _statuses.Clear();
}
