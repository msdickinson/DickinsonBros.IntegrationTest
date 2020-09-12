using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DickinsonBros.IntegrationTest.Models
{
    [ExcludeFromCodeCoverage]
    public class TestResults
    {
        public string Log { get; set; }
        public byte[] TRXFile { get; set; }
    }
}
