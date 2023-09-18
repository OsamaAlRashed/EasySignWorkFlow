using EasySignWorkFlow;
using EasySignWorkFlow.Models;

namespace EasySignWorkFlow.Extensions;

public static class RequestExtensions
{
    public static bool Approve<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, Action<TRequest> action = null)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
        => request.Approve(flowMachine, default, action);

    public static bool Approve<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, TKey signedBy, Action<TRequest> action = null)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
        => request.Approve(flowMachine, default, "", action);

    public static bool Approve<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, TKey signedBy, string note, Action<TRequest> action = null)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
    {
        if (request.CurrentStatus == null)
            throw new ArgumentNullException(nameof(request), "No states yet.");

        var func = flowMachine.Map[request.CurrentStatus];
        var nextStatus = func(request);

        request.AddState(new State<TKey, TStatus>(nextStatus, DateTime.Now, signedBy, note));
        action(request);

        return true;
    }

    public static bool Approve2<TRequest, TStatusEnum,TStatus, TKey>(this TRequest request,
        FlowMachine2<TRequest, TStatus> flowMachine, TKey signedBy, string note)
        where TKey : IEquatable<TKey>
        where TStatusEnum : Enum
        where TRequest : Request<TKey, TStatusEnum>
        
    {
        if (request.CurrentStatus == null)
            throw new ArgumentNullException(nameof(request), "No states yet.");

        // flowMachine.Fire(request,() => request.CurrentStatus2);
        //
        return true;
    }

    public static bool Reject<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, Action<TRequest> action = null)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
    {
        return true;
    }

    public static bool Reset<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, Action<TRequest> action = null)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
    {
        return true;
    }

    public static bool Sign<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, Action<TRequest> action = null)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
    {
        return true;
    }

    public static bool Cancel<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, Action<TRequest> action = null)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
    {
        return true;
    }
}