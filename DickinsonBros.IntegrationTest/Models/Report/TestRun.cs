using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace DickinsonBros.IntegrationTest.Models.Report
{
    [ExcludeFromCodeCoverage]
    public class TestRun
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; } = "Customer Acquisition Integeration Tests";

        [XmlAttribute("runUser")]
        public string RunUser { get; set; } = "Customer Acquisition Team";

        [XmlElement()]
        public Times Times { get; set; }

        [XmlElement()]
        public TestSettings TestSettings { get; set; }

        [XmlArray()]
        public UnitTestResult[] Results { get; set; }

        [XmlArray()]
        public UnitTest[] TestDefinitions { get; set; }

        [XmlArray()]
        public TestEntry[] TestEntries { get; set; }

        [XmlArray()]
        public TestList[] TestLists { get; set; }

        [XmlElement()]
        public ResultSummary ResultSummary { get; set; }

    }


    //public Times Times { get; set; }
    //public TestSettings Times { get; set; }
    //public IEnumerable<UnitTestResult> UnitTestResult  { get; set; }
    //public IEnumerable<TestDefinitions> TestDefinitions  { get; set; }
    //public TestEntries TestEntries  { get; set; }
    //public TestLists TestLists { get; set; }
    //public ResultSummary ResultSummary  { get; set; }

}

//<TestRun id = "{RUN_ID}" name="{TEST_NAME} {TEST_RUN_DATE_TIME_SIMPLE_FORMAT}" runUser="{RUN_USER}" xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">
//    <Times creation = "{TEST_RUN_START_TIME}" queuing="{TEST_RUN_START_TIME}" start="{TEST_RUN_START_TIME}" finish="{TEST_RUN_FINSH_TIME}" />
//    <TestSettings name = "default" id="{TEST_SETTINGS_GUID}">
//        <Deployment runDeploymentRoot = "" />
//    </ TestSettings >
//    < Results >
//        < UnitTestResult
//            executionId="{UNIT_TEST_EXECUTION_ID}"
//            testId="{UNIT_TEST_ID}"
//            testName="{UNIT_TEST_NAME}"
//            computerName="{COMPUTER_NAME}"
//            duration="{UNIT_TEST_DURATION}"
//            startTime="{UNIT_TEST_START_TIME}"
//            endTime="{UNIT_TEST_END_TIME}"
//            testType="{TEST_TYPE_ID}"
//            outcome="{UNIT_TEST_OUTCOME}"
//            testListId="{TEST_LIST_ID}"
//            relativeResultsDirectory="{UNIT_TEST_EXECUTION_ID}"
//            />
//  </Results>
//  <TestDefinitions>
//    <UnitTest name = "{UNIT_TEST_NAME}" storage="{UNIT_TEST_PATH}" id="{UNIT_TEST_ID}">
//      <Execution id = "{UNIT_TEST_EXECUTION_ID}" />
//      < TestMethod
//        codeBase="{UNIT_TEST_PATH}"
//        adapterTypeName="executor://mstestadapter/v2"
//        className="{UNIT_TEST_CLASS_NAME}"
//        name="{UNIT_TEST_NAME}"
//      />
//    </UnitTest>
//  </TestDefinitions>
//  <TestEntries>
//    <TestEntry
//        executionId = "{UNIT_TEST_EXECUTION_ID}"
//        testId="{UNIT_TEST_ID}"
//        testListId="{TEST_LIST_ID}"
//    />
//  </TestEntries>
//  <TestLists>
//    <TestList name = "Results Not in a List" id="{TEST_LIST_ID}" />
//    <TestList name = "All Loaded Results" id="{TEST_LIST_ID_Two}" />
//  </TestLists>
//  <ResultSummary outcome = "{RESULT_OUTCOME}" >
//    < Counters
//        total="{RESULT_TOTAL}"
//        executed="{RESULT_TOTAL}"
//        passed="{RESULT_PASSED}"
//        failed="{RESULT_FAILED}"
//        error="0"
//        timeout="0"
//        aborted="0"
//        inconclusive="0" 
//        passedButRunAborted="0" 
//        notRunnable="0}" 
//        notExecuted="0" 
//        disconnected="0" 
//        warning="0" 
//        completed="0" 
//        inProgress="0" 
//        pending="0" 
//    />
//  </ResultSummary>
//</TestRun>