using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceAnalyser.Classes
{
    public class CatagorisedTransactions
    {
        public string Catagory { get; set; }
        public string Description { get; set; }
        

        public void Catagorise(string catagory, string description)
        {
            Catagory = catagory;
            Description = description;
        }            
    }
}
