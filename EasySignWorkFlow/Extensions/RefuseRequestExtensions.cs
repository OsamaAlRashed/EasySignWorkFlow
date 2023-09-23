using EasySignWorkFlow.Commons;
using EasySignWorkFlow.Enums;
using EasySignWorkFlow.Exceptions;
using EasySignWorkFlow.Models;

namespace EasySignWorkFlow.Extensions;

public static class RefuseRequestExtensions
{
    public static ActionResult<TStatus> Refuse<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
    where TRequest : Request<TKey, TStatus>
        => request.Refuse(flowMachine, default, action);

    public static ActionResult<TStatus> Refuse<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        TKey signedBy,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
    where TRequest : Request<TKey, TStatus>
        => request.Refuse(flowMachine, signedBy, string.Empty, action);

    public static ActionResult<TStatus> Refuse<TRequest, TKey, TStatus>(
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

        if (!flowMachine.RefuseStatus.HasValue)
            throw new RefuseNotSetException();

        if (request.CurrentStatus.Value.IsRefuseStatus(flowMachine) ||
            request.CurrentStatus.Value.IsCancelStatus(flowMachine) ||
            request.CurrentStatus.Value.IsFinalStatus(flowMachine))
        {
            return ActionResult<TStatus>.SetFailed(
                ActionType.Refuse,
                request.CurrentStatus,
                request.CurrentStatus,
                "Can not refuse the request if the current status is refused, canceled, or it is in final status");
        }

        request.AddState(new State<TKey, TStatus>(flowMachine.RefuseStatus.Value, flowMachine.GetCurrentDateTime(), signedBy, note));
        if (action is not null)
            action(request);

        return ActionResult<TStatus>.SetSuccess(
            ActionType.Refuse,
            request.CurrentStatus,
            flowMachine.RefuseStatus);
    }
}
