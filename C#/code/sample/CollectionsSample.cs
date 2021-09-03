/*
 * @Author: KingWJC
 * @Date: 2021-08-18 13:48:22
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-09-03 17:16:20
 * @Descripttion: 
 * @FilePath: \code\sample\CollectionsSample.cs
 *
 * 比较时，需要判断比较对象是否为空。
 * 迭代器中，modCount记录集合修改的次数，next和remove操作之前都会先调用checkForComodification来检查expectedModCount和modCount是否相等, 用于防止多个迭代器遍历集合时，因为修改而造成数据混乱，保证数据唯一性。
 *
 */
using System;
using System.Linq;
using System.Collections.Generic;
using static System.Console;

namespace code.sample
{
    public class CollectionsSample
    {
        public static void Test()
        {
            "List sample".WriteTemplate();
            ListSample();

            "SortedList<IKey,IValue> sample".WriteTemplate();
            SortedListSample();

            "Queue sample FIFO".WriteTemplate();
            QueueSample();

            "Stack sample LIFO".WriteTemplate();
            StackSample();

            "Linklist sample".WriteTemplate();
            LinkedListSample();

            "Set sample".WriteTemplate();
            SetSample();

            "Dictionary sample".WriteTemplate();
            DictionarySample();
        }

        public static void SetSample()
        {
            var companyTeams = new HashSet<string>() { "Ferrari", "McLaren", "Mercedes" };
            var traditionalTeams = new HashSet<string>() { "Ferrari", "McLaren" };
            var privateTeams = new HashSet<string>() { "Red Bull", "Lotus", "Toro Rosso", "Force India", "Sauber" };

            if (privateTeams.Add("Williams"))
                WriteLine("Williams added");
            if (!companyTeams.Add("McLaren"))
                WriteLine("McLaren was already in this set");

            if (traditionalTeams.IsSubsetOf(companyTeams))
            {
                WriteLine("子集: traditionalTeams is subset of companyTeams");
            }
            if (companyTeams.IsSupersetOf(traditionalTeams))
            {
                WriteLine("超集: companyTeams is a superset of traditionalTeams");
            }

            traditionalTeams.Add("Williams");
            if (privateTeams.Overlaps(traditionalTeams))
            {
                WriteLine("交集: At least one team is the same with traditional and private teams");
            }

            //排序集
            var allTeams = new SortedSet<string>(companyTeams);
            allTeams.UnionWith(privateTeams);
            WriteLine("<all teams>");
            AccessElements1<string>(allTeams);

            allTeams.ExceptWith(privateTeams);
            WriteLine("<no private team left>");
            AccessElements1<string>(allTeams);
        }

        public static void ListSample()
        {
            //初始化 
            var graham = new Racer("Hill", "UK", 14);
            var emerson = new Racer("Fittipaldi", "Brazil", 14);
            var mario = new Racer("Andretti", "USA", 12);
            //初始化容器大小为20
            var racers = new List<Racer>(20) { graham, emerson, mario };

            //增加
            racers.Add(new Racer("Schumacher", "Germany", 91));
            racers.AddRange(new Racer[]{
                new Racer(  "Lauda", "Austria", 25),
               new Racer( "Prost", "France", 51)});
            racers.Insert(3, new Racer("Hill", "USA", 3));

            WriteLine("遍历IEnumerable");
            AccessElements1<Racer>(racers);
            WriteLine("遍历List");
            AccessElements2<Racer>(racers);

            WriteLine("查找");
            int index = racers.IndexOf(mario);
            index = racers.FindIndex(new FindCountry("France").FindCountryPredicate);
            index = racers.FindIndex(r => r.Country == "France");
            Racer racer = racers.Find(r => r.Name == "");
            List<Racer> bigWinners = racers.FindAll(r => r.Wins > 20);
            AccessElements1(bigWinners);

            WriteLine("分组");
            ILookup<string, Racer> lookupRacers = racers.ToLookup(r => r.Country);
            foreach (Racer r in lookupRacers["Australia"])
            {
                WriteLine(r);
            }
        }

