using System;
using System.Linq;
using System.Collections.Generic;
using static System.Console;

namespace code
{
    public class OperatorsAndCast
    {
        public static void Test()
        {
            "运算符重载".WriteTemplate();
            OperatorOverloading();

            "用户定义的类型转换".WriteTemplate();
            CastSample();

            "自定义的索引运算符".WriteTemplate();
            IndexSample();
        }

        private static void OperatorOverloading()
        {
            Vector vect1, vect2, vect3;

            vect1 = new Vector(3.0, 3.0);
            vect2 = new Vector(2.0, -4.0);
            vect3 = vect1 + vect2;

            WriteLine($"vect1 = {vect1}");
            WriteLine($"vect2 = {vect2}");
            WriteLine($"vect3 = vect1 + vect2 = {vect3}");
            WriteLine($"2 * vect3 = {2 * vect3}");
            WriteLine($"vect3 += vect2 gives {vect3 += vect2}");
            WriteLine($"vect3 = vect1 * 2 gives {vect3 = vect1 * 2}");
            WriteLine($"vect1 * vect3 = {vect1 * vect3}");

            WriteLine($"vect1 == vect2 returns {(vect1 == vect2)}");
            WriteLine($"vect2 != vect3 returns {(vect2 != vect3)}");

            var vect4 = new Vector(5.0, 2.0);
            var vect5 = new Vector(2.0, 5.0);
            WriteLine(vect4.GetHashCode());
            WriteLine(vect5.GetHashCode());
        }

        private static void CastSample()
        {
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
                // balance = (Currency)(-50.50);
                // WriteLine($"Result is {balance}");

                // 多重类型转换, 若没有uint转换的方法,则相当于:
                // uint balance3 = (uint)(float)balance2;
                uint balance3 = (uint)balance2;
                WriteLine($"Converting to uint gives {balance3}");
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private static void IndexSample()
        {
            var p1 = new People("Ayrton", new DateTime(1960, 3, 21));
            var p2 = new People("Ronnie", new DateTime(1944, 2, 14));
            var p3 = new People("Jochen", new DateTime(1942, 4, 18));
            var p4 = new People("Francois", new DateTime(1944, 2, 25));
            var coll = new PeopleCollection(p1, p2, p3, p4);

            WriteLine(coll[2]);
            foreach (var r in coll[new DateTime(1960, 3, 21)])
            {
                WriteLine(r);
            }
        }
    }

    /* 运算符重载 */
    class Vector : IEquatable<Vector>
    {
        public double X { get; }
        public double Y { get; }

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"X:{X}, Y:{Y}";

        public override int GetHashCode() => X.GetHashCode() + (Y.GetHashCode() << 4);
        /* 重写==运算符必须重写GetHashCode,Equals */
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this == (Vector)obj;
        }

        /* IEquatable接口 */
        public bool Equals(Vector other) => this == other;

        public static Vector operator +(Vector left, Vector right) => new Vector(left.X + right.X, left.Y + right.Y);
        //重载
        public static Vector operator *(Vector left, double right) => new Vector(left.X * right, left.Y * right);
        public static Vector operator *(double left, Vector right) => right * left;
        //返回其它类型
        public static Double operator *(Vector left, Vector right) => left.X * right.X + left.Y * right.Y;

        public static bool operator ==(Vector left, Vector right)
        {

            if (Object.ReferenceEquals(left, right)) return true;

            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(Vector left, Vector right) => !(left == right);
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
            checked
            {
                // 无checked, 则不报溢出异常
                uint dollars = (uint)value;
                ushort cents = Convert.ToUInt16((value - dollars) * 100);
                return new Currency(dollars, cents);
            }
        }

        /* uint与Currency结构的转换 */
        public static implicit operator uint(Currency obj) => obj.Dollars;
        public static explicit operator Currency(uint value) => new Currency(value, 0);
    }

    /* 自定义的索引运算符 */
    class PeopleCollection
    {
        private People[] _people;

        public PeopleCollection(params People[] parameters)
        {
            _people = parameters;
        }

        public People this[int index]
        {
            get { return _people[index]; }
            set { _people[index] = value; }
        }

        public IEnumerable<People> this[DateTime birthDay] => _people.Where(p => p.BirthDay == birthDay);
    }

    class People
    {
        public DateTime BirthDay { get; }
        public string Name { get; }
        public People(string name, DateTime dateTime)
        {
            BirthDay = dateTime;
            Name = name;
        }
        public override string ToString() => $"Name:{Name}, BirtyDay:{BirthDay}";
    }
}