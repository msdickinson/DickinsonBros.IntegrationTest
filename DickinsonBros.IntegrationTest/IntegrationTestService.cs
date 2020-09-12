using DickinsonBros.IntegrationTest.Models.TestAutomation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DickinsonBros.IntegrationTest.Services;

namespace DickinsonBros.IntegrationTest
{
    public class IntegrationTestService : IIntegrationTestService
    {
        internal readonly ITRXReportService _trxReportService;
        internal const string NULL_TEST_CLASS_ERROR_MESSAGE = "TestClass Is Null, Please ensure when calling SetupTests that the input has a value";
        internal const string SUCCESSLOG_PRAM_NAME = "successLog";
        internal const string CORRELATIONID_EXPECTED_PRAM_NAME = "correlationId";
        public IntegrationTestService(ITRXReportService trxReportService)
        {
            _trxReportService = trxReportService;
        }    
        public IEnumerable<Test> SetupTests(object testClass)
        {
            if(testClass == null)
            {
                throw (new NullReferenceException(NULL_TEST_CLASS_ERROR_MESSAGE));
            }

            var tests = new List<Test>();

            var testAPIAttribute = (TestAPIAttribute)System.Attribute.GetCustomAttributes(testClass.GetType()).FirstOrDefault(e=> e is TestAPIAttribute);

            var methodInfos = testClass
                 .GetType()
                 .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                 .ToList();     

            foreach (Type interf in testClass.GetType().GetInterfaces())
            {
                foreach (MethodInfo method in interf.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!methodInfos.Any(e => e.Name == method.Name))
                    {
                        methodInfos.Add(method);
                    }
                }
            }

            var filteredList = methodInfos
                .Where
                (
                    methodInfo =>
                    methodInfo.GetParameters().Length == 2 &&
                    methodInfo.GetParameters()[0].Name == SUCCESSLOG_PRAM_NAME &&
                    methodInfo.GetParameters()[0].ParameterType == typeof(List<string>) &&
                    methodInfo.GetParameters()[1].Name == CORRELATIONID_EXPECTED_PRAM_NAME &&
                    methodInfo.GetParameters()[1].ParameterType == typeof(string) &&
                    methodInfo.ReturnType == typeof(Task)
                )
                .ToList();

            return filteredList.Select(method => new Test
            {
                MethodInfo = method,
                TestClass = testClass,
                TestsName = testAPIAttribute?.Name,
                TestGroup = testAPIAttribute?.Group,
            }).AsEnumerable();
        }

        public async Task<TestSummary> RunTests(IEnumerable<Test> tests)
        {
            var testSummary = new TestSummary();
            var tasks = new List<Task<TestResult>>();

            foreach (var test in tests)
            {
                tasks.Add(Process(test.TestClass, test));
            };

            testSummary.StartDateTime = DateTime.UtcNow;
            testSummary.TestResults = (await Task.WhenAll(tasks)).ToList();
            testSummary.EndDateTime = DateTime.UtcNow;

            return testSummary;
        }

        internal async Task<TestResult> Process<T>(T tests, Test test)
        {
            var correlationId = Guid.NewGuid().ToString();
            bool pass = false;
            Exception exception = null;
            var successLog = new List<string>();

            DateTime startDateTime = DateTime.Now;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                await (Task)test.MethodInfo.Invoke(tests, new Object[] { successLog, correlationId });
                pass = true;
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                sw.Stop();
            }
            DateTime endDateTime = DateTime.Now;
            return new TestResult
            {
                ClassName = test.MethodInfo.ReflectedType.Name,
                TestsName = test.TestsName != null ? test.TestsName : test.MethodInfo.ReflectedType.Name,
                TestName = test.MethodInfo.Name,
                TestGroup = test.TestGroup,
                CorrelationId = correlationId,
                Pass = pass,
                Duration = sw.Elapsed,
                Exception = exception,
                SuccessLog = successLog,
                StartTime = startDateTime,
                EndTime = endDateTime,
                ExecutionId = Guid.NewGuid(),
                TestId = Guid.NewGuid(),
                TestType = Guid.NewGuid()

            };
        }   

        public string GenerateLog(TestSummary testSummary, bool showSuccessLogsOnSuccess)
        {
            var logs = new List<string>();

            foreach (var testGroup in testSummary.TestResults.GroupBy(e => e.TestsName))
            {
                logs.Add(testGroup.Key);
                logs.Add("");
                foreach (var result in testGroup.OrderBy(e => e.TestName))
                {
                    var pass = result.Pass ? "PASS" : "FAIL";
                    logs.Add($"{ pass }  { result.TestName } - { result.CorrelationId }");
                    if (
                        result.SuccessLog.Any()                   &&
                        (result.Pass && showSuccessLogsOnSuccess) ||
                        (!result.Pass)
                      )
                    {
                        foreach (var log in result.SuccessLog)
                        {
                            logs.Add($"+ {log}");
                        }
                    }
                    if (!result.Pass)
                    {
                        logs.Add($"- {result.Exception.Message}");
                        logs.Add("");
                    }
                    if (
                           result.SuccessLog.Any()      &&
                           result.Pass                  &&
                           showSuccessLogsOnSuccess
                       )
                    {
                        logs.Add("");
                    }
                }
                logs.Add("");
            }

            return String.Join(Environment.NewLine, logs.ToArray());
        }

        public async Task<MemoryStream> GenerateZip(string report, string log)
        {
            using var ms = new MemoryStream();
            using var archive =
                new System.IO.Compression.ZipArchive(ms, ZipArchiveMode.Create, true);
            byte[] reportBytes = Encoding.ASCII.GetBytes(report);
            byte[] logBytes = Encoding.ASCII.GetBytes(log);

            var zipEntry = archive.CreateEntry("Report.trx",
                CompressionLevel.Fastest);
            using (var zipStream = zipEntry.Open())
            {
                await zipStream.WriteAsync(reportBytes, 0, reportBytes.Length).ConfigureAwait(false);
            }

            var zipEntry2 = archive.CreateEntry("log.txt",
                CompressionLevel.Fastest);
            using (var zipStream = zipEntry2.Open())
            {
                await zipStream.WriteAsync(logBytes, 0, logBytes.Length).ConfigureAwait(false);
            }
            return ms;
        }

        public string GenerateTRXReport(TestSummary testSummary)
        {
            return _trxReportService.GenerateTRXReport(testSummary);
        }
    }


}
