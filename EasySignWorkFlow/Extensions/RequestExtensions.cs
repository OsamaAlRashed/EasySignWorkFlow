using EasySignWorkFlow.Models;

namespace EasySignWorkFlow.Extensions;

public static class RequestExtensions
{
    public static void OnCreate<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        TKey? signedBy = default)
        
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
    {
        if (request.Statuses.Any())
        {
            throw new Exception("");
        }

        request.AddState(new State<TKey, TStatus>(flowMachine.InitStatus, DateTime.Now, signedBy, string.Empty));
    }

    public static bool Approve<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, Action<TRequest>? action = default)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
        => request.Approve(flowMachine, default, action);

    public static bool Approve<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, TKey signedBy, Action<TRequest>? action = default)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
        => request.Approve(flowMachine, signedBy, string.Empty, action);

    public static bool Approve<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, TKey signedBy, string note, Action<TRequest>? action = default)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus> 
        => Handle(request, flowMachine, signedBy, note, action);

    public static bool Refuse<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, Action<TRequest>? action = default)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
        => request.Refuse(flowMachine, default, action);

    public static bool Refuse<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, TKey signedBy, Action<TRequest>? action = default)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
        => request.Refuse(flowMachine, signedBy, string.Empty, action);

    public static bool Refuse<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, TKey signedBy, string note, Action<TRequest>? action = default)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
        => Handle(request, flowMachine, signedBy, note, action);

    public static bool Reset<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, Action<TRequest>? action = default)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
        => request.Reset(flowMachine, default, action);

    public static bool Reset<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, TKey signedBy, Action<TRequest>? action = default)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
        => request.Reset(flowMachine, signedBy, string.Empty, action);

    public static bool Reset<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, TKey signedBy, string note, Action<TRequest>? action = default)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
        => Reset(request, flowMachine, signedBy, note, action);

    public static bool Cancel<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, Action<TRequest>? action = default)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
        => request.Cancel(flowMachine, default, action);

    public static bool Cancel<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, TKey signedBy, Action<TRequest>? action = default)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
        => request.Cancel(flowMachine, signedBy, string.Empty, action);

    public static bool Cancel<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, TKey signedBy, string note, Action<TRequest>? action = default)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
        => Reset(request, flowMachine, signedBy, note, action);

    private static bool Handle<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine,
        TKey? signedBy,
        string? note,
        Action<TRequest>? action = default)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
    {
        if (request.CurrentStatus is null)
            throw new ArgumentNullException(nameof(request), "No states yet.");

        flowMachine.SetTransaction((request, current, next) => request.AddState(new State<TKey, TStatus>(next, DateTime.Now, signedBy, note)));

        var result = flowMachine.Fire(request, request.CurrentStatus);
        if (result && action is not null)
            action(request);

        return result;
    }
}