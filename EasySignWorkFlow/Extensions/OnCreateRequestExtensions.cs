using EasySignWorkFlow.Commons;
using EasySignWorkFlow.Enums;
using EasySignWorkFlow.Exceptions;
using EasySignWorkFlow.Models;

namespace EasySignWorkFlow.Extensions;

public static class OnCreateRequestExtensions
{
    public static ActionResult<TStatus> OnCreate<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        TKey? signedBy = default,
        string note = "")

        where TKey : IEquatable<TKey>
        where TStatus : struct, Enum
        where TRequest : Request<TKey, TStatus>
    {
        if (request.Statuses.Any())
            throw new InitialStatusAlreadyExist();

        request.AddState(new State<TKey, TStatus>(flowMachine.InitStatus, flowMachine.GetCurrentDateTime(), signedBy, note));

        return ActionResult<TStatus>.SetSuccess(
            ActionType.OnCreate,
            request.CurrentStatus,
            request.CurrentStatus);
    }
}
