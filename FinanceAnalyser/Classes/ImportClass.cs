using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace FinanceAnalyser.Classes
{
    public class ImportClass
    {
        Excel.Application myApp;
        Excel.Workbook myWorkbook;        

        /// <summary>
        /// Takes a string as filepath to the data file
        /// Opens Excel COM link to work with the data
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public Excel.Workbook ImportMethod(string filepath)
        {
            myApp = new Excel.Application();
            myWorkbook = myApp.Workbooks.Open(filepath);

            return myWorkbook;
        }

        /// <summary>
        /// Saves and closes the Excel document, removing the COM link at the same time
        /// </summary>
        public void AppClosing()
        {
            // Close workbook
            myWorkbook.Save();
            myWorkbook.Close(true);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(myWorkbook);

            //Close app & COM connection
            myApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(myApp);
        }
    }
}
