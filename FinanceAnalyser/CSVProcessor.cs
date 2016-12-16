using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;

namespace FinanceAnalyser
{
    /// <summary>
    /// Uses CsvHelper to import CSV files and extract the data in formatted transactions.
    /// </summary>
    public class CSVProcessor
    {
        public IEnumerable<Transaction> GetCSVTransactions(string filepath)
        {
            using (var fileReader = File.OpenText(filepath))
            {
                var csv = new CsvReader(fileReader);
                var transactions = new List<Transaction>();

                //Reads each row one at a time
                while (csv.Read())
                {
                    // Retrieve values from the specified named columns
                    DateTime date = csv.GetField<DateTime>("Transaction Date");
                    string type = csv.GetField("Transaction Type");
                    string description = csv.GetField("Transaction Description");
                    decimal? debit = csv.GetField<decimal?>("Debit Amount");
                    decimal? credit = csv.GetField<decimal?>("Credit Amount");
                    decimal balance = csv.GetField<decimal>("Balance");

                    //Format values into relevant format
                    decimal debitFormatted;
                    decimal creditFormatted;

                    if (!debit.HasValue) { debitFormatted = 0; } else { debitFormatted = debit.Value; }
                    if (!credit.HasValue) { creditFormatted = 0; } else { creditFormatted = credit.Value; }

                    //Build a Transaction from the row data
                    transactions.Add(new Transaction(date, type, description, debitFormatted, creditFormatted, balance));
                }

                return transactions;
            }
        }
    }
}
