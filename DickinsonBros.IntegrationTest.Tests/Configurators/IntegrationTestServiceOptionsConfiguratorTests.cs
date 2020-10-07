using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.IntegrationTest.Configurators;
using DickinsonBros.IntegrationTest.Models;
using DickinsonBros.Test;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace DickinsonBros.Telemetry.Tests.Configurators
{
    [TestClass]
    public class IntegrationTestServiceOptionsConfiguratorTests : BaseTest
    {
        [TestMethod]
        public async Task Configure_Runs_ConfigReturns()
        {
            var integrationTestServiceOptions = new IntegrationTestServiceOptions
            {
                ConnectionString = "SampleConnectionString"
            };

            var integrationTestServiceOptionsDecrypted = new IntegrationTestServiceOptions
            {
                ConnectionString = "SampleDecryptedConnectionString"
            };

            var configurationRoot = BuildConfigurationRoot(integrationTestServiceOptions);

            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var configurationEncryptionServiceMock = serviceProvider.GetMock<IConfigurationEncryptionService>();

                    configurationEncryptionServiceMock
                    .Setup
                    (
                        configurationEncryptionService => configurationEncryptionService.Decrypt
                        (
                            integrationTestServiceOptions.ConnectionString
                        )
                    )
                    .Returns
                    (
                        integrationTestServiceOptionsDecrypted.ConnectionString
                    );

                    //Act
                    var options = serviceProvider.GetRequiredService<IOptions<IntegrationTestServiceOptions>>().Value;

                    //Assert
                    Assert.IsNotNull(options);

                    Assert.AreEqual(integrationTestServiceOptionsDecrypted.ConnectionString, options.ConnectionString);

                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection, configurationRoot)
            );
        }

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddOptions();
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddSingleton<IConfigureOptions<IntegrationTestServiceOptions>, IntegrationTestServiceOptionsConfigurator>();
            serviceCollection.AddSingleton(Mock.Of<IConfigurationEncryptionService>());

            return serviceCollection;
        }

        #endregion
    }
}
