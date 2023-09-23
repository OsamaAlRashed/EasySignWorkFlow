﻿using EasySignWorkFlow.Exceptions;
using EasySignWorkFlow.Models;

namespace EasySignWorkFlow.Extensions;

public static class GetNextUsersRequestExtensions
{
    public static IEnumerable<TKey> GetNextUsers<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine)
    where TKey : IEquatable<TKey>
    where TStatus : Enum
    where TRequest : Request<TKey, TStatus>
        => GetNextUsersAsync(request, flowMachine).GetAwaiter().GetResult();

    public static async Task<IEnumerable<TKey>> GetNextUsersAsync<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine)
    where TKey : IEquatable<TKey>
    where TStatus : Enum
    where TRequest : Request<TKey, TStatus>
    {
        if (request.CurrentStatus is null)
            throw new CurrentStatusNullException();

        foreach (var transaction in flowMachine.Map[request.CurrentStatus])
        {
            if (await transaction.ValidAsync(request))
            {
                return await transaction.GetNextUserAsync(request, request.CurrentStatus);
            }
        }

        return Enumerable.Empty<TKey>();
    }
}
