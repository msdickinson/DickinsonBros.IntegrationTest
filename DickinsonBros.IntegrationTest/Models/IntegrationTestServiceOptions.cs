using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.IntegrationTest.Models
{
    [ExcludeFromCodeCoverage]
    public class IntegrationTestServiceOptions
    {
        public string ConnectionString { get; set; }
    }
}
