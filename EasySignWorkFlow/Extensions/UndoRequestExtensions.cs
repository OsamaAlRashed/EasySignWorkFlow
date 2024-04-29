using EasySignWorkFlow.Commons;
using EasySignWorkFlow.Exceptions;
using EasySignWorkFlow.Models;

namespace EasySignWorkFlow.Extensions;

public static class UndoRequestExtensions
{
    public static Result<TStatus> Undo<TRequest, TKey, TStatus>(
        this TRequest request,
        TKey undoBy,
        Action<TRequest>? action = default)
        where TKey : IEquatable<TKey>
        where TStatus : struct, Enum
        where TRequest : Request<TKey, TStatus>
    {
        if (request.CurrentState is null)
            throw new CurrentStateNullException();

        if (request.Statuses.Count == 1)
            throw new InvalidOperationException("The initial state cannot be undone.");

        var lastState = request.Statuses[^1];

        if (lastState.SignedBy is null || !lastState.SignedBy.Equals(undoBy))
        {
            return Result<TStatus>
                .SetFailed(Enums.ActionType.Undo, lastState.Status, lastState.Status, "Can not undo!");
        }

        request.Remove(lastState);

        if (action is not null)
            action(request);

        return Result<TStatus>.SetSuccess(
            Enums.ActionType.Undo,
            lastState.Status,
            request.Statuses[^1].Status);
    }
}
