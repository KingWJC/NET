using System;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using static System.Console;

namespace code
{
    public class StringAndRegularEx
    {
        public static void Test()
        {
            "字符串的使用".WriteTemplate();
            StringSample();
            StringBuilderSample();
            "字符串格式化".WriteTemplate();
            StringFormat();
            "正则表达式".WriteTemplate();
            RegularEx();
        }

        /*
         * @description: 字符串编码
         * @param {*}
         * @return {*}
         */
        private static void StringSample()
        {
            String greetingText = "Hello ";
            greetingText += "World!";
            for (int i = 'a'; i <= 'z'; i++)
            {
                char oldChar = (char)i;
                char newChar = (char)(i - 1);
                greetingText = greetingText.Replace(oldChar, newChar);
            }
            for (int i = 'Z'; i >= 'A'; i--)
            {
                greetingText = greetingText.Replace((char)i, (char)(i + 1));
            }
            WriteLine($"Encode String : {greetingText}");
        }

        /*
         * @description: StringBuilder编码
         * @param {*}
         * @return {*}
         */
        private static void StringBuilderSample()
        {
            StringBuilder greetingText = new StringBuilder("Hello ");
            greetingText.Append("World!");
            for (int i = 'a'; i <= 'z'; i++)
            {
                char oldChar = (char)i;
                char newChar = (char)(i - 1);
                greetingText = greetingText.Replace(oldChar, newChar);
            }
            for (int i = 'Z'; i >= 'A'; i--)
            {
                greetingText = greetingText.Replace((char)i, (char)(i + 1));
            }
            WriteLine($"Encode StringBuilder : {greetingText}");
        }

        /*
         * @description: 自定义格式化
         * @param {*}
         * @return {*}
         */
        private static void StringFormat()
        {
            var user = new User("wjc", "123");
            WriteLine("user.ToString(\"N\") : " + user.ToString("N"));
            WriteLine($"user:N ：{user:N}");
            WriteLine();

            string s1 = "Hello";
            string formatString = $"FormatString: {s1},{{0}}";
            string s2 = "Wrold";
            WriteLine(formatString, s2);
            WriteLine();

            var day = new DateTime(2021, 8, 18);
            WriteLine($"2021-8-18:d : {day:d}");
            WriteLine($"InvariantCulture : {day:d}".ToString(CultureInfo.InvariantCulture));
            WriteLine();

            // invalid format string : formatstring: user:N ：{0:N}
            // ShowFormattableDetail($"user:N ：{user:N}");
            ShowFormattableDetail($"2021-8-18:d : {day:d}");
        }

        /*
         * @description: 输出格式化字符串的参数列表
         * @param {*}
         * @return {*}
         */
        private static void ShowFormattableDetail(FormattableString s)
        {
            WriteLine($"argument count: {s.ArgumentCount}");
            WriteLine($"format: {s.Format}");
            for (int i = 0; i < s.ArgumentCount; i++)
            {
                WriteLine($"Argument {i} : {s.GetArgument(i)}");
            }
        }

        /*
         * @description: 正则表达式
         * @param {*}
         * @return {*}
         */
        private static void RegularEx()
        {
            const string text = @"This book is perfect for both experienced C# programmers : additions and applications "
            + "Hey, I've just found this amazing URI at http:// what was it --oh yes https://www.wrox.com or http://www.wrox.com:80";

            WriteLine("Find per");
            MatchCollection matches = Regex.Matches(text, "per", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            WriteMatches(text, matches);

            WriteLine(@"Find \ba\S*ions\b");
            matches = Regex.Matches(text, @"\ba\S*ions\b", RegexOptions.IgnoreCase);
            WriteMatches(text, matches);

            WriteLine(@"Find named groups");
            string pattern = @"\b(https?)(://)([.\w]+)([\s:]([\d]{2,4})?)\b";
            pattern = @"\b(?<protocol>https?)(?:://)(?<address>[.\w]+)([\s:](?<port>[\d]{2,4})?)\b";
            Regex regex = new Regex(pattern, RegexOptions.ExplicitCapture);
            matches = regex.Matches(text);
            foreach (Match m in matches)
            {
                WriteLine($"Match: {m}\n");
                foreach (Group g in m.Groups)
                {
                    if (g.Success)
                    {
                        WriteLine($"group index: {g.Index}, value: {g.Value}");
                    }
                }

                foreach (var groupName in regex.GetGroupNames())
                {
                    WriteLine($"match for {groupName}: {m.Groups[groupName].Value}");
                }
                WriteLine();
            }
        }

        public static void WriteMatches(string text, MatchCollection matches)
        {
            WriteLine($"Original text was: \n\n{text}\n");
            WriteLine($"No. of matches: {matches.Count}");

            foreach (Match nextMatch in matches)
            {
                int index = nextMatch.Index;
                string result = nextMatch.ToString();
                int charsBefore = (index < 5) ? index : 5;
                int fromEnd = text.Length - index - result.Length;
                int charsAfter = (fromEnd < 5) ? fromEnd : 5;
                int charsToDisplay = charsBefore + charsAfter + result.Length;

                WriteLine($"Index: {index}, \tString: {result}, \t" +
                  $"{text.Substring(index - charsBefore, charsToDisplay)}");
            }
        }
    }

    class User : IFormattable
    {
        public string Name { get; }
        public string Password { get; }
        public User(string name, string password)
        {
            Name = name;
            Password = password;
        }

        public override string ToString()
        {
            return $"Name : {Name}, Password : {Password}";
        }

        //只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释 ? 。
        public string ToString(string format, IFormatProvider formatProvider)
        {
            switch (format)
            {
                case "N": return Name;
                case "P": return Password;
                case "A": return ToString();

                default: throw new FormatException($"invalid format string {format}");
            }
        }

        public virtual string ToString(string format) => ToString(format, null);
    }
}