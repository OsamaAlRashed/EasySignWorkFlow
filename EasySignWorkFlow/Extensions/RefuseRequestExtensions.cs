using EasySignWorkFlow.Commons;
using EasySignWorkFlow.Enums;
using EasySignWorkFlow.Exceptions;
using EasySignWorkFlow.Models;

namespace EasySignWorkFlow.Extensions;

public static class RefuseRequestExtensions
{
    public static Result<TStatus> Refuse<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
    where TRequest : Request<TKey, TStatus>
        => request.Refuse(flowMachine, default, action);

    public static Result<TStatus> Refuse<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        TKey signedBy,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
    where TRequest : Request<TKey, TStatus>
        => request.Refuse(flowMachine, signedBy, string.Empty, action);

    public static Result<TStatus> Refuse<TRequest, TKey, TStatus>(
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
            return Result<TStatus>.SetFailed(
                ActionType.Refuse,
                request.CurrentStatus,
                request.CurrentStatus,
                "Can not refuse the request if the current status is refused, canceled, or it is in final status");
        }

        TStatus current = request.CurrentStatus.Value;

        request.Add(new State<TKey, TStatus>(flowMachine.RefuseStatus.Value, flowMachine.GetCurrentDateTime(), signedBy, note));
        if (action is not null)
            action(request);

        if (flowMachine.OnRefuseAsync is not null)
            flowMachine.OnRefuseAsync(request, current, flowMachine.RefuseStatus.Value);

        return Result<TStatus>.SetSuccess(
            ActionType.Refuse,
            current,
            flowMachine.RefuseStatus);
    }
}
