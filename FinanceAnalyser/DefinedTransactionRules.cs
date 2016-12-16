using System.Collections.Generic;

namespace FinanceAnalyser
{
    public class DefinedTransactionRules
    {
        public static List<Transaction> ApplyDefinedTransactionRules(List<Transaction> Transactions)
        {
            List<Transaction> transactionsRuled = new List<Transaction>();

            foreach(var transaction in Transactions)
            {
                if (transaction.Category != "")
                {                    
                    if (transaction.Type == "CASHPOINT")
                    {
                        transaction.Category = "Cash";
                        transactionsRuled.Add(transaction);
                    }
                    else if (transaction.Description.Contains("Spotify"))
                    {
                        transaction.Category = "Spotify";
                        transactionsRuled.Add(transaction);
                    }
                }
            }

            return transactionsRuled;
        }
    }
}
