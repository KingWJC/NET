namespace code.programs.pipelinetask
{
    public class Info
    {
        public string Word { get; set; }
        public int Count { get; set; }
        public string Color { get; set; }

        public override string ToString() => $"{Count} times: {Word}";
    }
}