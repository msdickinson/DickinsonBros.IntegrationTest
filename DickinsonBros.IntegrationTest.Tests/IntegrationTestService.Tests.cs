using DickinsonBros.IntegrationTest.Models.TestAutomation;
using DickinsonBros.IntegrationTest.Services;
using DickinsonBros.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTest.Tests
{
    [TestClass]
    public class TestAutomationTests : BaseTest
    {
        #region SetupTests

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException), IntegrationTestService.NULL_TEST_CLASS_ERROR_MESSAGE)]
        public async Task SetupTests_NullInput_Throws()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    var nullMixedClass = (MixedClass)null;

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    //Act
                    var observed = uutConcrete.SetupTests(nullMixedClass);

                    //Assert

                    await Task.CompletedTask.ConfigureAwait(false);

                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }


        [TestMethod]
        public async Task SetupTests_ExampleTestClass_NullTestsNameAndTestGroup()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    var exampleTestClass = new ExampleTestClass();

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    //Act
                    var observed = uutConcrete.SetupTests(exampleTestClass);

                    //Assert
                    Assert.AreEqual(2, observed.Count());

                    var observedList = observed.ToList();

                    Assert.AreEqual("Example_MethodOne_Async", observedList[0].MethodInfo.Name);
                    Assert.AreEqual("Example_MethodTwo_Async", observedList[1].MethodInfo.Name);

                    Assert.IsNull(observedList[0].TestsName);
                    Assert.IsNull(observedList[1].TestsName);

                    Assert.IsNull(observedList[0].TestGroup);
                    Assert.IsNull(observedList[1].TestGroup);

                    await Task.CompletedTask.ConfigureAwait(false);

                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task SetupTests_MixedClass_ReturnsExpectedResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    var exampleMixedClass = new MixedClass();
                    var expectedTestNames = "MixedTests";
                    var expectedTestGroup = "TestGroup";

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    //Act
                    var observed = uutConcrete.SetupTests(exampleMixedClass);

                    //Assert
                    Assert.AreEqual(4, observed.Count());

                    var observedList = observed.ToList();

                    Assert.AreEqual("MethodFive_Match", observedList[0].MethodInfo.Name);
                    Assert.AreEqual("MethodSix_Matchs", observedList[1].MethodInfo.Name);
                    Assert.AreEqual("MethodSeven_Matchs", observedList[2].MethodInfo.Name);
                    Assert.AreEqual("MethodEight_Matchs", observedList[3].MethodInfo.Name);
                    
                    Assert.AreEqual(expectedTestNames, observedList[0].TestsName);
                    Assert.AreEqual(expectedTestNames, observedList[1].TestsName);
                    Assert.AreEqual(expectedTestNames, observedList[2].TestsName);
                    Assert.AreEqual(expectedTestNames, observedList[3].TestsName);

                    Assert.AreEqual(expectedTestGroup, observedList[0].TestGroup);
                    Assert.AreEqual(expectedTestGroup, observedList[1].TestGroup);
                    Assert.AreEqual(expectedTestGroup, observedList[2].TestGroup);
                    Assert.AreEqual(expectedTestGroup, observedList[3].TestGroup);

                    await Task.CompletedTask.ConfigureAwait(false);

                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region RunTests

        [TestMethod]
        public async Task RunTests_MethodRuns_ReturnsTestResults()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange​
                    var exampleTestClass = new MixedClass();
                    
                    var expectedMethodInfos = exampleTestClass
                                           .GetType()
                                           .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                           .Where(e => e.DeclaringType.Name == exampleTestClass.GetType().Name);
                    
                    var tests = new List<Models.TestAutomation.Test>
                    {
                        new Models.TestAutomation.Test()
                        {
                            MethodInfo = expectedMethodInfos.First(),
                            TestClass = exampleTestClass
                        },
                        new Models.TestAutomation.Test()
                        {
                            MethodInfo = expectedMethodInfos.Last(),
                            TestClass = exampleTestClass
                        }
                    };
                    
                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;
                    
                    // Act
                    
                    var observed = await uutConcrete.RunTests(tests).ConfigureAwait(false);
                    
                    // Assert​
                    Assert.IsNotNull(observed);
                    Assert.IsNotNull(observed.Duration);
                    Assert.IsNotNull(observed.EndDateTime);
                    Assert.IsNotNull(observed.StartDateTime);
                    Assert.AreEqual(2, observed.TestResults.Count());
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region Process

        [TestMethod]
        public async Task Process_MethodRuns_ReturnsTestResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange

                    var exampleTestClass = new ExampleTestClass();

                    var expectedMethodInfos = exampleTestClass
                                           .GetType()
                                           .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                           .Where
                                           (
                                                methodInfo =>
                                                    methodInfo.GetParameters().Length == 2 &&
                                                    methodInfo.GetParameters()[0].Name == IntegrationTestService.SUCCESSLOG_PRAM_NAME &&
                                                    methodInfo.GetParameters()[0].ParameterType == typeof(List<string>) &&
                                                    methodInfo.GetParameters()[1].Name == IntegrationTestService.CORRELATIONID_EXPECTED_PRAM_NAME &&
                                                    methodInfo.GetParameters()[1].ParameterType == typeof(string) &&
                                                    methodInfo.ReturnType == typeof(Task)
                                           );

                    var test = new Models.TestAutomation.Test()
                    {
                        MethodInfo = expectedMethodInfos.First(),
                        TestClass = exampleTestClass
                    };

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    // Act

                    var observed = await uutConcrete.Process(test.TestClass, test).ConfigureAwait(false);

                    // Assert

                    Assert.IsNotNull(observed);
                    Assert.IsNull(observed.TestGroup);
                    Assert.AreEqual(test.MethodInfo.ReflectedType.Name, observed.ClassName);
                    Assert.IsNotNull(observed.CorrelationId);
                    Assert.IsNotNull(observed.Duration);
                    Assert.IsNotNull(observed.EndTime);
                    Assert.IsNull(observed.Exception);
                    Assert.IsNotNull(observed.ExecutionId);
                    Assert.IsTrue(observed.Pass);
                    Assert.IsNotNull(observed.StartTime);
                    Assert.AreEqual(1, observed.SuccessLog.Count());
                    Assert.IsNotNull(observed.TestId);
                    Assert.IsNotNull(test.MethodInfo.Name, observed.TestName);
                    Assert.IsNotNull(observed.TestType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task Process_MethodRunsWithTestClassAndTestGroup_ReturnsTestResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange

                    var exampleMixedClass = new MixedClass();

                    var expectedMethodInfos = exampleMixedClass
                                           .GetType()
                                           .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                           .Where
                                           (
                                                methodInfo =>
                                                    methodInfo.GetParameters().Length == 2 &&
                                                    methodInfo.GetParameters()[0].Name == IntegrationTestService.SUCCESSLOG_PRAM_NAME &&
                                                    methodInfo.GetParameters()[0].ParameterType == typeof(List<string>) &&
                                                    methodInfo.GetParameters()[1].Name == IntegrationTestService.CORRELATIONID_EXPECTED_PRAM_NAME &&
                                                    methodInfo.GetParameters()[1].ParameterType == typeof(string) &&
                                                    methodInfo.ReturnType == typeof(Task)
                                           );
                    var testAPIAttribute = (TestAPIAttribute)System.Attribute.GetCustomAttributes(exampleMixedClass.GetType()).FirstOrDefault(e => e is TestAPIAttribute);

                    var test = new Models.TestAutomation.Test()
                    {
                        MethodInfo = expectedMethodInfos.First(),
                        TestClass = exampleMixedClass,
                        TestGroup = testAPIAttribute.Group,
                        TestsName = testAPIAttribute.Name
                    };

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    // Act

                    var observed = await uutConcrete.Process(test.TestClass, test).ConfigureAwait(false);

                    // Assert

                    Assert.IsNotNull(observed);
                    Assert.AreEqual(test.MethodInfo.ReflectedType.Name, observed.ClassName);
                    Assert.AreEqual(test.TestGroup, observed.TestGroup);
                    Assert.AreEqual(test.TestsName, observed.TestsName);
                    Assert.IsNotNull(observed.CorrelationId);
                    Assert.IsNotNull(observed.Duration);
                    Assert.IsNotNull(observed.EndTime);
                    Assert.IsNull(observed.Exception);
                    Assert.IsNotNull(observed.ExecutionId);
                    Assert.IsTrue(observed.Pass);
                    Assert.IsNotNull(observed.StartTime);
                    Assert.AreEqual(0, observed.SuccessLog.Count());
                    Assert.IsNotNull(observed.TestId);
                    Assert.IsNotNull(test.MethodInfo.Name, observed.TestName);
                    Assert.IsNotNull(observed.TestType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }


        [TestMethod]
        public async Task Process_MethodThrows_ReturnsTestResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange

                    var exampleTestClass = new ExampleTestClass();

                    var expectedMethodInfos = exampleTestClass
                                           .GetType()
                                           .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                           .Where(e => e.DeclaringType.Name == exampleTestClass.GetType().Name);

                    var test = new Models.TestAutomation.Test()
                    {
                        MethodInfo = expectedMethodInfos.Last(),
                        TestClass = exampleTestClass
                    };

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    // Act

                    var observed = await uutConcrete.Process(test.TestClass, test).ConfigureAwait(false);

                    // Assert

                    Assert.IsNotNull(observed);
                    Assert.AreEqual(test.MethodInfo.ReflectedType.Name, observed.ClassName);
                    Assert.IsNotNull(observed.CorrelationId);
                    Assert.IsNotNull(observed.Duration);
                    Assert.IsNotNull(observed.EndTime);
                    Assert.IsNotNull(observed.Exception);
                    Assert.IsNotNull(observed.ExecutionId);
                    Assert.IsFalse(observed.Pass);
                    Assert.IsNotNull(observed.StartTime);
                    Assert.AreEqual(1, observed.SuccessLog.Count());
                    Assert.IsNotNull(observed.TestId);
                    Assert.IsNotNull(test.MethodInfo.Name, observed.TestName);
                    Assert.IsNotNull(observed.TestType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region GenerateTRXReport

        [TestMethod]
        public async Task GenerateTRXReport_Runs_TrxReportServiceGenerateTRXReportCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange
                    var trxReportServiceMock = serviceProvider.GetMock<ITRXReportService>();
                    var expectedSummary = "";
                    trxReportServiceMock
                    .Setup
                    (
                        trxReportService => trxReportService.GenerateTRXReport
                        (
                            It.IsAny<TestSummary>()
                        )
                    )
                    .Returns
                    (
                        expectedSummary
                    );

                    var testSummary = new TestSummary();

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    // Act

                    var observed = uutConcrete.GenerateTRXReport(testSummary);

                    // Assert
                    trxReportServiceMock
                    .Verify
                    (
                        trxReportService => trxReportService.GenerateTRXReport
                        (
                            testSummary
                        ),
                        Times.Once
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task GenerateTRXReport_Runs_ReturnsStrinFromTrxReportServiceGenerateResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange
                    var trxReportServiceMock = serviceProvider.GetMock<ITRXReportService>();
                    var expectedSummary = "";
                    trxReportServiceMock
                    .Setup
                    (
                        trxReportService => trxReportService.GenerateTRXReport
                        (
                            It.IsAny<TestSummary>()
                        )
                    )
                    .Returns
                    (
                        expectedSummary
                    );

                    var testSummary = new TestSummary();

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    // Act

                    var observed = uutConcrete.GenerateTRXReport(testSummary);

                    // Assert
                    Assert.AreEqual(expectedSummary, observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region GenerateLog

        [TestMethod]
        public async Task GenerateLog_ShowSuccessLogsOnSuccessFalse_ReturnsTestResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange

                    var testSummary = new TestSummary
                    {
                        Duration = TimeSpan.FromMinutes(1),
                        StartDateTime = DateTime.Now.AddMinutes(-1),
                        EndDateTime = DateTime.Now,
                        TestResults = new List<Models.TestAutomation.TestResult>
                        {
                            new Models.TestAutomation.TestResult
                            {
                                TestGroup = "SampleTestGroup",
                                TestsName = "SampleTestsName",
                                ClassName = "SampleClassname",
                                CorrelationId = "SampleCorrelationId",
                                Duration = TimeSpan.FromMinutes(1),
                                StartTime = DateTime.Now,
                                EndTime = DateTime.Now.AddMinutes(-1),
                                Exception = null,
                                ExecutionId = Guid.NewGuid(),
                                Pass = true,
                                SuccessLog = new List<string>{ "Part 1 Successful" },
                                TestId = Guid.NewGuid(),
                                TestName = "SampleTestName1",
                                TestType = Guid.NewGuid()
                            },
                            new Models.TestAutomation.TestResult
                            {
                                TestGroup = "SampleTestGroup",
                                TestsName = "SampleTestsName",
                                ClassName = "SampleClassname",
                                CorrelationId = "SampleCorrelationId",
                                Duration = TimeSpan.FromMinutes(1),
                                StartTime = DateTime.Now,
                                EndTime = DateTime.Now.AddMinutes(-1),
                                Exception = new Exception("SampleException"),
                                ExecutionId = Guid.NewGuid(),
                                Pass = false,
                                SuccessLog = new List<string>{ "Part 1 Successful" },
                                TestId = Guid.NewGuid(),
                                TestName = "SampleTestName2",
                                TestType = Guid.NewGuid()
                            }
                        }
                    };

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    // Act

                    var observed = uutConcrete.GenerateLog(testSummary, false);

                    // Assert

                    Assert.IsNotNull(observed);
                    Assert.AreEqual("SampleTestsName\r\n\r\nPASS  SampleTestName1 - SampleCorrelationId\r\nFAIL  SampleTestName2 - SampleCorrelationId\r\n+ Part 1 Successful\r\n- SampleException\r\n\r\n", observed);
                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task GenerateLog_ShowSuccessLogsOnSuccessTrue_ReturnsTestResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange

                    var testSummary = new TestSummary
                    {
                        Duration = TimeSpan.FromMinutes(1),
                        StartDateTime = DateTime.Now.AddMinutes(-1),
                        EndDateTime = DateTime.Now,
                        TestResults = new List<Models.TestAutomation.TestResult>
                    {
                        new Models.TestAutomation.TestResult
                        {
                            TestGroup = "SampleTestGroup",
                            TestsName = "SampleTestsName",
                            ClassName = "SampleClassname",
                            CorrelationId = "SampleCorrelationId",
                            Duration = TimeSpan.FromMinutes(1),
                            StartTime = DateTime.Now,
                            EndTime = DateTime.Now.AddMinutes(-1),
                            Exception = null,
                            ExecutionId = Guid.NewGuid(),
                            Pass = true,
                            SuccessLog = new List<string> { "Part 1 Successful" },
                            TestId = Guid.NewGuid(),
                            TestName = "SampleTestName1",
                            TestType = Guid.NewGuid()
                        },
                        new Models.TestAutomation.TestResult
                        {
                            TestGroup = "SampleTestGroup",
                            TestsName = "SampleTestsName",
                            ClassName = "SampleClassname",
                            CorrelationId = "SampleCorrelationId",
                            Duration = TimeSpan.FromMinutes(1),
                            StartTime = DateTime.Now,
                            EndTime = DateTime.Now.AddMinutes(-1),
                            Exception = null,
                            ExecutionId = Guid.NewGuid(),
                            Pass = true,
                            SuccessLog = new List<string> { },
                            TestId = Guid.NewGuid(),
                            TestName = "SampleTestName2",
                            TestType = Guid.NewGuid()
                        },
                        new Models.TestAutomation.TestResult
                        {
                            TestGroup = "SampleTestGroup",
                            TestsName = "SampleTestsName",
                            ClassName = "SampleClassname",
                            CorrelationId = "SampleCorrelationId",
                            Duration = TimeSpan.FromMinutes(1),
                            StartTime = DateTime.Now,
                            EndTime = DateTime.Now.AddMinutes(-1),
                            Exception = new Exception("SampleException"),
                            ExecutionId = Guid.NewGuid(),
                            Pass = false,
                            SuccessLog = new List<string> { "Part 1 Successful" },
                            TestId = Guid.NewGuid(),
                            TestName = "SampleTestName3",
                            TestType = Guid.NewGuid()
                        },
                        new Models.TestAutomation.TestResult
                        {
                            TestGroup = "SampleTestGroup",
                            TestsName = "SampleTestsName",
                            ClassName = "SampleClassname",
                            CorrelationId = "SampleCorrelationId",
                            Duration = TimeSpan.FromMinutes(1),
                            StartTime = DateTime.Now,
                            EndTime = DateTime.Now.AddMinutes(-1),
                            Exception = new Exception("SampleException"),
                            ExecutionId = Guid.NewGuid(),
                            Pass = false,
                            SuccessLog = new List<string> { },
                            TestId = Guid.NewGuid(),
                            TestName = "SampleTestName4",
                            TestType = Guid.NewGuid()
                        }

                    }
                    };

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    // Act

                    var observed = uutConcrete.GenerateLog(testSummary, true);

                    // Assert

                    Assert.IsNotNull(observed);
                    Assert.AreEqual("SampleTestsName\r\n\r\nPASS  SampleTestName1 - SampleCorrelationId\r\n+ Part 1 Successful\r\n\r\nPASS  SampleTestName2 - SampleCorrelationId\r\nFAIL  SampleTestName3 - SampleCorrelationId\r\n+ Part 1 Successful\r\n- SampleException\r\n\r\nFAIL  SampleTestName4 - SampleCorrelationId\r\n- SampleException\r\n\r\n", observed);
                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region GenerateZip

        [TestMethod]
        public async Task GenerateZip_Runs_ReturnsStream()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange
                    var report = "SampleReport";
                    var log = "SampleLog";
                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    // Act

                    var observed = await uutConcrete.GenerateZip(report, log).ConfigureAwait(false);

                    // Assert
                    Assert.IsNotNull(observed);

                    using (MemoryStream stream = new MemoryStream(observed.ToArray()))
                    {
                        using (ZipArchive zip = new ZipArchive(stream))
                        {
                            var entryFirst = zip.Entries.First();
                            var entryLast = zip.Entries.Last();

                            using (StreamReader sr = new StreamReader(entryFirst.Open()))
                            {
                                var fileresults = sr.ReadToEnd();
                                Assert.AreEqual(report, fileresults);
                            };

                            using (StreamReader sr = new StreamReader(entryLast.Open()))
                            {
                                var fileresults = sr.ReadToEnd();
                                Assert.AreEqual(log, fileresults);
                            };
                        }
                    }
                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IIntegrationTestService, IntegrationTestService>();
            serviceCollection.AddSingleton(Mock.Of<ITRXReportService>());

            return serviceCollection;
        }

        public interface IExampleTestClass
        {
            Task Example_MethodOne_Async(List<string> successLog, string corrlectionId);
            Task Example_MethodTwo_Async(List<string> successLog, string corrlectionId);
        }
        public class ExampleTestClass : IExampleTestClass
        {
            public async Task Example_MethodOne_Async(List<string> successLog, string correlationId)
            {
                successLog.Add("Step 1 Successful");
                await Task.CompletedTask.ConfigureAwait(false);
            }
            public async Task Example_MethodTwo_Async(List<string> successLog, string correlationId)
            {
                successLog.Add("Step 1 Successful");
                await Task.CompletedTask.ConfigureAwait(false);
                throw new Exception("SampleException");
            }
        }


        [TestAPIAttribute(Name = "MixedTests",Group = "TestGroup")]
        public class MixedClass : InterfaceOne, InterfaceTwo
        {
            internal async Task MethodOne_NotMatch() { await Task.CompletedTask.ConfigureAwait(false); }
            private async Task MethodTwo_NotMatch() { await Task.CompletedTask.ConfigureAwait(false); }
            public void MethodThree_NotMatch() { }
            public void MethodFour_NotMatch(List<string> successLog, string correlationId) { }
            public async Task MethodFive_Match(List<string> successLog, string correlationId) { await Task.CompletedTask.ConfigureAwait(false); }

            public async Task MethodSix_Matchs(List<string> successLog, string correlationId)
            {
                await Task.CompletedTask.ConfigureAwait(false);
            }

            async Task InterfaceTwo.MethodSeven_Matchs(List<string> successLog, string correlationId)
            {
                await Task.CompletedTask.ConfigureAwait(false);
            }

            async Task InterfaceThree.MethodEight_Matchs(List<string> successLog, string correlationId)
            {
                await Task.CompletedTask.ConfigureAwait(false);
            }
        }

        public interface InterfaceOne
        {
            Task MethodSix_Matchs(List<string> successLog, string correlationId);
        }

        public interface InterfaceTwo : InterfaceThree
        {
            Task MethodSeven_Matchs(List<string> successLog, string correlationId);
        }
        public interface InterfaceThree
        {
            Task MethodEight_Matchs(List<string> successLog, string correlationId);
        }
        #endregion


    }
}
