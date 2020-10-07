using DickinsonBros.IntegrationTest.Services.IntegreationTestDB;
using DickinsonBros.SQL.Abstractions;
using DickinsonBros.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTest.Tests.Services.IntegreationTestDB
{
    [TestClass]
    public class IntegreationTestDBServiceTests : BaseTest
    {
        #region InsertAsync

        [TestMethod]
        public async Task InsertAsync_Runs_BulkCopyAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange

                    var testResult = new List<Models.Db.TestResult>();

                    //-- ISQLService
                    var sqlServiceMock = serviceProvider.GetMock<ISQLService>();

                    sqlServiceMock
                    .Setup
                    (
                        sqlService => sqlService.BulkCopyAsync
                        (
                            It.IsAny<string>(),
                            It.IsAny<IEnumerable<Models.Db.TestResult>>(),
                            It.IsAny<string>(),
                            It.IsAny<int?>(),
                            It.IsAny<TimeSpan?>(),
                            It.IsAny<CancellationToken?>()
                        )
                    );

                    var uut = serviceProvider.GetRequiredService<IIntegreationTestDBService>();
                    var uutConcrete = (IntegreationTestDBService)uut;

                    // Act

                    await uutConcrete.InsertAsync(testResult).ConfigureAwait(false);

                    // Assert
                    sqlServiceMock
                   .Verify
                   (
                       sqlService => sqlService.BulkCopyAsync
                       (
                           uutConcrete._integrationTestServiceOptions.ConnectionString,
                           testResult,
                           IntegreationTestDBService.TABLE_NAME,
                           null,
                           null,
                           null
                       ),
                       Times.Once
                   );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IIntegreationTestDBService, IntegreationTestDBService>();
            serviceCollection.AddSingleton(Mock.Of<ISQLService>());
            return serviceCollection;
        }

        #endregion

    }
}
