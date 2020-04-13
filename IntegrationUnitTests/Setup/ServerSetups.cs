using AutoMapper;
using IntegrationEventLogService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace IntegrationUnitTests
{
    public static class ServerSetups
    {
        public static TestServer CreateServer<TDbContext, TStartup, TDTO, TEntity>(string testDataFilePath, bool migrateIntegrationDb = false) 
            where TDbContext : DbContext 
            where TStartup : class
        {
            TestServer testServer = CreateTestServer<TStartup>();

            MigrateDbContextx<TDbContext>(migrateIntegrationDb, testServer);

            SeedTestData<TDbContext, TDTO, TEntity>(testDataFilePath, testServer);

            return testServer;
        }

        private static void SeedTestData<TDbContext, TDTO, TEntity>(string testDataFilePath, TestServer testServer) where TDbContext : DbContext
        {
            using (var scope = testServer.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<TDbContext>();

                var mapper = scopedServices.GetRequiredService<IMapper>();
                // Ensure the database is created.
                db.Database.EnsureCreated();

                var entities = JsonConvert.DeserializeObject<IEnumerable<TDTO>>(File.ReadAllText(testDataFilePath));

                foreach (var entity in entities)
                {
                    var product = mapper.Map<TEntity>(entity);
                    db.Add(mapper.Map<TEntity>(entity));
                }

                db.SaveChanges();
            }
        }

        private static TestServer CreateTestServer<TStartup>() where TStartup : class
        {
            var path = Assembly.GetAssembly(typeof(ServerSetups))
                          .Location;

            var config = new ConfigurationBuilder().AddJsonFile("testsettings.json").Build();

            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path))
                .ConfigureAppConfiguration((config) =>
                {
                    config.AddJsonFile("testsettings.json");
                })
                .UseStartup<TStartup>();

            var testServer = new TestServer(hostBuilder);
            return testServer;
        }

        private static void MigrateDbContextx<TDbContext>(bool migrateIntegrationDb, TestServer testServer) where TDbContext : DbContext
        {
            testServer.Host
                .MigrateDbContext<TDbContext>((context, services) =>
                {
                    context.Database.Migrate();
                });

            if (migrateIntegrationDb)
            {
                testServer.Host
                .MigrateDbContext<IntegrationLogDbContext>((context, services) =>
                {
                    context.Database.Migrate();
                });
            }
        }
    }
}