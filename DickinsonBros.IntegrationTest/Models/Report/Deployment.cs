using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace DickinsonBros.IntegrationTest.Models.Report
{
    [ExcludeFromCodeCoverage]
    public class Deployment
    {
        [XmlAttribute("runDeploymentRoot")]
        public string RunDeploymentRoot {get; set;}
    }
}
