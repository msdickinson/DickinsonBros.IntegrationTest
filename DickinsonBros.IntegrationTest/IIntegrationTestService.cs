using DickinsonBros.IntegrationTest.Models.TestAutomation;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTest
{
    public interface IIntegrationTestService
    {
        IEnumerable<Test> SetupTests(object testClass);
        Task<TestSummary> RunTests(IEnumerable<Test> tests);
        string GenerateLog(TestSummary testSummary, bool showSuccessLogsOnSuccess);
        string GenerateTRXReport(TestSummary testSummary);
        Task<MemoryStream> GenerateZip(string report, string log);
        Task SaveResultsToDatabase(TestSummary testSummary);
    }
}