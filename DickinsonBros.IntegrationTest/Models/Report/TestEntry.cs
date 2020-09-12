using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace DickinsonBros.IntegrationTest.Models.Report
{
    [ExcludeFromCodeCoverage]
    public class TestEntry
    {
        [XmlAttribute("executionId")]
        public string ExecutionId { get; set; }

        [XmlAttribute("testId")]
        public string TestId { get; set; }

        [XmlAttribute("testListId")]
        public string TestListId { get; set; }
    }
}
