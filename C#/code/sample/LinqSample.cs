/*
 * @Author: KingWJC
 * @Date: 2021-08-23 10:29:23
 * @LastEditors: KingWJC
 * @LastEditTime: 2021-08-23 17:27:06
 * @Descripttion: 
 * @FilePath: \code\sample\LinqSample.cs
 */

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using static System.Console;

namespace code.sample
{
    public class LinqSample
    {
        public static void Test()
        {
            // LinqFilter();

            LinqJoin();

            // LinqParallel();

            // LinqExpression();
        }

        /*
         * @description:  筛选
         * @param {*}
         * @return {*}
         */
        public static void LinqFilter()
        {
            // Filtering
            var query = from cr in Formula1.GetChampions()
                        where cr.Country == "Brazil" || cr.Country == "Austria"
                        orderby cr.Wins descending
                        select cr;

            var extensionQuery = Formula1.GetChampions().Where(cr => cr.Country == "Brazil").OrderByDescending(cr => cr.Wins).Select(cr => cr);

            //IndexFiltering 索引筛选, 获取姓氏以A开头且索引为偶数的赛车手，索引是筛选器返回结果的计数器, 
            var indexQuery = Formula1.GetChampions().Where((cr, index) => cr.LastName.StartsWith("A") && index % 2 != 0).Select((cr, index) => new { cr, index });
            foreach (var item in indexQuery)
            {
                Console.WriteLine($"{item.index},{item.cr:A}");
            }

            // type Filtering 类型筛选
            var typeQuery = new Object[] { "one", 23, "two", 3, 3 }.OfType<string>();

            // Compound from : 复合查询: 筛选驾驶法拉利的所有冠军
            var ferrariDrivers = from racer in Formula1.GetChampions()
                                 from car in racer.Cars
                                 where car == "Ferrari"
                                 orderby racer.LastName
                                 select racer.FirstName + " " + racer.LastName;

            // 分组，排序, 统计每个国家的赛车手数量
            var groupQuery = from racer in Formula1.GetChampions()
                             group racer by racer.Country into countryGroup
                             where countryGroup.Count() >= 2
                             orderby countryGroup.Count() descending
                             select new { Country = countryGroup.Key, Count = countryGroup.Count() };

            var groupQueryExt = Formula1.GetChampions()
            .GroupBy(racer => racer.Country)
            .OrderByDescending(g => g.Count()).ThenBy(g => g.Key)
            .Where(g => g.Count() >= 2)
            .Select(g => new { Country = g.Key, Count = g.Count() });

            // 定义变量
            var groupQueryVar = from racer in Formula1.GetChampions()
                                group racer by racer.Country into g
                                let count = g.Count()
                                where count >= 2
                                orderby count descending, g.Key
                                select new { Country = g.Key, Count = count };

            var groupQueryVarExt = Formula1.GetChampions()
            .GroupBy(racer => racer.Country)
            .Select(g => new { Group = g, Count = g.Count() })
            .Where(g => g.Count >= 2)
            .OrderByDescending(g => g.Count).ThenBy(g => g.Group.Key)
            .Select(g => new { Country = g.Group.Key, Count = g.Count });

            // 对嵌套的对象分组，内部from子句, 增加每个国际对应的赛车手的信息。
            var nestedGroupQuery = from racer in Formula1.GetChampions()
                                   group racer by racer.Country into g
                                   let count = g.Count()
                                   where count >= 2
                                   orderby count descending, g.Key
                                   select new
                                   {
                                       Country = g.Key,
                                       Count = count,
                                       Racers = from r in g
                                                orderby r.LastName
                                                select r.FirstName + " " + r.LastName
                                   };

            foreach (var item in nestedGroupQuery)
            {
                Console.WriteLine($"{item.Country,-10} {item.Count}");
                foreach (var name in item.Racers)
                {
                    Console.Write($"{name}; ");
                }
                Console.WriteLine();
            }
        }

