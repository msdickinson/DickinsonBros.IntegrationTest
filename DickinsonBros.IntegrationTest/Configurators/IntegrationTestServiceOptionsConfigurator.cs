using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.IntegrationTest.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DickinsonBros.IntegrationTest.Configurators
{
    public class IntegrationTestServiceOptionsConfigurator : IConfigureOptions<IntegrationTestServiceOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public IntegrationTestServiceOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        void IConfigureOptions<IntegrationTestServiceOptions>.Configure(IntegrationTestServiceOptions options)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var provider = scope.ServiceProvider;
                var configuration = provider.GetRequiredService<IConfiguration>();
                var configurationEncryptionService = provider.GetRequiredService<IConfigurationEncryptionService>();
                var integrationTestOptions = configuration.GetSection(nameof(IntegrationTestServiceOptions)).Get<IntegrationTestServiceOptions>();

                configuration.Bind($"{nameof(IntegrationTestServiceOptions)}", options);

                options.ConnectionString = configurationEncryptionService.Decrypt(integrationTestOptions.ConnectionString);
            }
        }
    }
}
