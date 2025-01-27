using EasySignWorkFlow.Abstractions;
using EasySignWorkFlow.Commons;
using EasySignWorkFlow.Enums;
using EasySignWorkFlow.Exceptions;
using EasySignWorkFlow.Models;

namespace EasySignWorkFlow.Extensions;

public static class ResetRequestExtensions
{
    public static Result<TStatus> Reset<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
    where TRequest : IRequest<TKey, TStatus>
        => request.Reset(flowMachine, default!, action);

    public static Result<TStatus> Reset<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        TKey signedBy,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
    where TRequest : IRequest<TKey, TStatus>
        => request.Reset(flowMachine, signedBy, string.Empty, action);

    public static Result<TStatus> Reset<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        TKey signedBy,
        string note,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
    where TRequest : IRequest<TKey, TStatus>
    {
        if (request.CurrentState is null)
            throw new CurrentStateNullException();

        if (!request.CurrentState.Status.IsRefuseStatus(flowMachine))
        {
            return Result<TStatus>.SetFailed(
                ActionType.Reset,
                request.CurrentState.Status,
                request.CurrentState.Status,
                "Can not reset the request if the current status is not refused.");
        }

        var newState = new State<TKey, TStatus>(flowMachine.InitStatus, flowMachine.GetCurrentDateTime(), signedBy, note);
        request.Add(newState);
        request.UpdateCurrentState(newState);

        if (action is not null)
            action(request);

        return Result<TStatus>.SetSuccess(
            ActionType.Reset,
            flowMachine.RefuseStatus,
            flowMachine.InitStatus);
    }
}
