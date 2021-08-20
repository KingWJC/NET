using System;
using static System.Console;

namespace code.sample
{
    public class ObjectsAndTypes
    {
        public static void Test()
        {
            "计算三角形的斜线长度".WriteTemplate();
            var dimension = new Dimensions(10, 10);
            Console.WriteLine(dimension.Diagnoral());

            "Extension Method Invoked".WriteTemplate();
            string fox = "the quick brown fox jumped over the lazy dogs down 987654321 times";
            //编译器改为调用静态方法: StringExtension.GetWordCount(fox);
            int wordCount = fox.GetWordCount();
            WriteLine($"<{fox}> has {wordCount} words");

            "枚举方法,转换和获取项".WriteTemplate();
            DaysOfWeek mondayAndWensday = DaysOfWeek.Monday | DaysOfWeek.Wednesday;
            WriteLine("枚举值:" + mondayAndWensday);
            DaysOfWeek tuesday;
            if (Enum.TryParse<DaysOfWeek>("Tuesday", out tuesday))
            {
                WriteLine("枚举值:" + tuesday);
            }
            Write("GetNames():");
            foreach (string item in Enum.GetNames(typeof(DaysOfWeek)))
            {
                Write(item + " - ");
            }
            WriteLine();
            Write("GetValues():");
            foreach (DaysOfWeek item in (DaysOfWeek[])Enum.GetValues(typeof(DaysOfWeek)))
            {
                Write(item + " - ");
            }

            "calling function".WriteTemplate();
            WriteLine($"static : Pi is {Sample.GetPi()}");
            var math = new Sample();
            math.Value = 30;
            WriteLine($"instance : Square of 30 is {Sample.GetPi()}");

            "out keyword".WriteTemplate();
            OutKeyword();

            "passing by value and reference".WriteTemplate();
            Change(math);
            WriteLine($"math.Value : {math.Value}");

            "静态类与普通类,静态构造函数,属性".WriteTemplate();
            WriteLine($"Sample: day is: {StaticSample.Day}");
        }

        /* 空值判断 类型转换 */
        private static void OutKeyword()
        {
            // 可空类型,三元运算符,空合并运算符
            int? x = 3;
            int x1 = x.HasValue ? x.Value : -1;
            int x2 = x ?? -1;

            string input = ReadLine();
            int result;
            if (Int32.TryParse(input, out result))
            {
                WriteLine($"result :{result}");
            }
            else
            {
                WriteLine("not a number");
            }
        }

        /* 按引用传递 多个引用指向同一直值 */
        private static void Change(Sample sample)
        {
            sample.Value = 10;
        }
    }

    /* 结构 */
    struct Dimensions
    {
        public double Lenght { get; set; }
        public double Width { get; set; }

        public Dimensions(double length, double width)
        {
            this.Lenght = length;
            this.Width = width;
        }

        //a*a+b*b=c*c
        public double Diagnoral() => Math.Sqrt(Lenght * Lenght + Width * Width);
    }

    /* 扩展方法 */
    static class StringExtension
    {
        public static int GetWordCount(this string s) => s.Split().Length;
    }

    /* 枚举 */
    enum DaysOfWeek
    {
        Monday = 0x1,
        Tuesday = 0x2,
        Wednesday = 0x4,
        Thursday = 0x8,
        Friday = 0x10,
        Saturday = 0x20,
        Sunday = 0x40,
        Weekend = Saturday | Sunday,
        Workday = 0x1f,
        AllWeek = Workday | Weekend
    }

    /* 类 */
    class Sample
    {
        public int Value { get; set; }

        /* 只读的静态属性只能在静态构造函数中赋值 */
        public static DaysOfWeek Day { get; set; }

        public Sample()
        {
            Day = DaysOfWeek.Workday;
        }

        static Sample()
        {

        }

        public int GetSquare() => Value * Value;

        public static double GetPi() => 3.141592653;
    }

    /* 静态类 */
    static class StaticSample
    {
        public static DaysOfWeek Day { get; }

        /* 静态构造函数 
         * access modifiers are not allowed on static constructors
        */
        static StaticSample()
        {
            DateTime now = DateTime.Now;
            if (now.DayOfWeek == DayOfWeek.Sunday || now.DayOfWeek == DayOfWeek.Saturday)
            {
                Day = DaysOfWeek.Weekend;
            }
            else
            {
                Day = DaysOfWeek.Workday;
            }
        }
    }
}