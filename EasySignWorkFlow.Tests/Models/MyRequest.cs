﻿using EasySignWorkFlow.Abstractions;
using EasySignWorkFlow.Models;

namespace EasySignWorkFlow.Tests.Models;

public class MyRequest : IRequest<Guid, MyRequestStatus>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Value { get; set; }

    public List<State<Guid, MyRequestStatus>> States { get; } = [];
    public State<Guid, MyRequestStatus>? CurrentState { get; set; }

    internal void Add(State<Guid, MyRequestStatus> state) => States.Add(state);

    internal void Clear() => States.Clear();

    internal void Remove(State<Guid, MyRequestStatus> state) => States.Remove(state);

    internal virtual void UpdateCurrentState(State<Guid, MyRequestStatus> state)
    {
        CurrentState = new State<Guid, MyRequestStatus>(state.Status, state.DateSigned, state.SignedBy, state.Note);
    }
}

public enum MyRequestStatus
{
    Draft,
    WaitingForManager1,
    WaitingForManager2,
    Accepted,
    Refused,
    Canceled,
}
