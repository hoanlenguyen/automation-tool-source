﻿namespace BITool.Models
{
    public class ImportDataHistory
    {
        public int ID { get; set; }
        public string ImportName { get; set; }
        public string FileName { get; set; }
        public DateTime ImportTime { get; set; } = DateTime.Now;        
        public string Source { get; set; }
        public int TotalRows { get; set; }
        public int TotalErrorRows { get; set; }
        public string ImportByEmail { get; set; }
    }
}