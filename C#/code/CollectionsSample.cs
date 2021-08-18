/*
 * @Author: KingWJC
 * @Date: 2021-08-18 13:48:22
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-08-18 17:44:23
 * @Descripttion: 
 * @FilePath: \code\CollectionsSample.cs
 *
 * 比较时，需要判断比较对象是否为空。
 *
 */
using System;
using System.Collections.Generic;
using static System.Console;

namespace code
{
    public class CollectionsSample
    {
        public static void Test()
        {
            var graham = new Racer("Hill", "UK", 14);
            var emerson = new Racer("Fittipaldi", "Brazil", 14);
            var mario = new Racer("Andretti", "USA", 12);

            //初始化容器大小为20
            var racers = new List<Racer>(20) { graham, emerson, mario };

            racers.Add(new Racer("Schumacher", "Germany", 91));
        }
    }


    /*
     * @description: 比较器
     * @param {*}
     * @return {*}
     */
    class RacerComparator : IComparer<Racer>
    {
        private CompareType compareType;
        public RacerComparator(CompareType type)
        {
            compareType = type;
        }
        public int Compare(Racer a, Racer b)
        {
            if (a == null && b == null) return 0;
            if (a == null) return -1;
            if (b == null) return 1;

            int result = 0;
            switch (compareType)
            {
                case CompareType.Name:
                    if (a.Name.Length != b.Name.Length)
                    {
                        result = a.Name.Length > b.Name.Length ? 1 : -1;
                    }
                    break;
                case CompareType.Country:
                    result = string.Compare(a.Country, b.Country);
                    if (result == 0)
                    {
                        result = string.Compare(a.Name, b.Name);
                    }
                    break;
                case CompareType.Wins:
                    result = a.Wins.CompareTo(b.Wins);
                    break;
                default: throw new ArgumentException("invalid compare type");
            }
            return 0;
        }
    }
    /*
     * @description: 比较方式
     * @param {*}
     * @return {*}
     */
    enum CompareType
    {
        Name = 1,
        Country = 2,
        Wins = 3
    }
    /*
     * @description: 比较类
     * @param {*}
     * @return {*}
     */
    class Racer : IComparable<Racer>
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public int Wins { get; set; }

        public Racer(string name, string country, int wins)
        {
            this.Name = name;
            this.Country = country;
            this.Wins = wins;
        }

        public override string ToString()
        {
            return $"Name:{Name}, Country:{Country}, Wins:{Wins}";
        }

        public int CompareTo(Racer other)
        {
            int result = Name?.CompareTo(other.Name) ?? -1;
            if (result == 0)
            {
                result = Country?.CompareTo(other.Country) ?? -1;
            }
            return result;
        }
    }
    /*
     * @description: 定义筛选条件
     * @param {*}
     * @return {*}
     */
    class FindCountry
    {
        private string country;
        public FindCountry(string country)
        {
            this.country = country;
        }

        public bool FindCountryPredicate(Racer racer) => racer?.Country == country;
    }
}