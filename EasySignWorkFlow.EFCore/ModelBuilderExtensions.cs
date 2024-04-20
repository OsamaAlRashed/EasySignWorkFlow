using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EasySignWorkFlow.EFCore;

public static class ModelBuilderExtensions
{
    public static void ConfigureRequest<TRequest, TKey, TStatus>(this ModelBuilder modelBuilder)
        where TRequest : EFRequest<TKey, TStatus>
        where TKey : IEquatable<TKey>
        where TStatus : struct, Enum
    {
        modelBuilder.Entity<TRequest>()
            .OwnsMany($"{typeof(TRequest).Name}States", x => x.Statuses);

        modelBuilder.Entity<TRequest>()
            .OwnsOne(x => x.CurrentState);
    }

    public static void ConfigureRequests(this ModelBuilder modelBuilder, Assembly assembly)
    {
        var requestTypes = assembly.GetTypes().Where(type =>
            !type.IsAbstract &&
             type.IsClass &&
            typeof(EFRequest<, >).IsAssignableFrom(type));

        foreach (var type in requestTypes)
        {
            var method = typeof(ModelBuilderExtensions)
                .GetMethod(nameof(ConfigureRequest))!
                .MakeGenericMethod(type, type.GetGenericArguments()[0], type.GetGenericArguments()[1]);
            method.Invoke(null, new object[] { modelBuilder });
        }
    }
}
