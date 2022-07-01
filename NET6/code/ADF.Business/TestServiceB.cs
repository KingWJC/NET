using ADF.Utility;
using ADF.IBusiness;

namespace ADF.Business
{
    public class TestServiceB : ITestServiceB
    {
        [PropertyInjection]
        public IUserBusiness userBusiness { get; set; }

        private ITestServiceA _iTestServiceA = null;
        [Constructor]
        public TestServiceB(ITestServiceA testServiceA, [ParamterConstant] int index)
        {
            _iTestServiceA = testServiceA;
            Console.WriteLine($"{this.GetType().Name}被构造 {index}。。。");
        }

        public void Show()
        {
            Console.WriteLine("TestServiceB: A123456");
            _iTestServiceA.Show();

            var user = userBusiness.GetUser(1);
            Console.WriteLine($"userBusiness:{user.LoginName}");
        }
    }
}
