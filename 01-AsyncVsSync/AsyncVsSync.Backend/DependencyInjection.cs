using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AsyncVsSync.Backend;

public static class DependencyInjection
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder, ILogger logger)
    {
        builder.Host.UseSerilog(logger);
        builder.Services
               .AddSingleton(new ThreadPoolWatcher())
               .AddHealthChecks();
        return builder;
    }
}