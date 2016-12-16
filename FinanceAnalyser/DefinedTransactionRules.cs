using System.Collections.Generic;

namespace FinanceAnalyser
{
    public class DefinedTransactionRules
    {
        /// <summary>
        /// Takes all of the imported transactions to auto-categorise based on pre-defined rules in this class.
        /// </summary>
        /// <param name="Transactions">All transactions</param>
        /// <returns>Transactions which have been auto-categorised</returns>
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
