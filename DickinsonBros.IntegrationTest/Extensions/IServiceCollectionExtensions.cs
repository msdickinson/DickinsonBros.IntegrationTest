using DickinsonBros.IntegrationTest.Configurators;
using DickinsonBros.IntegrationTest.Models;
using DickinsonBros.IntegrationTest.Services;
using DickinsonBros.IntegrationTest.Services.IntegreationTestDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DickinsonBros.IntegrationTest.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddIntegrationTestService(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IIntegrationTestService, IntegrationTestService>();
            serviceCollection.TryAddSingleton<IIntegreationTestDBService, IntegreationTestDBService>();
            serviceCollection.TryAddSingleton<ITRXReportService, TRXReportService>();
            serviceCollection.TryAddSingleton<IConfigureOptions<IntegrationTestServiceOptions>, IntegrationTestServiceOptionsConfigurator>();

            return serviceCollection;
        }
    }
}
