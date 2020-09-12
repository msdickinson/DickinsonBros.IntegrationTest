using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace DickinsonBros.IntegrationTest.Models.Report
{
    [ExcludeFromCodeCoverage]
    public class UnitTest
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("storage")]
        public string Storage { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement()]
        public Execution Execution { get; set; }

        [XmlElement()]
        public TestMethod TestMethod { get; set; }
    }
}
