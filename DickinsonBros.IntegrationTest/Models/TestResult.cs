using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.IntegrationTest.Models.TestAutomation
{
    [ExcludeFromCodeCoverage]
    public class TestResult
    {
        public string TestName { get; set; }
        public string TestsName { get; set; }
        public string TestGroup { get; set; }
        public bool Pass { get; set; }
        public string CorrelationId { get; set; }
        public Exception Exception { get; set; }
        public List<string> SuccessLog { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public System.Guid ExecutionId { get; set; }
        public System.Guid TestId { get; set; }
        public System.Guid TestType { get; set; }
        public string ClassName { get; set; }
    }
}
