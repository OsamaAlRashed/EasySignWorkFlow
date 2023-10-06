using EasySignWorkFlow.Commons;
using EasySignWorkFlow.Enums;
using EasySignWorkFlow.Exceptions;
using EasySignWorkFlow.Models;

namespace EasySignWorkFlow.Extensions;

public static class ApproveRequestExtensions
{
    public static Result<TStatus> Approve<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
    where TRequest : Request<TKey, TStatus>
        => request.Approve(flowMachine, default, action);

    public static Result<TStatus> Approve<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        TKey signedBy,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
    where TRequest : Request<TKey, TStatus>
        => request.Approve(flowMachine, signedBy, string.Empty, action);

    public static Result<TStatus> Approve<TRequest, TKey, TStatus>(
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

        if (request.CurrentStatus.Value.IsRefuseStatus(flowMachine) ||
            request.CurrentStatus.Value.IsCancelStatus(flowMachine) ||
            request.CurrentStatus.Value.IsFinalStatus(flowMachine))
        {
            return Result<TStatus>.SetFailed(
                ActionType.Approve,
                request.CurrentStatus,
                request.CurrentStatus,
                "Can not approve the request if the current status is refused, cancel, or it is in final status");
        }

        TStatus current = request.CurrentStatus.Value;
        TStatus? nextStatus = null;

        flowMachine.SetTransition((request, current, next) => 
        {
            nextStatus = next;
            request.Add(new State<TKey, TStatus>(next, flowMachine.GetCurrentDateTime(), signedBy, note));
        });

        var result = flowMachine.Fire(request, request.CurrentStatus.Value);

        if (!result)
        {
            return Result<TStatus>.SetFailed(
                 ActionType.Approve,
                 request.CurrentStatus,
                 request.CurrentStatus,
                 "Can not find the next state.");
        }

        if (action is not null)
            action(request);

        return Result<TStatus>.SetSuccess(
            ActionType.Approve,
            current,
            nextStatus);
    }
}