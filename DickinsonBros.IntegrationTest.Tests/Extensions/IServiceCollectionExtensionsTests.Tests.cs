using DickinsonBros.IntegrationTest.Extensions;
using DickinsonBros.IntegrationTest.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DickinsonBros.IntegrationTest.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddCosmosService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddIntegrationTestService();

            // Assert

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IIntegrationTestService) &&
                                           serviceDefinition.ImplementationType == typeof(IntegrationTestService) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ITRXReportService) &&
                               serviceDefinition.ImplementationType == typeof(TRXReportService) &&
                               serviceDefinition.Lifetime == ServiceLifetime.Singleton));
        }
    }
}