        public static void SortedListSample()
        {
            var books = new SortedList<string, string>();
            books.Add("Professional WPF Programming", "978–0–470–04180–2");
            books.Add("Professional ASP.NET MVC 5", "978–1–118-79475–3");
            foreach (KeyValuePair<string, string> book in books)
            {
                WriteLine($"{book.Key}, {book.Value}");
            }
            {
                string isbn;
                string title = "Professional C# 7.0";
                if (!books.TryGetValue(title, out isbn))
                {
                    WriteLine($"{title} not found");
                }
            }
        }

        public static void QueueSample()
        {
            var queue = new Queue<int>();
            queue.Enqueue(1);
            queue.Enqueue(2);
            WriteLine(queue.Peek());
            WriteLine("First iteration: ");
            while (queue.Count > 0)
            {
                WriteLine(queue.Dequeue());
            }
            queue.TrimExcess();
        }

        public static void StackSample()
        {
            var alphabet = new Stack<char>();
            alphabet.Push('A');
            alphabet.Push('B');
            WriteLine("First iteration: ");
            AccessElements1<char>(alphabet);
            WriteLine("Second iteration: ");
            while (alphabet.Count > 0)
            {
                WriteLine(alphabet.Pop());
            }
        }

        public static void LinkedListSample()
        {
            var links = new LinkedList<string>();
            links.AddFirst("First");
            links.AddLast("Last");
            links.AddBefore(links.Last, "Middle");
            AccessElements1<string>(links);
            LinkedListNode<string> node = links.Find("Middle");
            links.Remove(node);
            WriteLine("Remove Middle");
            AccessElements1<string>(links);
        }

        public static void DictionarySample()
        {
            var idTony = new EmployeeId("C3755");
            var tony = new EmployeeData(idTony, "Tony Stewart", 379025.00m);

            var idCarl = new EmployeeId("F3547");
            var carl = new EmployeeData(idCarl, "Carl Edwards", 403466.00m);

            var idKevin = new EmployeeId("C3386");
            var kevin = new EmployeeData(idKevin, "Kevin Harwick", 415261.00m);

            var employees = new Dictionary<EmployeeId, EmployeeData>(31)
            {
                [idTony] = tony,
                [idCarl] = carl,
                [idKevin] = kevin
            };

            foreach (var item in employees.Values)
            {
                WriteLine(item);
            }

            try
            {
                EmployeeId search = new EmployeeId("C755");
                EmployeeData result;
                if (employees.TryGetValue(search, out result))
                {
                    WriteLine($"search result : {result}");
                }
                else
                {
                    WriteLine("Employee with id {0} does not exist", search);
                }
            }
            catch (EmployeeIdException ex)
            {
                WriteLine(ex.Message);
            }
        }

        /* 遍历枚举 */
        private static void AccessElements1<T>(IEnumerable<T> list)
        {
            foreach (var item in list)
            {
                WriteLine(item);
            }
        }
        /* 遍历列表 */
        private static void AccessElements2<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                WriteLine(list[i]);
            }
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

    /* 生成ID, 保证唯一, 判断相等 */
    public struct EmployeeId : IEquatable<EmployeeId>
    {
        private readonly char _prefix;
        private readonly int _number;

        public EmployeeId(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            _prefix = (id.ToUpper())[0];
            int numLength = id.Length - 1;
            try
            {
                _number = int.Parse(id.Substring(1, numLength > 6 ? 6 : numLength));
            }
            catch (FormatException)
            {
                throw new EmployeeIdException("Invalid EmployeeId format");
            }
        }

        public override string ToString() => _prefix.ToString() + $"{_number,6:000000}";

        public override int GetHashCode() => (_number ^ _number << 16) * 0x15051505;

        public bool Equals(EmployeeId other) => (_prefix == other._prefix && _number == other._number);

        public override bool Equals(object obj) => Equals((EmployeeId)obj);

        public static bool operator ==(EmployeeId left, EmployeeId right) => left.Equals(right);

        public static bool operator !=(EmployeeId left, EmployeeId right) => !(left == right);
    }
    public class EmployeeData
    {
        private string _name;
        private decimal _salary;
        private readonly EmployeeId _id;

        public EmployeeData(EmployeeId id, string name, decimal salary)
        {
            _id = id;
            _name = name;
            _salary = salary;
        }

        public override string ToString() => $"{_id.ToString()}: {_name,-20} {_salary:C}";
    }
    public class EmployeeIdException : Exception
    {
        public EmployeeIdException(string message) : base(message) { }
    }
}