using System;

namespace FinanceAnalyser
{
    /// <summary>
    /// Data for each transaction
    /// </summary>
    public class Transaction
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Amount { get { return Credit - Debit; } }
        public decimal Balance { get; set; }
        public string Category { get; set; }

        public Transaction(DateTime date, string type, string description, decimal debit, decimal credit, decimal balance)
        {
            Date = date;
            Type = type;
            Description = description;
            Debit = Math.Abs(debit);
            Credit = Math.Abs(credit);
            Balance = balance;
        }
    }
}
