using static System.Console;

namespace code
{
    public class ExtensionMethod
    {
        public static void Test()
        {
            "Extension Method Invoked".WriteTemplate();
            string fox = "the quick brown fox jumped over the lazy dogs down 987654321 times";
            //编译器改为调用静态方法: StringExtension.GetWordCount(fox);
            int wordCount = fox.GetWordCount();
            WriteLine($"<{fox}> has {wordCount} words");
        }
    }

    static class StringExtension
    {
        public static int GetWordCount(this string s) => s.Split().Length;

        public static void WriteTemplate(this string s)
        {
            WriteLine();
            WriteLine($"===================={s}==================");
        }
    }
}