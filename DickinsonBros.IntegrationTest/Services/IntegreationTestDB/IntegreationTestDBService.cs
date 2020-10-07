using DickinsonBros.IntegrationTest.Models;
using DickinsonBros.IntegrationTest.Models.Db;
using DickinsonBros.SQL.Abstractions;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTest.Services.IntegreationTestDB
{
    public class IntegreationTestDBService : IIntegreationTestDBService
    {
        internal const string TABLE_NAME = "[IntegrationTest].[TestResult]";
        internal readonly ISQLService _sqlService;
        internal readonly IntegrationTestServiceOptions _integrationTestServiceOptions;
        public IntegreationTestDBService
        (
            ISQLService sqlService,
            IOptions<IntegrationTestServiceOptions> options
        )
        {
            _sqlService = sqlService;
            _integrationTestServiceOptions = options.Value;
        }

        public async Task InsertAsync(IEnumerable<TestResult> results)
        {
            await _sqlService.BulkCopyAsync(_integrationTestServiceOptions.ConnectionString, results, TABLE_NAME, null, null, null).ConfigureAwait(false);
        }
    }
}
