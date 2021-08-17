using static System.Console;

namespace code
{
    public class OperatorsAndCast
    {
        public static void Test()
        {
            "用户定义的类型转换".WriteTemplate();
            try
            {
                var balance = new Currency(50, 35);
                WriteLine($"balance is {balance}");

                float balance2 = balance;
                WriteLine($"After converting to float = {balance2}");

                balance = (Currency)balance2;
                WriteLine($"After converting to Currency = {balance}");

                WriteLine("Now attempt to convert out of range value of " +
                                    "-$50.50 to a Currency:");
                // Overflow Exception
                checked
                {
                   balance = (Currency)(-50.50);
                   WriteLine($"Result is {balance}");
                }

                uint balance3 = (uint)balance2;
                WriteLine($"Converting to uint gives {balance3}");
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }

    /* 
     * 用户定义的类型转换
     * 如果在进行要求的数据类型转换时没有可用的直接强制转换方式，
     * 编译器会自动寻找转换方式，把几种实现的强制转换合并。
     */
    struct Currency
    {
        public uint Dollars { get; }
        public ushort Cents { get; }

        public Currency(uint dollars, ushort cents)
        {
            Dollars = dollars;
            Cents = cents;
        }

        public override string ToString() => $"${Dollars}.[{Cents,-2:00}]";

        /*
         * @description: 隐式转换为float类型
         * @param {*}
         * @return {*}
         */
        public static implicit operator float(Currency obj)
        {
            return obj.Dollars + (obj.Cents / 100.0f);
        }

        /*
         * @description: float类型强制转换为Currency
         * @param {*}
         * @return {*}
         */
        public static explicit operator Currency(float value)
        {
            uint dollars = (uint)value;
            ushort cents = (ushort)((value - dollars) * 100);
            return new Currency(dollars, cents);
        }

        /* uint与Currency结构的转换 */
        public static implicit operator uint(Currency obj) => obj.Dollars;
        public static explicit operator Currency(uint value) => new Currency(value, 0);
    }
}