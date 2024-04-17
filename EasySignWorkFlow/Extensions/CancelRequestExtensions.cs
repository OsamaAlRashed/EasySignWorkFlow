using EasySignWorkFlow.Commons;
using EasySignWorkFlow.Enums;
using EasySignWorkFlow.Exceptions;
using EasySignWorkFlow.Models;

namespace EasySignWorkFlow.Extensions;

public static class CancelRequestExtensions
{
    public static Result<TStatus> Cancel<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
    where TRequest : Request<TKey, TStatus>
        => request.Cancel(flowMachine, default, action);

    public static Result<TStatus> Cancel<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        TKey signedBy,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
    where TRequest : Request<TKey, TStatus>
        => request.Cancel(flowMachine, signedBy, string.Empty, action);

    public static Result<TStatus> Cancel<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        TKey signedBy,
        string note,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
    where TRequest : Request<TKey, TStatus>
    {
        if (!request.CurrentStatus.HasValue)
            throw new CurrentStatusNullException();

        if (!flowMachine.CancelStatus.HasValue)
            throw new CancelNotSetException();

        if (request.CurrentStatus.Value.IsRefuseStatus(flowMachine) ||
            request.CurrentStatus.Value.IsCancelStatus(flowMachine) ||
            request.CurrentStatus.Value.IsFinalStatus(flowMachine))
        {
            return Result<TStatus>.SetFailed(
                ActionType.Cancel,
                request.CurrentStatus,
                request.CurrentStatus,
                "Can not cancel the request if the current status is refused, canceled, or it is in final status");
        }

        TStatus current = request.CurrentStatus.Value;

        var newState = new State<TKey, TStatus>(flowMachine.CancelStatus.Value, flowMachine.GetCurrentDateTime(), signedBy, note);
        request.Add(newState);
        request.UpdateCurrentState(newState);

        if (action is not null)
            action(request);

        if (flowMachine.OnCancelAsync is not null)
            flowMachine.OnCancelAsync(request, current, flowMachine.CancelStatus.Value);

        return Result<TStatus>.SetSuccess(
            ActionType.Cancel,
            current,
            flowMachine.CancelStatus);
    }
}
