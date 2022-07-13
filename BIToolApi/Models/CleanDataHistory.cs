namespace BITool.Models
{
    public class CleanDataHistory
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public DateTime CleanTime { get; set; }
        public string Source { get; set; }
        public int TotalRows { get; set; }
        public int TotalInvalidNumbers { get; set; }
        public int TotalDuplicateNumbersWithSystem { get; set; }
        public int TotalDuplicateNumbersInFile { get; set; }
    }
}