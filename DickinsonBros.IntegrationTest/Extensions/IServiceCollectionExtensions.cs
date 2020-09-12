using DickinsonBros.IntegrationTest.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTest.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddIntegrationTestService(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IIntegrationTestService, IntegrationTestService>();
            serviceCollection.TryAddSingleton<ITRXReportService, TRXReportService>();

            return serviceCollection;
        }
    }
}
