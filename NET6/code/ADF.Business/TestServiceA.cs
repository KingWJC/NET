using ADF.Utility;
using ADF.IBusiness;

namespace ADF.Business
{
    public class TestServiceA : ITestServiceA
    {
        [LogBefore]
        [LogAfter]
        public void Show()
        {
            Console.WriteLine("TestServiceA: A123456");
        }
    }
}
