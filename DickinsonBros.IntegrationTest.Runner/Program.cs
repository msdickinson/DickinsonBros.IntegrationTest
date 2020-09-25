using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using DickinsonBros.IntegrationTest.Runner.Services;
using DickinsonBros.IntegrationTest.Extensions;
using DickinsonBros.Logger.Extensions;
using DickinsonBros.Guid.Extensions;
using Microsoft.Extensions.Hosting;

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
                var services = InitializeDependencyInjection();
                ConfigureServices(services);

                using (var provider = services.BuildServiceProvider())
                {
                    var integrationTestService = provider.GetRequiredService<IIntegrationTestService>();
                    var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

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

                    hostApplicationLifetime.StopApplication();
                }
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

        private void ConfigureServices(IServiceCollection services)
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
            services.AddSingleton<IHostApplicationLifetime, HostApplicationLifetime>();

            //Stack
            services.AddGuidService();
            services.AddLoggingService();
            services.AddIntegrationTestService();
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

