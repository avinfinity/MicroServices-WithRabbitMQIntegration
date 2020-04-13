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
using Pricing.API.Events;
using Pricing.API.IntegrationEvents;
using Pricing.API.PipelineBehaviors;
using Pricing.Domain;
using Pricing.Infrastructure;
using System.Reflection;

namespace Pricing.API
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
                .AddHealthChecks(Configuration, GetType().Namespace)
                .AddDbContext(Configuration)
                .AddIntegrationServices(Configuration)
                .AddEventBus(Configuration)

                .AddMediatR(Assembly.GetExecutingAssembly())
                .AddAutoMapper(c => c.AddProfile<AutoMapperProfile>(), this.GetType())

                .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddTransient<IProductPriceQuery, ProductPriceQuery>()
                .AddTransient<ProductAddedIntegrationEventHandler>()
                .AddTransient<ProductRemovedIntegrationEventHandler>();
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

            ConfigureEventBus(app);
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<ProductAddedIntegrationEvent, ProductAddedIntegrationEventHandler>();
            eventBus.Subscribe<ProductRemovedIntegrationEvent, ProductRemovedIntegrationEventHandler>();
        }
    }
}