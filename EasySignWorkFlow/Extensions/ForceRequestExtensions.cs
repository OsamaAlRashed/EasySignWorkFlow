using EasySignWorkFlow.Commons;
using EasySignWorkFlow.Enums;
using EasySignWorkFlow.Exceptions;
using EasySignWorkFlow.Models;

namespace EasySignWorkFlow.Extensions;

public static class ForceRequestExtensions
{
    public static Result<TStatus> Force<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        TStatus status,
        TKey signedBy,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
    where TRequest : Request<TKey, TStatus>
        => request.Force(flowMachine, status, signedBy, string.Empty, action);

    public static Result<TStatus> Force<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        TStatus status,
        TKey signedBy,
        string note,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
    where TRequest : Request<TKey, TStatus>
    {
        if (request.CurrentState is null)
            throw new CurrentStateNullException();

        var newState = new State<TKey, TStatus>(status, flowMachine.GetCurrentDateTime(), signedBy, note);
        request.Add(newState);
        request.UpdateCurrentState(newState);

        if (action is not null)
            action(request);

        return Result<TStatus>.SetSuccess(
            ActionType.Approve,
            request.CurrentState.Status,
            status);
    }
}
