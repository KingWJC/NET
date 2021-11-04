/*
 * @Author: KingWJC
 * @Date: 2021-09-06 17:36:56
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-11-03 15:22:00
 * @Descripttion: 
 * @FilePath: \code\sample\VectorClass.cs
 *
 * 公共测试类，实现了集合的功能，以及自定义迭代器
 * 包括ToString，HashCode,Equals的重写，运算符的重载，以及IFomattable和IEnumable接口的实现。
 * 反射中特性的增加
 */
using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using static System.Console;
namespace code.sample
{

    [LastModified("6 Jun 2015", "updated for C# 6 and .NET Core")]
    [LastModified("14 Dec 2010", "IEnumerable interface implemented: " +
        "VectorClass can be treated as a collection")]
    public class VectorClass : IFormattable, IEnumerable<double>
    {
        public double X { get; }
        public double Y { get; }
        public double this[uint i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    default:
                        throw new IndexOutOfRangeException("Attempt to retrivee VectorClass element " + i);
                }
            }
        }
        public VectorClass(double x, double y)
        {
            X = x;
            Y = y;
        }
        public VectorClass(VectorClass vector)
        : this(vector.X, vector.Y)
        {

        }

        public double Sum()
        {
            return X + Y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return this == obj as VectorClass;
        }

        public override int GetHashCode() => (int)X | (int)Y;

        /*
         * @description: Epsilon表示大于零的最小正 Double 值
         * @param {*}
         * @return {*}
         */
        public static bool operator ==(VectorClass left, VectorClass right)
        {
            return Math.Abs(left.X - right.X) < double.Epsilon && Math.Abs(left.Y - right.Y) < double.Epsilon;
        }

        public static bool operator !=(VectorClass left, VectorClass right)
        {
            return !(left == right);
        }

        public override string ToString() => $"({X}, {Y})";

        [LastModified("10 Feb 2010", "Methode added in order to provider formatting support")]
        public string ToString(string format, IFormatProvider provider)
        {
            if (format == null)
            {
                return ToString();
            }
            switch (format.ToUpper())
            {
                case "N":
                    return "||" + (X * X + Y * Y).ToString() + "||";
                case "VE":
                    return $"( {X:E}, {Y:E})";
                default:
                    return ToString();
            }
        }

        [LastModified("6 Jun 2015", "added to implement IEnumerable<T>")]
        public IEnumerator<double> GetEnumerator()
        {
            return new VectorErumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [LastModified("6 Jun 2015", "Change to implement the generic version IEnumerator<T>")]
        [LastModified("14 Feb 2010", "Class created as part of collection support for VectorClass")]
        private class VectorErumerator : IEnumerator<double>
        {
            readonly VectorClass _theVector;
            int _location;
            public VectorErumerator(VectorClass vector)
            {
                _theVector = vector;
            }

            public object Current => Current;

            double IEnumerator<double>.Current
            {
                get
                {
                    if (_location < 0 || _location > 1)
                    {
                        throw new InvalidOperationException("The enumerator is either before the first element or after the last element of the VectorClass");
                    }
                    return _theVector[(uint)_location];
                }
            }

            public bool MoveNext()
            {
                ++_location;
                return (_location <= 1);
            }


            public void Reset()
            {
                _location = -1;
            }

            public void Dispose() { }
        }
    }

}