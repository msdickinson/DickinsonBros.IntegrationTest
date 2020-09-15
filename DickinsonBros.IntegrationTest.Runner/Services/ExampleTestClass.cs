using DickinsonBros.IntegrationTest.Models.TestAutomation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTest.Runner.Services
{
    [TestAPIAttribute(Name = "ExampleTestClass", Group = "TestGroupOne")]
    public class ExampleTestClass : IExampleTestClass
    {
        public async Task Example_MethodOne_Async(List<string> successLog)
        {
            successLog.Add("Step 1 Successful");
            await Task.CompletedTask.ConfigureAwait(false);
        }
        public async Task Example_MethodTwo_Async(List<string> successLog)
        {
            successLog.Add("Step 1 Successful");
            await Task.CompletedTask.ConfigureAwait(false);
            throw new Exception("SampleException");
        }
    }



}
