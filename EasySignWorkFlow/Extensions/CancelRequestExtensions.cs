using EasySignWorkFlow.Commons;
using EasySignWorkFlow.Enums;
using EasySignWorkFlow.Exceptions;
using EasySignWorkFlow.Models;

namespace EasySignWorkFlow.Extensions;

public static class CancelRequestExtensions
{
    public static ActionResult<TStatus> Cancel<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : Enum
    where TRequest : Request<TKey, TStatus>
        => request.Cancel(flowMachine, default, action);

    public static ActionResult<TStatus> Cancel<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        TKey signedBy,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : Enum
    where TRequest : Request<TKey, TStatus>
        => request.Cancel(flowMachine, signedBy, string.Empty, action);

    public static ActionResult<TStatus> Cancel<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        TKey signedBy,
        string note,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : Enum
    where TRequest : Request<TKey, TStatus>
    {
        if (request.CurrentStatus is null)
            throw new CurrentStatusNullException();

        if(flowMachine.CancelStatus is null)
            throw new CancelNotSetException();

        if (request.CurrentStatus.IsRefuseStatus(flowMachine) ||
            request.CurrentStatus.IsCancelStatus(flowMachine) ||
            request.CurrentStatus.IsFinalStatus(flowMachine))
        {
            return ActionResult<TStatus>.SetFailed(
                ActionType.Cancel,
                request.CurrentStatus,
                request.CurrentStatus,
                "Can not cancel the request if the current status is refused, canceled, or it is in final status");
        }

        request.AddState(new State<TKey, TStatus>(flowMachine.CancelStatus, flowMachine.GetCurrentDateTime(), signedBy, note));
        if (action is not null)
            action(request);

        return ActionResult<TStatus>.SetSuccess(
            ActionType.Cancel,
            request.CurrentStatus,
            flowMachine.CancelStatus);
    }
}
