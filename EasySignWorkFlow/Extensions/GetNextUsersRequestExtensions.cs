﻿using EasySignWorkFlow.Abstractions;
using EasySignWorkFlow.Exceptions;
using EasySignWorkFlow.Models;

namespace EasySignWorkFlow.Extensions;

public static class GetNextUsersRequestExtensions
{
    public static IEnumerable<TKey> GetNextUsers<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine)
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
    where TRequest : IRequest<TKey, TStatus>
        => GetNextUsersAsync(request, flowMachine).GetAwaiter().GetResult();

    public static async Task<IEnumerable<TKey>> GetNextUsersAsync<TRequest, TKey, TStatus>(
        this TRequest request,
        FlowMachine<TRequest, TKey, TStatus> flowMachine)
    where TKey : IEquatable<TKey>
    where TStatus : struct, Enum
    where TRequest : IRequest<TKey, TStatus>
    {
        if (request.CurrentState is null)
            throw new CurrentStateNullException();

        if (!flowMachine.Map.ContainsKey(request.CurrentState.Status))
        {
            return Enumerable.Empty<TKey>();
        }

        foreach (var transaction in flowMachine.Map[request.CurrentState.Status])
        {
            if (await transaction.ValidAsync(request))
            {
                return await transaction.GetNextUserAsync(request, request.CurrentState.Status);
            }
        }

        return Enumerable.Empty<TKey>();
    }
}
