using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace DickinsonBros.IntegrationTest.Models.Report
{
    [ExcludeFromCodeCoverage]
    public class TestList
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }
    }
}
