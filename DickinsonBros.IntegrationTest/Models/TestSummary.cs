using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.IntegrationTest.Models.TestAutomation
{
    [ExcludeFromCodeCoverage]
    public class TestSummary
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public IEnumerable<TestResult> TestResults { get; set; }
        public string Id { get; set; }

    }
}
