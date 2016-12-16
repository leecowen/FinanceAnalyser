using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FinanceAnalyser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Transaction> Transactions = new List<Transaction>();
        Dictionary<string, string> MatchedCategories = new Dictionary<string, string>();
        Dictionary<string, string> NewMatchedCategories = new Dictionary<string, string>();
        Transaction currentTransaction = null;
        SQLiteConnection dbConnection = new SQLiteConnection();

        /// <summary>
        /// App launch.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            ConnectToDatabase();
        }

        /// <summary>
        /// Connect to the SQLite database
        /// </summary>
        private void ConnectToDatabase()
        {
            dbConnection = DatabaseConnector.InitializeDatabase();
            MatchedCategories = DatabaseConnector.LoadSavedCategories(dbConnection);
        }

        /// <summary>
        /// User clicked the 'Import' button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportCSVFile_Click(object sender, RoutedEventArgs e)
        {
            //Prompts the user to select the file containing data
            string filepath = OpenFilePicker.GetFilepath();

            //Pulls all transactions from the CSV file and formats
            CSVProcessor csvProcessor = new CSVProcessor();
            Transactions = csvProcessor.GetCSVTransactions(filepath).ToList();


            // Apply categories to descriptions which meet predefined rules
            List<Transaction> TransactionsTest = new List<Transaction>();

            //Make things visible!
            ListViewTransactions.ItemsSource = Transactions;
            Phase1StackPanel.Visibility = Visibility.Collapsed;
            ListViewTransactions.Visibility = Visibility.Visible;
            Phase2StackPanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Loops through all of the transactions and adds categories if already known.
        /// </summary>
        private void CategoriseKnownTransactionDescriptions()
        {
            foreach (var transaction in Transactions)
            {
                string categoryName;

                if (MatchedCategories.TryGetValue(transaction.Description, out categoryName))
                {
                    transaction.Category = categoryName;
                }
            }
        }

        /// <summary>
        /// Applied catagories based upon predefined rules where possible to transactions, before showing the remaining unknown transactions to the user to categorise.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Phase4StartCategorising_Click(object sender, RoutedEventArgs e)
        {
            // Hide the previous phase
            ListViewTransactions.Visibility = Visibility.Collapsed;
            Phase2StackPanel.Visibility = Visibility.Collapsed;

            // Applies pre-defined rules against transactions to apply categories where possible (Cashpoints, spotify, etc)
            List<Transaction> transactionsRuled = DefinedTransactionRules.ApplyDefinedTransactionRules(Transactions);

            CategoriseKnownTransactionDescriptions();

            // Show the category sorting page
            Phase4StackPanel.Visibility = Visibility.Visible;

            Phase4MoveToNextTransaction();
        }

        /// <summary>
        /// Takes the first transaction in the list and displays it to the user to categorise
        /// </summary>
        private void Phase4MoveToNextTransaction()
        {
            // Keep count of the number of unique transaction descriptions which don't have a category
            int remaining = Transactions.Where(t => string.IsNullOrEmpty(t.Category)).Select(t => t.Description).Distinct().Count();

            // Get the first transaction. If null move to the next phase, else display the currentTranaction to the user
            currentTransaction = Transactions.FirstOrDefault(t => string.IsNullOrEmpty(t.Category));
            if (currentTransaction == null)
            {
                Phase5Start();
            }
            else
            {
                Phase4Remaining.Text = "Unknown transactions remaining: " + remaining;
                Phase4Date.Text = "Date: " + currentTransaction.Date;
                Phase4Type.Text = "Type: " + currentTransaction.Type;
                Phase4Description.Text = "Description: " + currentTransaction.Description;
                Phase4Debit.Text = "Debit amount: " + currentTransaction.Debit.ToString();
                Phase4Credit.Text = "Credit amount: " + currentTransaction.Credit.ToString();
            }
        }

        /// <summary>
        /// Takes the category the user has entered and applies it to all transactions with the same description.
        /// Add this new transaction description match to MatchedCategories and NewMatchedCategories so we know what to save to the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitCategory(object sender, RoutedEventArgs e)
        {
            string submittedCategory = CategoryInputBox.Text;
            string currentDescription = currentTransaction.Description;

            // Add this new match to our list of matchedCategories
            MatchedCategories.Add(currentDescription, submittedCategory);
            NewMatchedCategories.Add(currentDescription, submittedCategory);

            // Enable the save button
            Phase4SaveButton.Content = "Save progress";
            Phase4SaveButton.IsEnabled = true;

            //Move to the next transaction
            CategoriseKnownTransactionDescriptions();
            Phase4MoveToNextTransaction();
        }

        /// <summary>
        /// User submitting a new category for a transaction description and used the 'Enter' key rather than clicking submit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Phase4SubmitCategoryEnterKey(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SubmitCategory(sender, e);
                CategoryInputBox.Clear();
            }
        }

        /// <summary>
        /// Allows the user to save the categorising they have done so far and save it to the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Phase4SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Save NewMatchedCategories to the database
            DatabaseConnector.SaveCategories(NewMatchedCategories, dbConnection);

            // Empty NewMatchedCategories as they have been saved to the database.
            NewMatchedCategories.Clear();

            // Disable the save button and tell the user it was successful.
            Phase4SaveButton.Content = "Successfully saved";
            Phase4SaveButton.IsEnabled = false;
        }

        /// <summary>
        /// Phase 5 
        /// </summary>
        private void Phase5Start()
        {
            // Hide the previous phase
            Phase4SubmitButton.IsEnabled = false;
            Phase4SaveButton.Visibility = Visibility.Collapsed;
            Phase5ListView.Visibility = Visibility.Visible;

            //// THIS IS WHERE WE WANT TO SPLIT CATEGORYTOTALS BY MONTH RATHER THAN JUST A LUMP SUM

            Dictionary<string, decimal> categoryTotals = new Dictionary<string, decimal>();
            var months = Transactions.GroupBy(t => t.Date.Month);

            // Sum the total amount for each category
            foreach (var category in MatchedCategories.Values.Distinct())
            {
                //Totals the values associated with each category     
                categoryTotals.Add(category, Transactions.Where(t => t.Category == category).Sum(t => t.Amount));
            }

            // Update the UI to show the results
            Phase5ListView.ItemsSource = categoryTotals;
        }
    }
}
