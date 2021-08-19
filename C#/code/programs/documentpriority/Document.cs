namespace code.programs.documentpriority {
    public class Document {
        public string Title { get; }
        public string Content { get; }
        public byte Priority { get; }

        public Document (string title, string content, byte priority) {
            this.Title = title;
            this.Content = content;
            this.Priority = priority;
        }

        public override string ToString () {
            return $"Document : {Title},{Content},{Priority}";
        }
    }
}