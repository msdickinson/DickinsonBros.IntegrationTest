using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace DickinsonBros.IntegrationTest.Models.Report
{
    [ExcludeFromCodeCoverage]
    public class Execution
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
    }
}
