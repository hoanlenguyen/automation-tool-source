﻿namespace IntranetApi.Models
{
    public class BankCreateOrEdit : BaseEntity
    {
        public string Name { get; set; }
        public bool Status { get; set; }
        public DateTime CreationTime { get; set; }
    }
}