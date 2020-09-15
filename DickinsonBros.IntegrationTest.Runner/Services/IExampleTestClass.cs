using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTest.Runner.Services
{
    public interface IExampleTestClass
    {
        Task Example_MethodOne_Async(List<string> successLog);
        Task Example_MethodTwo_Async(List<string> successLog);
    }
}