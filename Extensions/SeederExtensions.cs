using System;

using adeeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace adeeb.Extensions;

public static class SeederExtensions
{
    public static void SeedData(this IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            DummyDbInitializer.Initialize(context);
        }
    }
}
