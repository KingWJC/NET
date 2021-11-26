namespace ADF.Entity
{
    public class TableSpace
    {
        private string tsName;
        public string TsName { get; set; }

        private string allBytes;
        public string AllBytes { get; set; }

        private string freeBytes;
        public string FreeBytes { get; set; }

        private string usedBytes;
        public string UsedBytes { get; set; }

        private string scale;
        public string Scale { get; set; }
    }
}