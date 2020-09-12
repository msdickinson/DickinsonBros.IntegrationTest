using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace DickinsonBros.IntegrationTest.Models.Report
{
    [ExcludeFromCodeCoverage]
    public class UnitTestResult
    {
        [XmlAttribute("executionId")]
        public string ExecutionId { get; set; }

        [XmlAttribute("testId")]
        public string TestId { get; set; }

        [XmlAttribute("testName")]
        public string TestName { get; set; }

        [XmlAttribute("computerName")]
        public string ComputerName { get; set; }

        [XmlAttribute("duration")]
        public string Duration { get; set; }

        [XmlAttribute("startTime")]
        public string StartTime { get; set; }

        [XmlAttribute("endTime")]
        public string EndTime { get; set; }

        [XmlAttribute("testType")]
        public string TestType { get; set; }

        [XmlAttribute("outcome")]
        public string Outcome { get; set; }

        [XmlAttribute("testListId")]
        public string TestListId { get; set; }

        [XmlAttribute("relativeResultsDirectory")]
        public string RelativeResultsDirectory { get; set; }
    }
}
