namespace code.programs.documentpriority
{
    public class Client
    {
        public static void Test()
        {
            DocumentManager pdm = new DocumentManager();
            pdm.Add(new Document("one", "Sample", 8));
            pdm.Add(new Document("two", "Sample", 3));
            pdm.Add(new Document("three", "Sample", 4));
            pdm.Add(new Document("four", "Sample", 8));
            pdm.Add(new Document("five", "Sample", 1));
            pdm.Add(new Document("six", "Sample", 9));
            pdm.Add(new Document("seven", "Sample", 1));
            pdm.Add(new Document("eight", "Sample", 1));

            pdm.DisplayAllNodes();
        }
    }
}