        /*
         * @description:  连接
         * @param {*}
         * @return {*}
         */
        public static void LinqJoin()
        {
            // 每年获取冠军的赛车手
            var racers = from r in Formula1.GetChampions()
                         from y in r.Years
                         select new { Year = y, Name = r.FirstName + " " + r.LastName };
            //每年获取冠军的车队
            var teams = from t in Formula1.GetConstructorChampions()
                        from y in t.Years
                        select new { Year = y, Name = t.Name };

            //笛卡尔积
            var rtCross = from r in racers
                          from t in teams
                          select new { Year = r.Year, Champion = r.Name, Constructor = t.Name };

            //内连接： 查询每年的冠军车队和赛车手
            var rtInner = from r in racers
                          join t in teams on r.Year equals t.Year
                          orderby t.Year
                          select new { Year = r.Year, Champion = r.Name, Constructor = t.Name };

            //外连接: teams集合的结果放入rt里，其中包含空值情况，Select中需要判断空值
            var rtLeft = from r in racers
                         join t in teams on r.Year equals t.Year into rt
                         from t in rt.DefaultIfEmpty()
                         orderby r.Year
                         select new
                         {
                             Year = r.Year,
                             Champion = r.Name,
                             Constructor = t?.Name ?? "no constructor championship"
                         };

            var rtLeftExt = racers.GroupJoin(teams, r => r.Year, t => t.Year, (r, t) => new { r = r, t = t }).SelectMany(rt => rt.t.DefaultIfEmpty(), (rt, t) => new
            {
                Year = rt.r.Year,
                Champion = rt.r.Name,
                Constructor = t?.Name ?? "no constructor championship"
            });

            Console.WriteLine("Year   World Champion\t   Constructor Title");
            foreach (var item in rtLeft.Take(10))
            {
                Console.WriteLine($"{item.Year}:  {item.Champion,-20} {item.Constructor}");
            }

            //每年的冠军列表包含三个赛车手，所以用SelectManay把列表摊平，返回每一项的3个赛车手的信息。
            var raceInfos = Formula1.GetChampionships()
            .SelectMany(cs => new List<RacerInfo>
            {
                 new RacerInfo {
                   Year = cs.Year,
                   Position = 1,
                   FirstName = cs.First.FirstName(),
                   LastName = cs.First.LastName()
                 },
                 new RacerInfo {
                   Year = cs.Year,
                   Position = 2,
                   FirstName = cs.Second.FirstName(),
                   LastName = cs.Second.LastName()
                 },
                 new RacerInfo {
                   Year = cs.Year,
                   Position = 3,
                   FirstName = cs.Third.FirstName(),
                   LastName = cs.Third.LastName()
                 }
            });

            //组连接, 连接l两个独立的序列，其中一个序列的某个元素，另一个序列存在对应的项列表.
            //通过匿名类型比较，将第二集合的结果添加到yearResult中，获取每个赛车手每年的得奖情况
            var qJoin = (from r in Formula1.GetChampions()
                         join ri in raceInfos on
                         new { FirstName = r.FirstName, LastName = r.LastName }
                         equals
                         new { FirstName = ri.FirstName, LastName = ri.LastName }
                         into yearResult
                         select new { FirstName = r.FirstName, LastName = r.LastName, Wins = r.Wins, Starts = r.Starts, Results = yearResult });

            foreach (var r in qJoin)
            {
                Console.WriteLine($"{r.FirstName} {r.LastName}");
                foreach (var results in r.Results)
                {
                    Console.WriteLine($"{results.Year} {results.Position}");
                }
            }

            // 如果方法不会再其它地方使用，可以定义为委托类型
            Func<string, IEnumerable<CarRacer>> racersByCar = car => from r in Formula1.GetChampions()
                                                                     from c in r.Cars
                                                                     where c == car
                                                                     orderby r.LastName
                                                                     select r;
            // 集合的操作, intersect交集
            Console.WriteLine("World champion with Ferrari and McLaren");
            foreach (var item in racersByCar("Ferrari").Intersect(racersByCar("McLaren")))
            {
                Console.WriteLine(item);
            }
        }

        /*
         * @description:  并行
         * @param {*}
         * @return {*}
         */
        public static void LinqParallel()
        {
            const int arraySize = 50000000;
            var r = new Random();
            var data = Enumerable.Range(0, arraySize).Select(x => r.Next(140));

            DateTime dtRecord = DateTime.Now;
            var res = (from x in data.AsParallel()
                       where Math.Log(x) < 4
                       select x).Average();
            DateTime.Now.Wastetime(dtRecord);

            dtRecord = DateTime.Now;
            var resExt = data.AsParallel().Where(x => Math.Log(x) < 4).Average();
            DateTime.Now.Wastetime(dtRecord);

            dtRecord = DateTime.Now;
            // 自定义分区器,并行模式：不再进行分析而直接采用并行模式， 并行任务数：4
            var resPar = (from x in Partitioner.Create(data.ToList(), loadBalance: true).AsParallel().WithDegreeOfParallelism(4).WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                          where Math.Log(x) < 4
                          select x).Average();
            DateTime.Now.Wastetime(dtRecord);

            Console.WriteLine($"result from {nameof(LinqParallel)}: {resExt}");

            //取消任务
            var cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                try
                {
                    var resCts = (from x in data.AsParallel().WithCancellation(cts.Token)
                                  where Math.Log(x) < 4
                                  select x).Average();
                    Console.WriteLine($"query finished, sum: {res}");
                }
                catch (OperationCanceledException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });

            Console.WriteLine("query started");
            Console.Write("Cancel?");
            string input = Console.ReadLine();
            if (input.ToLower().Equals("y"))
            {
                cts.Cancel();
            }
        }

