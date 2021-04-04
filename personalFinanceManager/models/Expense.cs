using System;
using SQLite;

namespace personalFinanceManager.models
{
    public class Expense
    {
        [PrimaryKey]
        public string ID { get; set; }
        public string ExpenseName { get; set; }
        public string ExpenseCategory { get; set; }
        public double ExpenseAmount { get; set; } = 0.00;
        public DateTime ExpenseDate { get; set; } = DateTime.Now;
    }
}
