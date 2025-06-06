using Microsoft.EntityFrameworkCore;

namespace Netcorext.EntityFramework.UserIdentityPattern.AspNetCore;

public static class ApplicationBuilderExtension
{
    public static string GenerateDdl<TEntity>(this IApplicationBuilder builder) => GenerateDdl(builder);

    public static string GenerateDdl(this IApplicationBuilder builder)
    {
        using var serviceScope = builder.ApplicationServices.CreateScope();

        var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContextAdapter>();

        return context.Master.Database.GenerateCreateScript();
    }

    public static bool EnsureCreateDatabase<TEntity>(this IApplicationBuilder builder) => EnsureCreateDatabase(builder);

    public static bool EnsureCreateDatabase(this IApplicationBuilder builder)
    {
        using var serviceScope = builder.ApplicationServices.CreateScope();

        var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContextAdapter>();

        return context.Master.Database.EnsureCreated();
    }

    public static bool EnsureDeleteDatabase<TEntity>(this IApplicationBuilder builder) => EnsureDeleteDatabase(builder);

    public static bool EnsureDeleteDatabase(this IApplicationBuilder builder)
    {
        using var serviceScope = builder.ApplicationServices.CreateScope();

        var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContextAdapter>();

        return context.Master.Database.EnsureDeleted();
    }

    public static void MigrateDatabase<TEntity>(this IApplicationBuilder builder) => MigrateDatabase(builder);

    public static void MigrateDatabase(this IApplicationBuilder builder)
    {
        using var serviceScope = builder.ApplicationServices.CreateScope();

        var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContextAdapter>();

        context.Master.Database.Migrate();
    }
    public static void WarmupDbContext(this IApplicationBuilder builder)
    {
        Task.Run(async () =>
                 {
                     using var serviceScope = builder.ApplicationServices.CreateScope();

                     var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContextAdapter>();

                     try
                     {
                         _ = context.Master.Model;
                         _ = context.Slave.Model;

                         await context.Master.Database.OpenConnectionAsync();
                         await context.Slave.Database.OpenConnectionAsync();

                         await context.Master.Database.ExecuteSqlRawAsync("SELECT 1");
                         await context.Slave.Database.ExecuteSqlRawAsync("SELECT 1");

                         await context.Master.Database.CloseConnectionAsync();
                         await context.Slave.Database.CloseConnectionAsync();
                     }
                     catch (Exception ex)
                     {
                         Console.Error.WriteLine(ex);
                     }
                 });
    }
}
