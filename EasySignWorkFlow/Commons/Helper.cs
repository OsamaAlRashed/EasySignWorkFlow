using EasySignWorkFlow.Abstractions;

namespace EasySignWorkFlow.Commons;

internal static class Helper
{
    internal static bool IsRefuseStatus<TRequest, TKey, TStatus>(this TStatus currentStatus, FlowMachine<TRequest, TKey, TStatus> flowMachine)
        where TKey : IEquatable<TKey>
        where TStatus : struct, Enum
        where TRequest : IRequest<TKey, TStatus> 
        => currentStatus.Equals(flowMachine.RefuseStatus);

    internal static bool IsCancelStatus<TRequest, TKey, TStatus>(this TStatus currentStatus, FlowMachine<TRequest, TKey, TStatus> flowMachine)
        where TKey : IEquatable<TKey>
        where TStatus : struct, Enum
        where TRequest : IRequest<TKey, TStatus>
        => currentStatus.Equals(flowMachine.CancelStatus);

    internal static bool IsFinalStatus<TRequest, TKey, TStatus>(this TStatus currentStatus, FlowMachine<TRequest, TKey, TStatus> flowMachine)
        where TKey : IEquatable<TKey>
        where TStatus : struct, Enum
        where TRequest : IRequest<TKey, TStatus>
        => !flowMachine.Map.ContainsKey(currentStatus) ||
           flowMachine.Map[currentStatus].Count == 0;

    internal static bool IsInitialStatus<TRequest, TKey, TStatus>(this TStatus currentStatus, FlowMachine<TRequest, TKey, TStatus> flowMachine)
        where TKey : IEquatable<TKey>
        where TStatus : struct, Enum
        where TRequest : IRequest<TKey, TStatus>
        => currentStatus.Equals(flowMachine.InitStatus);
}
