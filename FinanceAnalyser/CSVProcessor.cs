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

                    if (!debit.HasValue)
                    {
                        debitFormatted = 0;
                    }
                    else
                    {
                        debitFormatted = debit.Value;
                    }

                    if (!credit.HasValue)
                    {
                        creditFormatted = 0;
                    }
                    else
                    {
                        creditFormatted = credit.Value;
                    }

                    //Build a Transaction from the row data
                    transactions.Add(new Transaction(date, type, description, debitFormatted, creditFormatted, balance));
                }

                return transactions;
            }
        }

        /// <summary>
        /// Old method using manual file reading. Prone to errors.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        //public static IEnumerable<Transaction> GetCSVTransactions(string filepath)
        //{
        //    // We change file extension here to make sure it's a .csv file.
        //    // TODO: Error checking.
        //    string[] lines = File.ReadAllLines(System.IO.Path.ChangeExtension(filepath, ".csv"));

        //    // lines.Select allows me to project each line as a Transaction. 
        //    // This will give me an IEnumerable<Person> back.
        //    return lines.Select(line =>
        //    {
        //        string[] data = line.Split(',');

        //        // We return a transaction with the data in order.
        //        var date = data[0];
        //        var type = data[1];
        //        var sort = data[2];
        //        var accnum = data[3];
        //        var description = data[4];
        //        var debit = data[5];
        //        var credit = data[6];
        //        var balance = data[7];

        //        //Convert results to correct type
        //        DateTime dateFormatted = Convert.ToDateTime(date);
        //        string typeFormatted = Convert.ToString(type);
        //        string descriptionFormatted = Convert.ToString(description);
        //        decimal debitFormatted;
        //        if (debit != "")
        //        {
        //            debitFormatted = Convert.ToDecimal(debit);
        //        }
        //        else
        //        {
        //            debitFormatted = 0;
        //        }

        //        decimal creditFormatted;
        //        if (credit != "")
        //        {
        //            creditFormatted = Convert.ToDecimal(credit);
        //        }
        //        else
        //        {
        //            creditFormatted = 0;
        //        }

        //        decimal balanceFormatted;
        //        if (balance != "")
        //        {
        //            balanceFormatted = Convert.ToDecimal(balance);
        //        }
        //        else
        //        {
        //            balanceFormatted = 0;
        //        }

        //        Transaction currentTransaction = new Transaction(dateFormatted, typeFormatted, descriptionFormatted, debitFormatted, creditFormatted, balanceFormatted);

        //        //Return formatted transaction
        //        return currentTransaction;
        //    });
        //}
    }
}
