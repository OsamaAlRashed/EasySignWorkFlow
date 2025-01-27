using EasySignWorkFlow.Enums;

namespace EasySignWorkFlow.Commons;

public sealed class Result<TStatus>(
    ActionType actionType,
    TStatus? previousStatus,
    TStatus? newStatus,
    bool isSucceeded,
    string? message)
    where TStatus : struct, Enum
{
    public ActionType ActionType { get; private set; } = actionType;
    public TStatus? PreviousStatus { get; private set; } = previousStatus;
    public TStatus? NewStatus { get; private set; } = newStatus;
    public bool IsSucceeded { get; private set; } = isSucceeded;
    public string? Message { get; private set; } = message;


    internal static Result<TStatus> SetSuccess(ActionType actionType, TStatus? previousStatus, TStatus? newStatus)
        => new(actionType, previousStatus, newStatus, true, string.Empty);

    internal static Result<TStatus> SetFailed(ActionType actionType, TStatus? previousStatus, TStatus? newStatus, string message)
        => new(actionType, previousStatus, newStatus, false, message);

    public static bool operator true(Result<TStatus> actionResult) => actionResult.IsSucceeded;
    public static bool operator false(Result<TStatus> actionResult) => !actionResult.IsSucceeded;

    public static implicit operator bool(Result<TStatus> actionResult) => actionResult.IsSucceeded;
}
