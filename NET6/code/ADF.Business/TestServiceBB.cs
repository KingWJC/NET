using ADF.Utility;
using ADF.IBusiness;

namespace ADF.Business
{
    public class TestServiceBB : ITestServiceB
    {
        public TestServiceBB()
        {
            Console.WriteLine($"{this.GetType().Name}被构造。。。");
        }

        private ITestServiceA _iTestServiceA = null;
        [MethodInjection]
        public void Init(ITestServiceA testServiceA)
        {
            this._iTestServiceA = testServiceA;
        }

        [Login]
        [Monitor("执行时间")]
        public void Show()
        {
            Console.WriteLine("TestServiceBB: A123456");
            _iTestServiceA?.Show();
            Thread.Sleep(1000);
        }
    }
}
