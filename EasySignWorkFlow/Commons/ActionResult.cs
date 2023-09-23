using EasySignWorkFlow.Enums;

namespace EasySignWorkFlow.Commons;

public sealed class ActionResult<TStatus>
    where TStatus : struct, Enum
{
    public ActionType ActionType { get; private set; }
    public TStatus? PreviousStatus { get; private set; }
    public TStatus? NewStatus { get; private set; }
    public bool IsSucceeded { get; private set; }
    public string? Message { get; private set; }


    internal static ActionResult<TStatus> SetSuccess(ActionType actionType, TStatus? previousStatus, TStatus? newStatus)
    {
        return new ActionResult<TStatus>()
        {
            IsSucceeded = true,
            Message = string.Empty,
            ActionType = actionType,
            PreviousStatus = previousStatus,
            NewStatus = newStatus,
        };
    }

    internal static ActionResult<TStatus> SetFailed(ActionType actionType, TStatus? previousStatus, TStatus? newStatus, string message)
    {
        return new ActionResult<TStatus>()
        {
            IsSucceeded = false,
            Message = message,
            ActionType = actionType,
            PreviousStatus = previousStatus,
            NewStatus = newStatus,
        };
    }

    public static bool operator true(ActionResult<TStatus> actionResult) => actionResult.IsSucceeded;
    public static bool operator false(ActionResult<TStatus> actionResult) => !actionResult.IsSucceeded;

    public static implicit operator bool(ActionResult<TStatus> actionResult) => actionResult.IsSucceeded;
}
