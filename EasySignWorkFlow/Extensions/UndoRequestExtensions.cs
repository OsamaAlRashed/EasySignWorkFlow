using EasySignWorkFlow.Commons;
using EasySignWorkFlow.Exceptions;
using EasySignWorkFlow.Models;
namespace EasySignWorkFlow.Extensions;

public static class UndoRequestExtensions
{
    //public static Result<TStatus> Undo<TRequest, TKey, TStatus>(
    //    this TRequest request,
    //    FlowMachine<TRequest, TKey, TStatus> flowMachine,
    //    TKey signedBy)
    //    where TKey : IEquatable<TKey>
    //    where TStatus : struct, Enum
    //    where TRequest : Request<TKey, TStatus>
    //{
    //    if (!request.CurrentStatus.HasValue)
    //        throw new CurrentStatusNullException();

    //    if (!request.PreviousStatus.HasValue)
    //        throw new InvalidOperationException("");

    //    if (!request.PreviousState!.SignedBy!.Equals(signedBy))
    //    {

    //    }

    //    return Result<TStatus>.SetSuccess();
    //}
}
