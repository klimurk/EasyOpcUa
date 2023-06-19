using MudBlazor.Services;
using OpcUa.Persistance;
using OpcUa.Client.Applications;
using System.Reflection;
using OpcUa.Applications.Common.Mappings;
using OpcUa.Domain;

namespace OpcUa.Web;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public static IConfiguration Configuration { get; private set; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Application configuration
        //services.ConfigureOptions<AppConfig>();
        services.Configure<AppConfig>(Configuration.GetSection(nameof(AppConfig)));

        // Blazor services
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddMudServices();
        // Common services
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
            cfg.AddProfile(new AssemblyMappingProfile(typeof(OpcUaClient).Assembly));
        });

        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        services.AddClientApplication();
        services.AddPersistence();


    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
        });
    }
}
