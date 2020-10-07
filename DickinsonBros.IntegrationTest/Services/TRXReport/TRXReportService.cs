using DickinsonBros.Guid.Abstractions;
using DickinsonBros.IntegrationTest.Models;
using DickinsonBros.IntegrationTest.Models.Report;
using DickinsonBros.IntegrationTest.Models.TestAutomation;
using System.Linq;
using System.Xml.Serialization;

namespace DickinsonBros.IntegrationTest.Services
{
    public class TRXReportService : ITRXReportService
    {
        public const string TEST_RUN_NAME = "Customer Acquisition Integeration Tests";
        public const string TEST_RUN_USER = "Customer Acquisition Team";
        public const string RUN_DEPLOYMENT_ROOT = "";
        public const string DEPLOYMENT_NAME = "default";
        public const string XMLNS = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";
        public const string COMPUTER_NAME = "Azure Function";
        public const string RELATIVE_RESULTS_DIRECTORY = "";
        public const string UNIT_TEST_PATH = "";
        public const string ADAPTER_TYPE_NAME = "executor://mstestadapter/v2";
        public const string CODE_BASE = "";
        public const string TEST_LIST_NAME_RESULTS_NOT_IN_A_LIST = "Results Not in a List";
        public const string TEST_LIST_NAME_ALL_LOADED_TESTS = "All Loaded Results";
        public const string RESULT_OUTCOME = "Completed";

        internal readonly IGuidService _guidService;

        public TRXReportService(IGuidService guidService)
        {
            _guidService = guidService;
        }
        public string GenerateTRXReport
        (
            TestSummary testSummary
        )
        {
            var runId = _guidService.NewGuid().ToString();
            var testSettingsGuid = _guidService.NewGuid().ToString();

            var testLists = new TestList[]
            {
                new TestList
                {
                    Id = _guidService.NewGuid().ToString(),
                    Name = TEST_LIST_NAME_RESULTS_NOT_IN_A_LIST
                },
                new TestList
                {
                    Id = _guidService.NewGuid().ToString(),
                    Name = TEST_LIST_NAME_ALL_LOADED_TESTS
                }
            };

            var testRun = new TestRun
            {
                Id = runId,
                Name = $"{ TEST_RUN_NAME} {testSummary.StartDateTime.ToString(@"yyyy-MM-dd HH:mm:ss")}",
                RunUser = TEST_RUN_USER,
                Times = new Times
                {
                    Creation = testSummary.StartDateTime.ToString(@"yyyy-MM-ddTHH:mm:ss.FFFFFFF+00:00"),
                    Finsh = testSummary.EndDateTime.ToString(@"yyyy-MM-ddTHH:mm:ss.FFFFFFF+00:00"),
                    Queuing = testSummary.StartDateTime.ToString(@"yyyy-MM-ddTHH:mm:ss.FFFFFFF+00:00"),
                    Start = testSummary.StartDateTime.ToString(@"yyyy-MM-ddTHH:mm:ss.FFFFFFF+00:00")
                },
                TestSettings = new TestSettings
                {
                    Deployment = new Deployment
                    {
                        RunDeploymentRoot = RUN_DEPLOYMENT_ROOT
                    },
                    Name = DEPLOYMENT_NAME,
                    Id = testSettingsGuid.ToString()
                },
                Results = testSummary.TestResults.Select(testResult => new UnitTestResult
                {
                    ComputerName = COMPUTER_NAME,
                    Duration = testResult.Duration.ToString(),
                    StartTime = testResult.StartTime.ToString(@"yyyy-MM-ddTHH:mm:ss.FFFFFFF+00:00"),
                    EndTime = testResult.EndTime.ToString(@"yyyy-MM-ddTHH:mm:ss.FFFFFFF+00:00"),
                    ExecutionId = testResult.ExecutionId.ToString(),
                    Outcome = testResult.Pass ? "passed" : "failed",
                    RelativeResultsDirectory = RELATIVE_RESULTS_DIRECTORY,
                    TestId = testResult.TestId.ToString(),
                    TestListId = testLists[0].Id,
                    TestName = testResult.TestName,
                    TestType = testResult.TestType.ToString(),
                }).ToArray(),
                TestDefinitions = testSummary.TestResults.Select(testResult => new UnitTest
                {
                    Execution = new Execution
                    {
                        Id = testResult.ExecutionId.ToString()
                    },
                    Id = testResult.TestId.ToString(),
                    Name = testResult.TestName,
                    Storage = UNIT_TEST_PATH,
                    TestMethod = new TestMethod
                    {
                        AdapterTypeName = ADAPTER_TYPE_NAME,
                        ClassName = testResult.ClassName,
                        CodeBase = CODE_BASE,
                        Name = testResult.TestName
                    }
                }).ToArray(),
                TestEntries = testSummary.TestResults.Select(testResult => new TestEntry
                {
                    ExecutionId = testResult.ExecutionId.ToString(),
                    TestId = testResult.TestId.ToString(),
                    TestListId = testLists[0].Id
                }).ToArray(),
                TestLists = testLists,
                ResultSummary = new ResultSummary
                {
                    Outcome = RESULT_OUTCOME,
                    Counters = new Counters
                    {
                        Total = testSummary.TestResults.Count(),
                        Executed = testSummary.TestResults.Count(),
                        Passed = testSummary.TestResults.Count(testresult => testresult.Pass),
                        Failed = testSummary.TestResults.Count(testresult => !testresult.Pass)
                    }
                }
            };

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer xmlSerializer = new XmlSerializer(testRun.GetType());
            using Utf8StringWriter textWriter = new Utf8StringWriter();
            xmlSerializer.Serialize(textWriter, testRun, ns);
            return textWriter.ToString();
        }
    }
}
