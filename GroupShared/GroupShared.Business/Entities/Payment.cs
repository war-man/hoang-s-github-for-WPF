using System;

namespace GroupShared.Business.Entities
{
    public class Payment: BaseEntity
    {
        public DateTime Date { get; set; }
        public decimal Spent { get; set; }
        public string Reason { get; set; }
        public User[] Users { get; set; }
    }
}
