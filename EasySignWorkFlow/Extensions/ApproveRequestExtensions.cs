using EasySignWorkFlow.Commons;
using EasySignWorkFlow.Enums;
using EasySignWorkFlow.Exceptions;
using EasySignWorkFlow.Models;

namespace EasySignWorkFlow.Extensions;

public static class ApproveRequestExtensions
{
    public static ActionResult<TStatus> Approve<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : Enum
    where TRequest : Request<TKey, TStatus>
        => request!.Approve(flowMachine, default, action);

    public static ActionResult<TStatus> Approve<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        TKey signedBy,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : Enum
    where TRequest : Request<TKey, TStatus>
        => request.Approve(flowMachine, signedBy, string.Empty, action);

    public static ActionResult<TStatus> Approve<TRequest, TKey, TStatus>(
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

        if (request.CurrentStatus.IsRefuseStatus(flowMachine) ||
            request.CurrentStatus.IsCancelStatus(flowMachine) ||
            request.CurrentStatus.IsFinalStatus(flowMachine))
        {
            return ActionResult<TStatus>.SetFailed(
                ActionType.Approve,
                request.CurrentStatus,
                request.CurrentStatus,
                "Can not approve the request if the current status is refused, cancel, or it is in final status");
        }

        TStatus? nextStatus = request.CurrentStatus;

        flowMachine.SetTransition((request, current, next) => 
        {
            nextStatus = next;
            request.AddState(new State<TKey, TStatus>(next, flowMachine.GetCurrentDateTime(), signedBy, note));
        });

        var result = flowMachine.Fire(request, request.CurrentStatus);
        if (result && action is not null)
            action(request);

        return ActionResult<TStatus>.SetSuccess(
            ActionType.Approve,
            request.CurrentStatus,
            nextStatus);
    }
}