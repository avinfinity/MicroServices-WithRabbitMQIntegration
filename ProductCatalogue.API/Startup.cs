using AutoMapper;
using HealthChecks.UI.Client;
using IntegrationEventLogService;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductCatalogue.API.IntegrationEvents;
using ProductCatalogue.API.PipelineBehaviors;
using ProductCatalogue.API.Queries;
using ProductCatalogue.Infrastructure;
using System.Reflection;

namespace ProductCatalogue.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddFrameworkServices()
                .AddHealthChecks(Configuration,GetType().Namespace)
                .AddDbContext(Configuration)
                .AddIntegrationServices(Configuration)
                .AddEventBus(Configuration)

                .AddMediatR(Assembly.GetExecutingAssembly())
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(PublishEventsBehavior<,>))
                .AddAutoMapper(c => c.AddProfile<AutoMapperProfile>(), this.GetType())
                .AddTransient<IProductCatalogueIntegrationEventService, ProductCatalogueIntegrationEventService>()
                .AddTransient<IProductCatalogueQuery, ProductCatalogueQuery>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }
    }
}