        /*
         * @description: 表达式树 
         * @param {*}
         * @return {*}
         */
        public static void LinqExpression()
        {
            Expression<Func<CarRacer, bool>> expression = r => r.Country == "Brazil" && r.Wins > 6;

            Expression<Func<string, IEnumerable<CarRacer>>> racersByCar =
            car => from r in Formula1.GetChampions()
                   from c in r.Cars
                   where c == car
                   orderby r.LastName
                   select r;
            DisplayTree(0, "Lambda", racersByCar);
        }

        /*
         * @description: 显示表达式树的结构
         * @param {*}
         * @return {*}
         */
        private static void DisplayTree(int indent, string message, Expression expression)
        {
            string output = $"{string.Empty.PadLeft(indent, '>')} {message} ! NodeType:{expression.NodeType}; Expr: {expression}";

            indent++;
            switch (expression.NodeType)
            {
                case ExpressionType.Lambda:
                    WriteLine(output);
                    LambdaExpression lambdaExpr = (LambdaExpression)expression;
                    foreach (var item in lambdaExpr.Parameters)
                    {
                        DisplayTree(indent, "Parameter", item);
                    }
                    DisplayTree(indent, "Body", lambdaExpr.Body);
                    break;
                case ExpressionType.Constant:
                    ConstantExpression constExpr = (ConstantExpression)expression;
                    WriteLine($"{output} Const Value: {constExpr.Value}");
                    break;
                case ExpressionType.Parameter:
                    ParameterExpression paramExpr = (ParameterExpression)expression;
                    WriteLine($"{output} Param Type: {paramExpr.Type.Name}");
                    break;
                case ExpressionType.Equal:
                case ExpressionType.AndAlso:
                case ExpressionType.GreaterThan:
                    BinaryExpression binExpr = (BinaryExpression)expression;
                    if (binExpr.Method != null)
                    {
                        WriteLine($"{output} Method:{binExpr.Method.Name}");
                    }
                    else
                    {
                        WriteLine(output);
                    }
                    DisplayTree(indent, "Left", binExpr.Left);
                    DisplayTree(indent, "Right", binExpr.Right);
                    break;
                case ExpressionType.MemberAccess:
                    MemberExpression memberExpr = (MemberExpression)expression;
                    WriteLine($"{output} Member Name: {memberExpr.Member.Name}, Type: {memberExpr.Type.Name}");
                    DisplayTree(indent, "Member Expr", memberExpr.Expression);
                    break;
                default:
                    WriteLine();
                    WriteLine($"{expression.NodeType} {expression.Type.Name}");
                    break;
            }
        }
    }
    /*
     * @description: 一级方程式锦标赛
     * @param {*}
     * @return {*}
     */
    static class Formula1
    {
        private static List<CarRacer> _racers;
        public static IList<CarRacer> GetChampions()
        {
            if (_racers == null)
            {
                _racers = new List<CarRacer>(40);
                _racers.Add(new CarRacer("Nino", "Farina", "Italy", 33, 5, new int[] { 1950 }, new string[] { "Alfa Romeo" }));
                _racers.Add(new CarRacer("Alberto", "Ascari", "Italy", 32, 10, new int[] { 1952, 1953 }, new string[] { "Ferrari" }));
                _racers.Add(new CarRacer("Juan Manuel", "Fangio", "Argentina", 51, 24, new int[] { 1951, 1954, 1955, 1956, 1957 }, new string[] { "Alfa Romeo", "Maserati", "Mercedes", "Ferrari" }));
                _racers.Add(new CarRacer("Mike", "Hawthorn", "UK", 45, 3, new int[] { 1958 }, new string[] { "Ferrari" }));
                _racers.Add(new CarRacer("Phil", "Hill", "USA", 48, 3, new int[] { 1961 }, new string[] { "Ferrari" }));
                _racers.Add(new CarRacer("John", "Surtees", "UK", 111, 6, new int[] { 1964 }, new string[] { "Ferrari" }));
                _racers.Add(new CarRacer("Jim", "Clark", "UK", 72, 25, new int[] { 1963, 1965 }, new string[] { "Lotus" }));
                _racers.Add(new CarRacer("Jack", "Brabham", "Australia", 125, 14, new int[] { 1959, 1960, 1966 }, new string[] { "Cooper", "Brabham" }));
                _racers.Add(new CarRacer("Denny", "Hulme", "New Zealand", 112, 8, new int[] { 1967 }, new string[] { "Brabham" }));
                _racers.Add(new CarRacer("Graham", "Hill", "UK", 176, 14, new int[] { 1962, 1968 }, new string[] { "BRM", "Lotus" }));
                _racers.Add(new CarRacer("Jochen", "Rindt", "Austria", 60, 6, new int[] { 1970 }, new string[] { "Lotus" }));
                _racers.Add(new CarRacer("Jackie", "Stewart", "UK", 99, 27, new int[] { 1969, 1971, 1973 }, new string[] { "Matra", "Tyrrell" }));
                _racers.Add(new CarRacer("Emerson", "Fittipaldi", "Brazil", 143, 14, new int[] { 1972, 1974 }, new string[] { "Lotus", "McLaren" }));
                _racers.Add(new CarRacer("James", "Hunt", "UK", 91, 10, new int[] { 1976 }, new string[] { "McLaren" }));
                _racers.Add(new CarRacer("Mario", "Andretti", "USA", 128, 12, new int[] { 1978 }, new string[] { "Lotus" }));
                _racers.Add(new CarRacer("Jody", "Scheckter", "South Africa", 112, 10, new int[] { 1979 }, new string[] { "Ferrari" }));
                _racers.Add(new CarRacer("Alan", "Jones", "Australia", 115, 12, new int[] { 1980 }, new string[] { "Williams" }));
                _racers.Add(new CarRacer("Keke", "Rosberg", "Finland", 114, 5, new int[] { 1982 }, new string[] { "Williams" }));
                _racers.Add(new CarRacer("Niki", "Lauda", "Austria", 173, 25, new int[] { 1975, 1977, 1984 }, new string[] { "Ferrari", "McLaren" }));
                _racers.Add(new CarRacer("Nelson", "Piquet", "Brazil", 204, 23, new int[] { 1981, 1983, 1987 }, new string[] { "Brabham", "Williams" }));
                _racers.Add(new CarRacer("Ayrton", "Senna", "Brazil", 161, 41, new int[] { 1988, 1990, 1991 }, new string[] { "McLaren" }));
                _racers.Add(new CarRacer("Nigel", "Mansell", "UK", 187, 31, new int[] { 1992 }, new string[] { "Williams" }));
                _racers.Add(new CarRacer("Alain", "Prost", "France", 197, 51, new int[] { 1985, 1986, 1989, 1993 }, new string[] { "McLaren", "Williams" }));
                _racers.Add(new CarRacer("Damon", "Hill", "UK", 114, 22, new int[] { 1996 }, new string[] { "Williams" }));
                _racers.Add(new CarRacer("Jacques", "Villeneuve", "Canada", 165, 11, new int[] { 1997 }, new string[] { "Williams" }));
                _racers.Add(new CarRacer("Mika", "Hakkinen", "Finland", 160, 20, new int[] { 1998, 1999 }, new string[] { "McLaren" }));
                _racers.Add(new CarRacer("Michael", "Schumacher", "Germany", 287, 91, new int[] { 1994, 1995, 2000, 2001, 2002, 2003, 2004 }, new string[] { "Benetton", "Ferrari" }));
                _racers.Add(new CarRacer("Fernando", "Alonso", "Spain", 273, 33, new int[] { 2005, 2006 }, new string[] { "Renault" }));
                _racers.Add(new CarRacer("Kimi", "Räikkönen", "Finland", 253, 20, new int[] { 2007 }, new string[] { "Ferrari" }));
                _racers.Add(new CarRacer("Lewis", "Hamilton", "UK", 189, 53, new int[] { 2008, 2014, 2015 }, new string[] { "McLaren", "Mercedes" }));
                _racers.Add(new CarRacer("Jenson", "Button", "UK", 306, 16, new int[] { 2009 }, new string[] { "Brawn GP" }));
                _racers.Add(new CarRacer("Sebastian", "Vettel", "Germany", 179, 42, new int[] { 2010, 2011, 2012, 2013 }, new string[] { "Red Bull Racing" }));
                _racers.Add(new CarRacer("Nico", "Rosberg", "Germany", 207, 24, new int[] { 2016 }, new string[] { "Mercedes" }));
            }

            return _racers;
        }

        private static List<Team> _teams;
        public static IList<Team> GetConstructorChampions()
        {
            if (_teams == null)
            {
                _teams = new List<Team>()
                {
                    new Team("Vanwall", 1958),
                    new Team("Cooper", 1959, 1960),
                    new Team("Ferrari", 1961, 1964, 1975, 1976, 1977, 1979, 1982, 1983, 1999, 2000, 2001, 2002, 2003, 2004, 2007, 2008),
                    new Team("BRM", 1962),
                    new Team("Lotus", 1963, 1965, 1968, 1970, 1972, 1973, 1978),
                    new Team("Brabham", 1966, 1967),
                    new Team("Matra", 1969),
                    new Team("Tyrrell", 1971),
                    new Team("McLaren", 1974, 1984, 1985, 1988, 1989, 1990, 1991, 1998),
                    new Team("Williams", 1980, 1981, 1986, 1987, 1992, 1993, 1994, 1996, 1997),
                    new Team("Benetton", 1995),
                    new Team("Renault", 2005, 2006 ),
                    new Team("Brawn GP", 2009),
                    new Team("Red Bull Racing", 2010, 2011, 2012, 2013),
                    new Team("Mercedes", 2014, 2015, 2016)
                };
            }
            return _teams;
        }

        private static List<Championship> _championships;
        public static IEnumerable<Championship> GetChampionships()
        {
            if (_championships == null)
            {
                _championships = new List<Championship>();
                _championships.Add(new Championship
                {
                    Year = 1950,
                    First = "Nino Farina",
                    Second = "Juan Manuel Fangio",
                    Third = "Luigi Fagioli"
                });
                _championships.Add(new Championship
                {
                    Year = 1951,
                    First = "Juan Manuel Fangio",
                    Second = "Alberto Ascari",
                    Third = "Froilan Gonzalez"
                });
                _championships.Add(new Championship
                {
                    Year = 1952,
                    First = "Alberto Ascari",
                    Second = "Nino Farina",
                    Third = "Piero Taruffi"
                });
                _championships.Add(new Championship
                {
                    Year = 1953,
                    First = "Alberto Ascari",
                    Second = "Juan Manuel Fangio",
                    Third = "Nino Farina"
                });
                _championships.Add(new Championship
                {
                    Year = 1954,
                    First = "Juan Manuel Fangio",
                    Second = "Froilan Gonzalez",
                    Third = "Mike Hawthorn"
                });
                _championships.Add(new Championship
                {
                    Year = 1955,
                    First = "Juan Manuel Fangio",
                    Second = "Stirling Moss",
                    Third = "Eugenio Castellotti"
                });
                _championships.Add(new Championship
                {
                    Year = 1956,
                    First = "Juan Manuel Fangio",
                    Second = "Stirling Moss",
                    Third = "Peter Collins"
                });
                _championships.Add(new Championship
                {
                    Year = 1957,
                    First = "Juan Manuel Fangio",
                    Second = "Stirling Moss",
                    Third = "Luigi Musso"
                });
                _championships.Add(new Championship
                {
                    Year = 1958,
                    First = "Mike Hawthorn",
                    Second = "Stirling Moss",
                    Third = "Tony Brooks"
                });
                _championships.Add(new Championship
                {
                    Year = 1959,
                    First = "Jack Brabham",
                    Second = "Tony Brooks",
                    Third = "Stirling Moss"
                });
                _championships.Add(new Championship
                {
                    Year = 1960,
                    First = "Jack Brabham",
                    Second = "Bruce McLaren",
                    Third = "Stirling Moss"
                });
                _championships.Add(new Championship
                {
                    Year = 1961,
                    First = "Phil Hill",
                    Second = "Wolfgang von Trips",
                    Third = "Stirling Moss"
                });
                _championships.Add(new Championship
                {
                    Year = 1962,
                    First = "Graham Hill",
                    Second = "Jim Clark",
                    Third = "Bruce McLaren"
                });
                _championships.Add(new Championship
                {
                    Year = 1963,
                    First = "Jim Clark",
                    Second = "Graham Hill",
                    Third = "Richie Ginther"
                });
                _championships.Add(new Championship
                {
                    Year = 1964,
                    First = "John Surtees",
                    Second = "Graham Hill",
                    Third = "Jim Clark"
                });
                _championships.Add(new Championship
                {
                    Year = 1965,
                    First = "Jim Clark",
                    Second = "Graham Hill",
                    Third = "Jackie Stewart"
                });
                _championships.Add(new Championship
                {
                    Year = 1966,
                    First = "Jack Brabham",
                    Second = "John Surtees",
                    Third = "Jochen Rindt"
                });
                _championships.Add(new Championship
                {
                    Year = 1967,
                    First = "Dennis Hulme",
                    Second = "Jack Brabham",
                    Third = "Jim Clark"
                });
                _championships.Add(new Championship
                {
                    Year = 1968,
                    First = "Graham Hill",
                    Second = "Jackie Stewart",
                    Third = "Dennis Hulme"
                });
                _championships.Add(new Championship
                {
                    Year = 1969,
                    First = "Jackie Stewart",
                    Second = "Jackie Ickx",
                    Third = "Bruce McLaren"
                });
                _championships.Add(new Championship
                {
                    Year = 1970,
                    First = "Jochen Rindt",
                    Second = "Jackie Ickx",
                    Third = "Clay Regazzoni"
                });
                _championships.Add(new Championship
                {
                    Year = 1971,
                    First = "Jackie Stewart",
                    Second = "Ronnie Peterson",
                    Third = "Francois Cevert"
                });
                _championships.Add(new Championship
                {
                    Year = 1972,
                    First = "Emerson Fittipaldi",
                    Second = "Jackie Stewart",
                    Third = "Dennis Hulme"
                });
                _championships.Add(new Championship
                {
                    Year = 1973,
                    First = "Jackie Stewart",
                    Second = "Emerson Fittipaldi",
                    Third = "Ronnie Peterson"
                });
                _championships.Add(new Championship
                {
                    Year = 1974,
                    First = "Emerson Fittipaldi",
                    Second = "Clay Regazzoni",
                    Third = "Jody Scheckter"
                });
                _championships.Add(new Championship
                {
                    Year = 1975,
                    First = "Niki Lauda",
                    Second = "Emerson Fittipaldi",
                    Third = "Carlos Reutemann"
                });
                _championships.Add(new Championship
                {
                    Year = 1976,
                    First = "James Hunt",
                    Second = "Niki Lauda",
                    Third = "Jody Scheckter"
                });
                _championships.Add(new Championship
                {
                    Year = 1977,
                    First = "Niki Lauda",
                    Second = "Jody Scheckter",
                    Third = "Mario Andretti"
                });
                _championships.Add(new Championship
                {
                    Year = 1978,
                    First = "Mario Andretti",
                    Second = "Ronnie Peterson",
                    Third = "Carlos Reutemann"
                });
                _championships.Add(new Championship
                {
                    Year = 1979,
                    First = "Jody Scheckter",
                    Second = "Gilles Villeneuve",
                    Third = "Alan Jones"
                });
                _championships.Add(new Championship
                {
                    Year = 1980,
                    First = "Alan Jones",
                    Second = "Nelson Piquet",
                    Third = "Carlos Reutemann"
                });
                _championships.Add(new Championship
                {
                    Year = 1981,
                    First = "Nelson Piquet",
                    Second = "Carlos Reutemann",
                    Third = "Alan Jones"
                });
                _championships.Add(new Championship
                {
                    Year = 1982,
                    First = "Keke Rosberg",
                    Second = "Didier Pironi",
                    Third = "John Watson"
                });
                _championships.Add(new Championship
                {
                    Year = 1983,
                    First = "Nelson Piquet",
                    Second = "Alain Prost",
                    Third = "Rene Arnoux"
                });
                _championships.Add(new Championship
                {
                    Year = 1984,
                    First = "Niki Lauda",
                    Second = "Alain Prost",
                    Third = "Elio de Angelis"
                });
                _championships.Add(new Championship
                {
                    Year = 1985,
                    First = "Alain Prost",
                    Second = "Michele Alboreto",
                    Third = "Keke Rosberg"
                });
                _championships.Add(new Championship
                {
                    Year = 1986,
                    First = "Alain Prost",
                    Second = "Nigel Mansell",
                    Third = "Nelson Piquet"
                });
                _championships.Add(new Championship
                {
                    Year = 1987,
                    First = "Nelson Piquet",
                    Second = "Nigel Mansell",
                    Third = "Ayrton Senna"
                });
                _championships.Add(new Championship
                {
                    Year = 1988,
                    First = "Ayrton Senna",
                    Second = "Alain Prost",
                    Third = "Gerhard Berger"
                });
                _championships.Add(new Championship
                {
                    Year = 1989,
                    First = "Alain Prost",
                    Second = "Ayrton Senna",
                    Third = "Riccardo Patrese"
                });
                _championships.Add(new Championship
                {
                    Year = 1990,
                    First = "Ayrton Senna",
                    Second = "Alain Prost",
                    Third = "Nelson Piquet"
                });
                _championships.Add(new Championship
                {
                    Year = 1991,
                    First = "Ayrton Senna",
                    Second = "Nigel Mansell",
                    Third = "Riccardo Patrese"
                });
                _championships.Add(new Championship
                {
                    Year = 1992,
                    First = "Nigel Mansell",
                    Second = "Riccardo Patrese",
                    Third = "Michael Schumacher"
                });
                _championships.Add(new Championship
                {
                    Year = 1993,
                    First = "Alain Prost",
                    Second = "Ayrton Senna",
                    Third = "Damon Hill"
                });
                _championships.Add(new Championship
                {
                    Year = 1994,
                    First = "Michael Schumacher",
                    Second = "Damon Hill",
                    Third = "Gerhard Berger"
                });
                _championships.Add(new Championship
                {
                    Year = 1995,
                    First = "Michael Schumacher",
                    Second = "Damon Hill",
                    Third = "David Coulthard"
                });
                _championships.Add(new Championship
                {
                    Year = 1996,
                    First = "Damon Hill",
                    Second = "Jacques Villeneuve",
                    Third = "Michael Schumacher"
                });
                _championships.Add(new Championship
                {
                    Year = 1997,
                    First = "Jacques Villeneuve",
                    Second = "Heinz-Harald Frentzen",
                    Third = "David Coulthard"
                });
                _championships.Add(new Championship
                {
                    Year = 1998,
                    First = "Mika Hakkinen",
                    Second = "Michael Schumacher",
                    Third = "David Coulthard"
                });
                _championships.Add(new Championship
                {
                    Year = 1999,
                    First = "Mika Hakkinen",
                    Second = "Eddie Irvine",
                    Third = "Heinz-Harald Frentzen"
                });
                _championships.Add(new Championship
                {
                    Year = 2000,
                    First = "Michael Schumacher",
                    Second = "Mika Hakkinen",
                    Third = "David Coulthard"
                });
                _championships.Add(new Championship
                {
                    Year = 2001,
                    First = "Michael Schumacher",
                    Second = "David Coulthard",
                    Third = "Rubens Barrichello"
                });
                _championships.Add(new Championship
                {
                    Year = 2002,
                    First = "Michael Schumacher",
                    Second = "Rubens Barrichello",
                    Third = "Juan Pablo Montoya"
                });
                _championships.Add(new Championship
                {
                    Year = 2003,
                    First = "Michael Schumacher",
                    Second = "Kimi Räikkönen",
                    Third = "Juan Pablo Montoya"
                });
                _championships.Add(new Championship
                {
                    Year = 2004,
                    First = "Michael Schumacher",
                    Second = "Rubens Barrichello",
                    Third = "Jenson Button"
                });
                _championships.Add(new Championship
                {
                    Year = 2005,
                    First = "Fernando Alonso",
                    Second = "Kimi Räikkönen",
                    Third = "Michael Schumacher"
                });
                _championships.Add(new Championship
                {
                    Year = 2006,
                    First = "Fernando Alonso",
                    Second = "Michael Schumacher",
                    Third = "Felipe Massa"
                });
                _championships.Add(new Championship
                {
                    Year = 2007,
                    First = "Kimi Räikkönen",
                    Second = "Lewis Hamilton",
                    Third = "Fernando Alonso"
                });
                _championships.Add(new Championship
                {
                    Year = 2008,
                    First = "Lewis Hamilton",
                    Second = "Felipe Massa",
                    Third = "Kimi Raikkonen"
                });
                _championships.Add(new Championship
                {
                    Year = 2009,
                    First = "Jenson Button",
                    Second = "Sebastian Vettel",
                    Third = "Rubens Barrichello"
                });
                _championships.Add(new Championship
                {
                    Year = 2010,
                    First = "Sebastian Vettel",
                    Second = "Fernando Alonso",
                    Third = "Mark Webber"
                });
                _championships.Add(new Championship
                {
                    Year = 2011,
                    First = "Sebastian Vettel",
                    Second = "Jenson Button",
                    Third = "Mark Webber"
                });
                _championships.Add(new Championship
                {
                    Year = 2012,
                    First = "Sebastian Vettel",
                    Second = "Fernando Alonso",
                    Third = "Kimi Raikkonen"
                });
                _championships.Add(new Championship
                {
                    Year = 2013,
                    First = "Sebastian Vettel",
                    Second = "Fernando Alonso",
                    Third = "Mark Webber"
                });
                _championships.Add(new Championship
                {
                    Year = 2014,
                    First = "Lewis Hamilton",
                    Second = "Nico Rosberg",
                    Third = "Daniel Ricciardo"
                });
                _championships.Add(new Championship
                {
                    Year = 2015,
                    First = "Lewis Hamilton",
                    Second = "Nico Rosberg",
                    Third = "Sebastian Vettel"
                });
                _championships.Add(new Championship
                {
                    Year = 2016,
                    First = "Nico Rosberg",
                    Second = "Lewis Hamilton",
                    Third = "Daniel Ricciardo"
                });

            }
            return _championships;
        }

        private static IList<CarRacer> _moreRacers;
        private static IList<CarRacer> GetMoreRacers()
        {
            if (_moreRacers == null)
            {
                _moreRacers = new List<CarRacer>();
                _moreRacers.Add(new CarRacer("Luigi", "Fagioli", "Italy", starts: 7, wins: 1));
                _moreRacers.Add(new CarRacer("Jose Froilan", "Gonzalez", "Argentina", starts: 26, wins: 2));
                _moreRacers.Add(new CarRacer("Piero", "Taruffi", "Italy", starts: 18, wins: 1));
                _moreRacers.Add(new CarRacer("Stirling", "Moss", "UK", starts: 66, wins: 16));
                _moreRacers.Add(new CarRacer("Eugenio", "Castellotti", "Italy", starts: 14, wins: 0));
                _moreRacers.Add(new CarRacer("Peter", "Collins", "UK", starts: 32, wins: 3));
                _moreRacers.Add(new CarRacer("Luigi", "Musso", "Italy", starts: 24, wins: 1));
                _moreRacers.Add(new CarRacer("Tony", "Brooks", "UK", starts: 38, wins: 6));
                _moreRacers.Add(new CarRacer("Bruce", "McLaren", "New Zealand", starts: 100, wins: 4));
                _moreRacers.Add(new CarRacer("Wolfgang von", "Trips", "Germany", starts: 27, wins: 2));
                _moreRacers.Add(new CarRacer("Richie", "Ginther", "USA", starts: 52, wins: 1));
                _moreRacers.Add(new CarRacer("Jackie", "Ickx", "Belgium", starts: 116, wins: 8));
                _moreRacers.Add(new CarRacer("Clay", "Regazzoni", "Switzerland", starts: 132, wins: 5));
                _moreRacers.Add(new CarRacer("Ronnie", "Peterson", "Sweden", starts: 123, wins: 10));
                _moreRacers.Add(new CarRacer("Francois", "Cevert", "France", starts: 46, wins: 1));
                _moreRacers.Add(new CarRacer("Carlos", "Reutemann", "Argentina", starts: 146, wins: 12));
                _moreRacers.Add(new CarRacer("Gilles", "Villeneuve", "Canada", starts: 67, wins: 6));
                _moreRacers.Add(new CarRacer("Didier", "Pironi", "France", starts: 70, wins: 3));
                _moreRacers.Add(new CarRacer("John", "Watson", "UK", starts: 152, wins: 5));
                _moreRacers.Add(new CarRacer("Rene", "Arnoux", "France", starts: 149, wins: 7));
                _moreRacers.Add(new CarRacer("Elio", "de Angelis", "Italy", starts: 108, wins: 2));
                _moreRacers.Add(new CarRacer("Michele", "Alboreto", "Italy", starts: 194, wins: 5));
                _moreRacers.Add(new CarRacer("Gerhard", "Berger", "Austria", starts: 210, wins: 10));
                _moreRacers.Add(new CarRacer("Riccardo", "Patrese", "Italy", starts: 256, wins: 6));
                _moreRacers.Add(new CarRacer("David", "Coulthard", "UK", starts: 246, wins: 13));
                _moreRacers.Add(new CarRacer("Heinz-Harald", "Frentzen", "Germany", starts: 156, wins: 3));
                _moreRacers.Add(new CarRacer("Eddie", "Irvine", "UK", starts: 147, wins: 4));
                _moreRacers.Add(new CarRacer("Rubens", "Barrichello", "Brazil", starts: 322, wins: 11));
                _moreRacers.Add(new CarRacer("Juan Pablo", "Montoya", "Columbia", starts: 94, wins: 7));
                _moreRacers.Add(new CarRacer("Felipe", "Massa", "Brazil", starts: 251, wins: 11));
                _moreRacers.Add(new CarRacer("Mark", "Webber", "Australia", starts: 215, wins: 9));
                _moreRacers.Add(new CarRacer("Daniel", "Ricciardo", "Australia", starts: 109, wins: 4));
            }
            return _moreRacers;
        }
    }

    /*
     * @description: 每年比赛的前三名赛车手的信息
     * @param {*}
     * @return {*}
     */
    class RacerInfo
    {
        public int Year { get; set; }
        public int Position { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    /*
     * @description: 每年的比赛冠军信息
     * @param {*}
     * @return {*}
     */
    class Championship
    {
        public int Year { get; set; }
        public string First { get; set; }
        public string Second { get; set; }
        public string Third { get; set; }
    }

    /*
     * @description: 赛车手
     * @param {*}
     * @return {*}
     */
    class CarRacer : IComparable<CarRacer>, IFormattable
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public int Wins { get; set; }
        public int Starts { get; set; }
        // 赛车手在获取冠军时，开的赛车
        public IEnumerable<string> Cars { get; }
        // 赛车手获取冠军的年份
        public IEnumerable<int> Years { get; }

        public CarRacer(string firstName, string lastName, string country, int starts, int wins, IEnumerable<int> years, IEnumerable<string> cars)
        {
            FirstName = firstName;
            LastName = lastName;
            Country = country;
            Starts = starts;
            Wins = wins;
            Years = years != null ? new List<int>(years) : new List<int>();
            Cars = cars != null ? new List<string>(cars) : new List<string>();
        }

        public CarRacer(string firstName, string lastName, string country, int starts, int wins)
          : this(firstName, lastName, country, starts, wins, null, null) { }

        public override string ToString() => $"{FirstName} {LastName}";

        public string ToString(string format, IFormatProvider formatProvider)
        {
            switch (format)
            {
                case null:
                case "N": return ToString();
                case "F":
                    return FirstName;
                case "L":
                    return LastName;
                case "C":
                    return Country;
                case "S":
                    return Starts.ToString();
                case "W":
                    return Wins.ToString();
                case "A":
                    return $"{FirstName} {LastName}, country: {Country}; starts: {Starts}, wins: {Wins}";
                default:
                    throw new FormatException($"Format {format} not supported");
            }
        }

        public int CompareTo(CarRacer other)
        {
            return LastName?.CompareTo(other?.LastName) ?? -1;
        }
    }

    /*
     * @description: 车队
     * @param {*}
     * @return {*}
     */
    class Team
    {
        // 车队名称
        public string Name { get; }
        // 车队获取冠军的年份
        public IEnumerable<int> Years { get; }

        public Team(string name, params int[] years)
        {
            this.Name = name;
            this.Years = years != null ? new List<int>(years) : new List<int>();
        }
    }
}