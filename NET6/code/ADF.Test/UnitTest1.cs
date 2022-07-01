using ADF.Business;
using ADF.DataAccess;
using ADF.Entity;
using ADF.IBusiness;
using ADF.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ADF.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void QueryTest()
        {
            IUserBusiness userBussiness = new UserBusiness();
            User user = userBussiness.GetUser(695);
            Assert.IsNotNull(user);
        }

        [Test]
        public void InsertTest()
        {
            IUserBusiness userBusiness = new UserBusiness();
            User user = userBusiness.GetUser(695);
            user.Id = 1;
            user.LoginName = "test1";
            userBusiness.AddData(user);
            Assert.IsNotNull(user);
        }

        [Test]
        public void UpdateTest()
        {
            IUserBusiness userBusiness = new UserBusiness();
            User user = userBusiness.GetUser(1);
            user.LoginName = "test2";
            userBusiness.UpdateData(user);
            Assert.IsNotNull(user);
        }

        [Test]
        public void DeleteTest()
        {
            IUserBusiness userBusiness = new UserBusiness();
            userBusiness.DeleteData(new List<int> { 1 });
        }

        /// <summary>
        /// ��������
        /// </summary>
        [Test]
        public void ExpressionTest()
        {
            SqlVisitor visitor = new SqlVisitor();
            Expression<Func<User, bool>> expression = u => (u.Id > 10 && u.Id < 700) || u.Id == 1 && u.UserName.StartsWith("E") && u.UserName.EndsWith("n") && u.UserName.Contains("level");
            visitor.Visit(expression);
            string sqlWhere = visitor.GetWhere();
            Console.WriteLine(sqlWhere);
        }

        [Test]
        public void IOCTest()
        {
            IObjectContainer container = new ObjectContainer();
            container.Register<IUserBusiness, UserBusiness>();
            container.Register<ITestServiceA, TestServiceA>();
            container.Register<ITestServiceB, TestServiceB>(paraList: new object[] { 20 });
            container.Register<ITestServiceB, TestServiceBB>("double");

            ITestServiceB testServiceB = container.Resolve<ITestServiceB>();
            testServiceB.Show();
            ITestServiceB testServiceBB = container.Resolve<ITestServiceB>("double");
            testServiceBB.Show();
        }

        [Test]
        public void IOCLiveTest()
        {
            IObjectContainer container = new ObjectContainer();

            {
                container.Register<ITestServiceA, TestServiceA>(lifetimeType: LifetimeType.Transient);
                ITestServiceA a1 = container.Resolve<ITestServiceA>();
                ITestServiceA a2 = container.Resolve<ITestServiceA>();
                Console.WriteLine(object.ReferenceEquals(a1, a2));
            }

            {
                container.Register<ITestServiceA, TestServiceA>(lifetimeType: LifetimeType.Singleton);
                ITestServiceA a1 = container.Resolve<ITestServiceA>();
                ITestServiceA a2 = container.Resolve<ITestServiceA>();
                Console.WriteLine(object.ReferenceEquals(a1, a2));
            }

            {
                container.Register<ITestServiceA, TestServiceA>(lifetimeType: LifetimeType.Scope);
                ITestServiceA a1 = container.Resolve<ITestServiceA>();
                ITestServiceA a2 = container.Resolve<ITestServiceA>();
                Console.WriteLine(object.ReferenceEquals(a1, a2));

                IObjectContainer container1 = container.CloneContainer();
                ITestServiceA a11 = container1.Resolve<ITestServiceA>();
                ITestServiceA a22 = container1.Resolve<ITestServiceA>();
                Console.WriteLine(object.ReferenceEquals(a11, a22));

                Console.WriteLine(object.ReferenceEquals(a1, a11));
                Console.WriteLine(object.ReferenceEquals(a2, a22));
            }

            {
                container.Register<ITestServiceA, TestServiceA>(lifetimeType: LifetimeType.PerThread);
                ITestServiceA a1 = container.Resolve<ITestServiceA>();
                ITestServiceA a2 = container.Resolve<ITestServiceA>();
                ITestServiceA a3 = null;
                ITestServiceA a4 = null;
                ITestServiceA a5 = null;

                Task.Run(() =>
                {
                    Console.WriteLine($"This is {Thread.CurrentThread.ManagedThreadId} a3");
                    a3 = container.Resolve<ITestServiceA>();
                });
                Task.Run(() =>
                {
                    Console.WriteLine($"This is {Thread.CurrentThread.ManagedThreadId} a4");
                    a4 = container.Resolve<ITestServiceA>();
                }).ContinueWith(t =>
                {
                    Console.WriteLine($"This is {Thread.CurrentThread.ManagedThreadId} a5");
                    a5 = container.Resolve<ITestServiceA>();
                });

                Thread.Sleep(1000);

                Console.WriteLine(object.ReferenceEquals(a1, a2));
                Console.WriteLine(object.ReferenceEquals(a1, a3));
                Console.WriteLine(object.ReferenceEquals(a1, a4));
                Console.WriteLine(object.ReferenceEquals(a1, a5));

                Console.WriteLine(object.ReferenceEquals(a3, a4));
                Console.WriteLine(object.ReferenceEquals(a3, a5));

                Console.WriteLine(object.ReferenceEquals(a4, a5));//应该相同，但结果是False
            }
        }

        [Test]
        public void AOPTest()
        {
            IObjectContainer container = new ObjectContainer();
            container.Register<ITestServiceA, TestServiceA>();
            container.Register<ITestServiceB, TestServiceBB>();

            //ITestServiceA testServiceA = container.Resolve<ITestServiceA>();
            //testServiceA.Show();

            ITestServiceB testService = container.Resolve<ITestServiceB>();
            testService.Show();
        }
    }
}