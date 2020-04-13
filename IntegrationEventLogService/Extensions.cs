using EventBus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace IntegrationEventLogService
{
    public static class Extensions
    {
        public static IServiceCollection AddIntegrationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<Func<DbConnection, List<Type>, IIntegrationEventLogService>>(
                sp => (DbConnection c, List<Type> integrationEventTypes) => new IntegrationEventLogService(c, integrationEventTypes));

            services.AddSingleton<IRabbitMQConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RabbitMQConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = configuration["EventBusConnection"],
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
                {
                    factory.UserName = configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
                {
                    factory.Password = configuration["EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new RabbitMQConnection(factory, logger, retryCount);
            });

            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var subscriptionClientName = configuration["SubscriptionClientName"];

            services.AddSingleton<IEventBus, RabbitMQEventBus>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQConnection>();
                var logger = sp.GetRequiredService<ILogger<RabbitMQEventBus>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsHandler>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new RabbitMQEventBus(rabbitMQPersistentConnection, eventBusSubcriptionsManager, sp, logger, subscriptionClientName, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsHandler, EventBusSubscriptionHandler>();

            return services;
        }

        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost, Action<TContext, IServiceProvider> migrateFunc) where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

                    var retries = 10;
                    var retry = Policy.Handle<SqlException>()
                        .WaitAndRetry(
                            retryCount: retries,
                            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                            onRetry: (exception, timeSpan, retry, ctx) =>
                            {
                                logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}",
                                    nameof(TContext), exception.GetType().Name, exception.Message, retry, retries);
                            });
                    retry.Execute(() => migrateFunc(context, services));

                    logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
                }
            }

            return webHost;
        }
    }
}