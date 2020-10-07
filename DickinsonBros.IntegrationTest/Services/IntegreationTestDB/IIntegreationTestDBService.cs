using DickinsonBros.IntegrationTest.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTest.Services.IntegreationTestDB
{
    public interface IIntegreationTestDBService
    {
        Task InsertAsync(IEnumerable<Models.Db.TestResult> results);
    }
}