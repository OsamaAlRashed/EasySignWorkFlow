using EasySignWorkFlow.Commons;
using EasySignWorkFlow.Enums;
using EasySignWorkFlow.Exceptions;
using EasySignWorkFlow.Models;

namespace EasySignWorkFlow.Extensions;

public static class OnCreateRequestExtensions
{
    public static Result<TStatus> OnCreate<TRequest, TKey, TStatus>(
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

        request.Add(new State<TKey, TStatus>(flowMachine.InitStatus, flowMachine.GetCurrentDateTime(), signedBy, note));

        if (flowMachine.OnInitAsync is not null)
            flowMachine.OnInitAsync(request, flowMachine.InitStatus, flowMachine.InitStatus);

        return Result<TStatus>.SetSuccess(
            ActionType.OnCreate,
            flowMachine.InitStatus,
            request.CurrentStatus);
    }
}
