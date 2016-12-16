using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceAnalyser.Classes
{
    public static class CSVProcessor
    {
        public static IEnumerable<Transaction> GetCSVTransactions(string filepath)
        {
            // We change file extension here to make sure it's a .csv file.
            // TODO: Error checking.
            string[] lines = File.ReadAllLines(System.IO.Path.ChangeExtension(filepath, ".csv"));

            // lines.Select allows me to project each line as a Transaction. 
            // This will give me an IEnumerable<Person> back.
            return lines.Select(line =>
            {
                string[] data = line.Split(',');

                // We return a transaction with the data in order.
                var date = data[0];
                var type = data[1];
                var sort = data[2];
                var accnum = data[3];
                var description = data[4];
                var debit = data[5];
                var credit = data[6];
                var balance = data[7];

                //Convert results to correct type
                string dateFormatted = Convert.ToDateTime(date).ToShortDateString();
                string typeFormatted = Convert.ToString(type);
                string descriptionFormatted = Convert.ToString(description);
                decimal debitFormatted;
                if (debit != "")
                {
                    debitFormatted = Convert.ToDecimal(debit);
                }
                else
                {
                    debitFormatted = 0;
                }

                decimal creditFormatted;
                if (credit != "")
                {
                    creditFormatted = Convert.ToDecimal(credit);
                }
                else
                {
                    creditFormatted = 0;
                }

                decimal balanceFormatted;
                if (balance != "")
                {
                    balanceFormatted = Convert.ToDecimal(balance);
                }
                else
                {
                    balanceFormatted = 0;
                }

                Transaction currentTransaction = new Transaction(dateFormatted, typeFormatted, descriptionFormatted, debitFormatted, creditFormatted, balanceFormatted);

                //Return formatted transaction
                return currentTransaction;
            });
        }

        public static bool GetCSVCatagories()
        {


            return false;
        }

        public static bool WriteCSVCatagories()
        {


            return false;
        }
    }
}
