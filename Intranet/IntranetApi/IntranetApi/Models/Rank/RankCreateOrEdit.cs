﻿namespace IntranetApi.Models
{
    public class RankCreateOrEdit : BaseEntity
    {
        public string Name { get; set; }
        public bool Status { get; set; }
        public int Level { get; set; }
    }
}