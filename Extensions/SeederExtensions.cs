using System;

using adeeb.Data;
using AdeebBackend.Data;

namespace AdeebBackend.Extensions;

public static class SeederExtensions
{
    public static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<AppDbContext>();
        DummyDbInitializer.Seed(context);
        return app;
    }
}
