using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using OpcUa.Web.Data;
using Serilog;

namespace OpcUa.Web;
public class Program
{
    private static IHost _Host;
    private static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json")
            .Build();
        var logConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration);
        var logger = logConfig.CreateLogger();
        Log.Logger = logger;
        try
        {
            //Log.Information("Application build services");
            _Host = CreateHostBuilder(args).Build();

            Log.Information("Application starting");
            _Host.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application failed");
        }
        finally
        {
            Log.Information("Exit Application");
            Log.CloseAndFlush();
        }

    }
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults((webBuilder) =>
        {
            webBuilder.ConfigureAppConfiguration((ctx, cfg) =>
            {
                cfg.SetBasePath(Environment.CurrentDirectory);
                cfg.AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json", false, true);
            });
            webBuilder.UseStartup<Startup>();
        })
        .UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));
}
