namespace code.programs.documentconsume
{
    public class Document
    {
        public string Title { get; }
        public string Content { get; }

        public Document(string title, string content)
        {
            this.Title = title;
            this.Content = content;
        }

        public override string ToString()
        {
            return $"Document : {Title},{Content}";
        }
    }
}