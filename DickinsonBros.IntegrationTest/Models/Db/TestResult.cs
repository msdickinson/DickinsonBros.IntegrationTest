using System;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.IntegrationTest.Models.Db
{
    [ExcludeFromCodeCoverage]
    public class TestResult
    {
        public string TestGroup { get; set; }
        public string TestName { get; set; }
        public bool Pass { get; set; }
        public TimeSpan Duration { get; set; }
        public string CorrelationId { get; set; }
        public string TestRunId { get; set; }
        public DateTime DateTime { get; set; }
    }
}
