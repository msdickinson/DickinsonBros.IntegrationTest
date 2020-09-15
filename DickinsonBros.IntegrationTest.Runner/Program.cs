using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using DickinsonBros.IntegrationTest.Runner.Services;
using DickinsonBros.IntegrationTest.Extensions;
using DickinsonBros.Logger.Extensions;
using DickinsonBros.Guid.Extensions;

namespace DickinsonBros.IntegrationTest.Runner
{
    class Program
    {
        IConfiguration _configuration;
        async static Task Main()
        {
            await new Program().DoMain();
        }
        async Task DoMain()
        {
            try
            {
                using var applicationLifetime = new ApplicationLifetime();
                var services = InitializeDependencyInjection();
                ConfigureServices(services, applicationLifetime);

                using (var provider = services.BuildServiceProvider())
                {
                    var integrationTestService = provider.GetRequiredService<IIntegrationTestService>();

                    var exampleTestClass = new ExampleTestClass();
                    
                    //Setup Tests
                    var tests = integrationTestService.SetupTests(exampleTestClass);

                    //Run Tests
                    var testSummary = await integrationTestService.RunTests(tests).ConfigureAwait(false);

                    //Process Test Summary
                    var trxReport = integrationTestService.GenerateTRXReport(testSummary);
                    var log = integrationTestService.GenerateLog(testSummary, true);

                    //Package Results
                    var zip = integrationTestService.GenerateZip(trxReport, log);

                    //Conole Summary
                    Console.WriteLine("Log:");
                    Console.WriteLine(log);
                    Console.WriteLine();

                    Console.WriteLine("TRX Report:");
                    Console.WriteLine(trxReport);
                    Console.WriteLine();

                    Console.WriteLine($"Zip Generated: {zip != null}");
                    Console.WriteLine();
                }
                applicationLifetime.StopApplication();
                await Task.CompletedTask.ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Console.WriteLine("End...");
                Console.ReadKey();
            }
        }

        private void ConfigureServices(IServiceCollection services, ApplicationLifetime applicationLifetime)
        {
            services.AddOptions();
            services.AddLogging(config =>
            {
                config.AddConfiguration(_configuration.GetSection("Logging"));

                if (Environment.GetEnvironmentVariable("BUILD_CONFIGURATION") == "DEBUG")
                {
                    config.AddConsole();
                }
            });
            services.AddSingleton<IApplicationLifetime>(applicationLifetime);

            //Add IntegrationTest Service
            services.AddIntegrationTestService();

            //Add Logging Service
            services.AddLoggingService();

            //Add Guid Service
            services.AddGuidService();
        }

        IServiceCollection InitializeDependencyInjection()
        {
            var aspnetCoreEnvironment = Environment.GetEnvironmentVariable("BUILD_CONFIGURATION");
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", true);
            _configuration = builder.Build();
            var services = new ServiceCollection();
            services.AddSingleton(_configuration);
            return services;
        }
    }
}

