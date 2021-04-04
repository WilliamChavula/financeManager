using System;
using SQLite;

namespace personalFinanceManager.models
{
    public class Saving
    {
        [PrimaryKey]
        public string ID { get; set; }
        public double SavingAmount { get; set; } = 0.00;
        public DateTime SavingDate { get; set; } = DateTime.Now;
    }
}
