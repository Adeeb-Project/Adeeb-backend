using System;

namespace AdeebBackend.Extensions;

using adeeb.Data;
using adeeb.Models;
using AdeebBackend.Services;
using Microsoft.EntityFrameworkCore;



public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(configuration.GetConnectionString("MySqlConnection"),
                             new MySqlServerVersion(new Version(8, 0, 30))));

        services.Configure<AwsSettings>(configuration.GetSection("AWS"));

        services.AddHttpClient();

        services.AddTransient<S3Service>();

        services.AddScoped<JwtService>();

        //custom services add here guys:
        services.AddScoped<TwilioEmailService>();
        services.AddScoped<SurveysService>();

        return services;
    }

    public static IServiceCollection AddControllersWithCustomJson(this IServiceCollection services)
    {
        services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });

        services.AddEndpointsApiExplorer();

        return services;
    }
}
