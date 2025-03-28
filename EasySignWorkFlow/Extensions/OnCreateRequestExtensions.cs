﻿using EasySignWorkFlow.Abstractions;
using EasySignWorkFlow.Commons;
using EasySignWorkFlow.Enums;
using EasySignWorkFlow.Exceptions;
using EasySignWorkFlow.Models;

namespace EasySignWorkFlow.Extensions;

public static class OnCreateRequestExtensions
{
    public static Result<TStatus> OnCreate<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        TKey? signedBy = default,
        string note = "",
        Action<TRequest>? action = default)

        where TKey : IEquatable<TKey>
        where TStatus : struct, Enum
        where TRequest : IRequest<TKey, TStatus>
    {
        if (request.States.Any())
            throw new InitialStatusAlreadyExist();

        var newState = new State<TKey, TStatus>(flowMachine.InitStatus, flowMachine.GetCurrentDateTime(), signedBy, note);
        request.Add(newState);
        request.UpdateCurrentState(newState);

        if (flowMachine.OnInitAsync is not null)
            flowMachine.OnInitAsync(request, flowMachine.InitStatus, flowMachine.InitStatus);

        if (action is not null)
            action(request);

        return Result<TStatus>.SetSuccess(
            ActionType.OnCreate,
            flowMachine.InitStatus,
            request.CurrentState!.Status);
    }
}
