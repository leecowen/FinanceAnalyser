using System;

namespace FinanceAnalyser
{
    public class OpenFilePicker
    {
        /// <summary>
        /// Gets the filepath of the file the user selected
        /// </summary>
        /// <returns></returns>
        public static string GetFilepath()
        {
            // Create the OpenFIleDialog object
            Microsoft.Win32.OpenFileDialog openPicker = new Microsoft.Win32.OpenFileDialog();

            // We are using excel files in this example
            openPicker.DefaultExt = ".csv";
            openPicker.Filter = "CSV files (*.csv)|*.csv|XML files (*.xml)|*.xml";

            // Display the OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openPicker.ShowDialog();

            // Check to see if we have a result 
            if (result == true)
            {
                return openPicker.FileName.ToString();
            }
            else
            {
                return null;
            }
        }
    }
}
