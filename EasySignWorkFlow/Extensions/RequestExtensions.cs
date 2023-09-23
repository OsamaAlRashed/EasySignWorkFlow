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
        if (request.Statuses.Any())
        {
            throw new Exception("Request already has an initial status.");
        }

        request.AddState(new State<TKey, TStatus>(flowMachine.InitStatus, DateTime.Now, signedBy, string.Empty));
    }


    #region Approve
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
    {
        if(request.CurrentStatus!.Equals(flowMachine.RefuseStatus) ||
           request.CurrentStatus!.Equals(flowMachine.CancelStatus) ||
           !flowMachine.Map.ContainsKey(request.CurrentStatus) || 
           !flowMachine.Map[request.CurrentStatus].Any()) // final status
        {
            return false;
        }

        return Handle(request, flowMachine, signedBy, note, action);
    }

    #endregion

    #region Refuse
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
    {
        if (request.CurrentStatus!.Equals(flowMachine.RefuseStatus) ||
           request.CurrentStatus!.Equals(flowMachine.CancelStatus) ||
           !flowMachine.Map.ContainsKey(request.CurrentStatus) ||
           !flowMachine.Map[request.CurrentStatus].Any()) // final status
        {
            return false;
        }

        request.AddState(new State<TKey, TStatus>(flowMachine.RefuseStatus, DateTime.Now, signedBy, string.Empty));
        if (action is not null)
            action(request);

        return true;
    }
    #endregion

    #region Reset
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
    {
        if (!request.CurrentStatus!.Equals(flowMachine.RefuseStatus))
        {
            return false;
        }

        request.AddState(new State<TKey, TStatus>(flowMachine.InitStatus, DateTime.Now, signedBy, string.Empty));
        if (action is not null)
            action(request);

        return true;
    }


    #endregion

    #region Cancel
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
    {
        if (request.CurrentStatus!.Equals(flowMachine.RefuseStatus) ||
           request.CurrentStatus!.Equals(flowMachine.CancelStatus) ||
           !flowMachine.Map.ContainsKey(request.CurrentStatus) ||
           !flowMachine.Map[request.CurrentStatus].Any()) // final status
        {
            return false;
        }

        request.AddState(new State<TKey, TStatus>(flowMachine.CancelStatus, DateTime.Now, signedBy, string.Empty));
        if (action is not null)
            action(request);

        return true;
    }
    #endregion

    #region GetNextUsers
    public static async Task<IEnumerable<TKey>> GetNextUsersAsync<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus>
    {
        if (request.CurrentStatus is null)
            throw new ArgumentNullException(nameof(request), "No states yet.");

        foreach (var transaction in flowMachine.Map[request.CurrentStatus])
        {
            if (await transaction.ValidAsync(request))
            {
                return await transaction.GetNextUserAsync(request, request.CurrentStatus);
            }
        }

        return Enumerable.Empty<TKey>();
    }

    public static IEnumerable<TKey> GetNextUsers<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine)
        where TKey : IEquatable<TKey>
        where TStatus : Enum
        where TRequest : Request<TKey, TStatus> 
        => GetNextUsersAsync(request, flowMachine).GetAwaiter().GetResult();
    #endregion

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

        flowMachine.SetTransition((request, current, next) => request.AddState(new State<TKey, TStatus>(next, DateTime.Now, signedBy, note)));

        var result = flowMachine.Fire(request, request.CurrentStatus);
        if (result && action is not null)
            action(request);

        return result;
    }
}