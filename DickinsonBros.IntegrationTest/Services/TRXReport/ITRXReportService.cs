using DickinsonBros.IntegrationTest.Models.TestAutomation;

namespace DickinsonBros.IntegrationTest.Services
{
    public interface ITRXReportService
    {
        string GenerateTRXReport(TestSummary testSummary);
    }
}