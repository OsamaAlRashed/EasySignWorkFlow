using EasySignWorkFlow.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EasySignWorkFlow;

public static class ModelBuilderExtensions
{
    public static void ConfigureRequest<TRequest, TKey, TStatus>(this ModelBuilder modelBuilder)
        where TRequest : class, IRequest<TKey, TStatus>
        where TKey : IEquatable<TKey>
        where TStatus : struct, Enum
    {
        modelBuilder.Entity<TRequest>()
            .OwnsMany($"{typeof(TRequest).Name}States", x => x.Statuses);

        modelBuilder.Entity<TRequest>()
            .OwnsOne(x => x.CurrentState);
    }
}
