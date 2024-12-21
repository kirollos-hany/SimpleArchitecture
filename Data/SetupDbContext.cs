using Microsoft.EntityFrameworkCore;
using SimpleArchitecture.Auditing.Interceptors;
using SimpleArchitecture.EventHandling.Interceptors;

namespace SimpleArchitecture.Data;

public static class SetupDbContext
{
    public static void SetupContext(this IServiceCollection serviceCollection, string connectionString)
    {
        serviceCollection.AddDbContext<SimpleArchitectureDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, opts =>
            {
                opts.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                opts.MigrationsAssembly("SimpleArchitecture");
            });

            options.AddInterceptors(sp.GetRequiredService<ActionAuditInterceptor>(),
                sp.GetRequiredService<FireEventInterceptor>());
        });
    }
}