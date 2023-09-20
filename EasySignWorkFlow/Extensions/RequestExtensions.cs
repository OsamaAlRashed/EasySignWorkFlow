using EasySignWorkFlow;
using EasySignWorkFlow.Models;
using System.Net.NetworkInformation;

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
        request.AddState(new State<TKey, TStatus>(flowMachine.InitStatus, DateTime.Now, signedBy, ""));
    }

    public static bool Approve<TRequest, TKey, TStatus>(this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine, Action<TRequest>? action = null)
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
        if (request.CurrentStatus is null)
            throw new ArgumentNullException(nameof(request), "No states yet.");

        flowMachine.SetTransaction((request, current, next) => request.AddState(new State<TKey, TStatus>(next, DateTime.Now, signedBy, note)));

        action(request);

        var result = flowMachine.Fire(request, request.CurrentStatus);
        if (result)
            action(request);

        return result;
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