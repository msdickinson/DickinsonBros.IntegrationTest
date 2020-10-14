# DickinsonBros.IntegrationTest
<a href="https://dev.azure.com/marksamdickinson/dickinsonbros/_build/latest?definitionId=74&amp;branchName=master"> <img alt="Azure DevOps builds (branch)" src="https://img.shields.io/azure-devops/build/marksamdickinson/DickinsonBros/74/master"> </a> <a href="https://dev.azure.com/marksamdickinson/dickinsonbros/_build/latest?definitionId=74&amp;branchName=master"> <img alt="Azure DevOps coverage (branch)" src="https://img.shields.io/azure-devops/coverage/marksamdickinson/dickinsonbros/74/master"> </a><a href="https://dev.azure.com/marksamdickinson/DickinsonBros/_release?_a=releases&view=mine&definitionId=35"> <img alt="Azure DevOps releases" src="https://img.shields.io/azure-devops/release/marksamdickinson/b5a46403-83bb-4d18-987f-81b0483ef43e/35/36"> </a><a href="https://www.nuget.org/packages/DickinsonBros.IntegrationTest/"><img src="https://img.shields.io/nuget/v/DickinsonBros.IntegrationTest"></a>

A IntegrationTest service

Features
* SetupTests, RunTests, GenerateLog, GenerateTRXReport, GenerateZip and SaveResultsToDatabase
* A simple method runner using reflection

<h2>Example Usage</h2>

```C#
  var integrationTestService = provider.GetRequiredService<IIntegrationTestService>();

  var exampleTestClass = new ExampleTestClass();

  //Setup Tests
  var tests = integrationTestService.SetupTests(exampleTestClass);

  //Run Tests
  var testSummary = await integrationTestService.RunTests(tests).ConfigureAwait(false);

  //Process Test Summary
  var trxReport = integrationTestService.GenerateTRXReport(testSummary);
  var log = integrationTestService.GenerateLog(testSummary, true);

  //Package Results
  var zip = integrationTestService.GenerateZip(trxReport, log);

  //Conole Summary
  Console.WriteLine("Log:");
  Console.WriteLine(log);
  Console.WriteLine();

  Console.WriteLine("TRX Report:");
  Console.WriteLine(trxReport);
  Console.WriteLine();

  Console.WriteLine($"Zip Generated: {zip != null}");
  Console.WriteLine();
```

```
Log:
ExampleTestClass

PASS  Example_MethodOne_Async - c55875ff-0816-4975-b5bc-f74c18185db2
+ Step 1 Successful

FAIL  Example_MethodTwo_Async - ff01dce3-335a-499c-9fd8-7f9277897fb4
+ Step 1 Successful
- SampleException



TRX Report:
<?xml version="1.0" encoding="utf-8"?>
<TestRun id="6d48360c-90e8-42bc-8025-f6bf90c9e94f" name="Customer Acquisition Integeration Tests 2020-09-12 22:25:36" runUser="Customer Acquisition Team">
  <Times creation="2020-09-12T22:25:36.849483+00:00" queuing="2020-09-12T22:25:36.849483+00:00" start="2020-09-12T22:25:36.849483+00:00" finsh="2020-09-12T22:25:36.8509633+00:00" />
  <TestSettings name="default" id="ed2aa645-14be-409d-be85-6088d2cbaacf">
    <Deployment runDeploymentRoot="" />
  </TestSettings>
  <Results>
    <UnitTestResult executionId="96b9d297-dc4e-47ca-9a90-4f2f4aab0a57" testId="0e595d2a-6c01-4e4b-aebc-b421bc4b00f3" testName="Example_MethodOne_Async" computerName="Azure Function" duration="00:00:00.0007224" startTime="2020-09-12T15:25:36.750145+00:00" endTime="2020-09-12T15:25:36.7519719+00:00" testType="414f057f-ea94-4a5b-84cd-646e85fc804f" outcome="passed" testListId="5c160071-fd95-4289-bab6-90f48f3802aa" relativeResultsDirectory="" />
    <UnitTestResult executionId="02c06e49-030f-4d19-9aa6-11aa21abd769" testId="03fd9608-1e1b-461a-8ec9-063a8f0f9f64" testName="Example_MethodTwo_Async" computerName="Azure Function" duration="00:00:00.0963658" startTime="2020-09-12T15:25:36.7530313+00:00" endTime="2020-09-12T15:25:36.8494005+00:00" testType="9f89c091-1ccd-4e10-a214-65f016cdb6f1" outcome="failed" testListId="5c160071-fd95-4289-bab6-90f48f3802aa" relativeResultsDirectory="" />
  </Results>
  <TestDefinitions>
    <UnitTest name="Example_MethodOne_Async" storage="" id="0e595d2a-6c01-4e4b-aebc-b421bc4b00f3">
      <Execution id="96b9d297-dc4e-47ca-9a90-4f2f4aab0a57" />
      <TestMethod codeBase="" adapterTypeName="executor://mstestadapter/v2" className="ExampleTestClass" name="Example_MethodOne_Async" />
    </UnitTest>
    <UnitTest name="Example_MethodTwo_Async" storage="" id="03fd9608-1e1b-461a-8ec9-063a8f0f9f64">
      <Execution id="02c06e49-030f-4d19-9aa6-11aa21abd769" />
      <TestMethod codeBase="" adapterTypeName="executor://mstestadapter/v2" className="ExampleTestClass" name="Example_MethodTwo_Async" />
    </UnitTest>
  </TestDefinitions>
  <TestEntries>
    <TestEntry executionId="96b9d297-dc4e-47ca-9a90-4f2f4aab0a57" testId="0e595d2a-6c01-4e4b-aebc-b421bc4b00f3" testListId="5c160071-fd95-4289-bab6-90f48f3802aa" />
    <TestEntry executionId="02c06e49-030f-4d19-9aa6-11aa21abd769" testId="03fd9608-1e1b-461a-8ec9-063a8f0f9f64" testListId="5c160071-fd95-4289-bab6-90f48f3802aa" />
  </TestEntries>
  <TestLists>
    <TestList name="Results Not in a List" id="5c160071-fd95-4289-bab6-90f48f3802aa" />
    <TestList name="All Loaded Results" id="161d8b36-561c-4311-baca-1939b904a1b7" />
  </TestLists>
  <ResultSummary outcome="Completed">
    <Counters total="2" executed="2" passed="1" failed="1" error="0" timeout="0" aborted="0" inconclusive="0" passedButRunAborted="0" notRunnable="0" notExecuted="0" disconnected="0" warning="0" completed="0" inProgress="0" pending="0" />
  </ResultSummary>
</TestRun>

Zip Generated: True
```

[Sample Runner](https://github.com/msdickinson/DickinsonBros.IntegrationTest/tree/master/DickinsonBros.IntegrationTest.Runner)
