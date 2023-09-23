using EasySignWorkFlow.Enums;

namespace EasySignWorkFlow.Commons;

public sealed class Result<TStatus>
    where TStatus : struct, Enum
{
    public Result(
        ActionType actionType,
        TStatus? previousStatus,
        TStatus? newStatus,
        bool isSucceeded,
        string? message)
    {
        ActionType = actionType;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        IsSucceeded = isSucceeded;
        Message = message;
    }

    public ActionType ActionType { get; private set; }
    public TStatus? PreviousStatus { get; private set; }
    public TStatus? NewStatus { get; private set; }
    public bool IsSucceeded { get; private set; }
    public string? Message { get; private set; }


    internal static Result<TStatus> SetSuccess(ActionType actionType, TStatus? previousStatus, TStatus? newStatus)
        => new(actionType, previousStatus, newStatus, true, string.Empty);

    internal static Result<TStatus> SetFailed(ActionType actionType, TStatus? previousStatus, TStatus? newStatus, string message)
        => new(actionType, previousStatus, newStatus, false, message);

    public static bool operator true(Result<TStatus> actionResult) => actionResult.IsSucceeded;
    public static bool operator false(Result<TStatus> actionResult) => !actionResult.IsSucceeded;

    public static implicit operator bool(Result<TStatus> actionResult) => actionResult.IsSucceeded;
}
