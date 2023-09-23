using EasySignWorkFlow.Commons;
using EasySignWorkFlow.Enums;
using EasySignWorkFlow.Exceptions;
using EasySignWorkFlow.Models;

namespace EasySignWorkFlow.Extensions;

public static class ResetRequestExtensions
{
    public static ActionResult<TStatus> Reset<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : Enum
    where TRequest : Request<TKey, TStatus>
        => request.Reset(flowMachine, default, action);

    public static ActionResult<TStatus> Reset<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        TKey signedBy,
        Action<TRequest>? action = default)
    where TKey : IEquatable<TKey>
    where TStatus : Enum
    where TRequest : Request<TKey, TStatus>
        => request.Reset(flowMachine, signedBy, string.Empty, action);

    public static ActionResult<TStatus> Reset<TRequest, TKey, TStatus>(
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

        if (!request.CurrentStatus.IsRefuseStatus(flowMachine))
        {
            return ActionResult<TStatus>.SetFailed(
                ActionType.Reset,
                request.CurrentStatus,
                request.CurrentStatus,
                "Can not reset the request if the current status is not refused.");
        }

        request.AddState(new State<TKey, TStatus>(flowMachine.InitStatus, flowMachine.GetCurrentDateTime(), signedBy, note));
        if (action is not null)
            action(request);

        return ActionResult<TStatus>.SetSuccess(
            ActionType.Reset,
            request.CurrentStatus,
            flowMachine.InitStatus);
    }
}
