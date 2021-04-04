using System;
using SQLite;

namespace personalFinanceManager.models
{
    public class Income
    {
        [PrimaryKey]
        public string ID { get; set; }
        public double IncomeAmount { get; set; }
        public string IncomeSource { get; set; }
        public DateTime IncomeDate { get; set; } = DateTime.Now;
    }
}
