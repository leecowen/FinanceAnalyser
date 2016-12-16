﻿using System;
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
        // SERIALISE THIS SHIT
        Dictionary<string, string> MatchedCategories = new Dictionary<string, string>();
        Dictionary<string, string> NewMatchedCategories = new Dictionary<string, string>();
        Transaction currentTransaction = null;
        SQLiteConnection dbConnection = new SQLiteConnection();

        /// <summary>
        /// App launch. Register a handler for when the app closes so we can neatly exit from Excel
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            //Connect to sqlite database
            // If anything breaks in here, it causes a gross XAML crash. Need to move it out of MainWindow Constructor.
            dbConnection = DatabaseConnector.InitializeDatabase();
            MatchedCategories = DatabaseConnector.LoadSavedCategories(dbConnection);

            // Hook to the application close event. No longer needed as we don't need to unhook from Excel anymore?
            //Application.Current.MainWindow.Closing += new System.ComponentModel.CancelEventHandler(AppClosing);
        }

        /// <summary>
        /// User clicked the 'Import' button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            //Prompts the user to select the file containing data
            string filepath = this.GetFilepath();

            // Pulls all transactions from the CSV file and formats
            Transactions = CSVProcessor.GetCSVTransactions(filepath).ToList();

            // Apply categories to descriptions which meet hardcoded rules
            List<Transaction> TransactionsTest = new List<Transaction>();


            ListViewTransactions.ItemsSource = Transactions;

            //Make things visible!
            Phase1StackPanel.Visibility = Visibility.Collapsed;
            ListViewTransactions.Visibility = Visibility.Visible;
            Phase2StackPanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Gets the filepath of the file the user selected
        /// </summary>
        /// <returns></returns>
        private string GetFilepath()
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

        /// <summary>
        /// Begins the analysis of the data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnalyse_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("How did you even click this? The button is disabled");
        }

        private void Categorise()
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

        private void btnCategorise_Click(object sender, RoutedEventArgs e)
        {
            ListViewTransactions.Visibility = Visibility.Collapsed;
            Phase2StackPanel.Visibility = Visibility.Collapsed;

            // Applies pre-defined rules against transactions to apply categories where possible
            List<Transaction> transactionsRuled = DefinedTransactionRules.ApplyDefinedTransactionRules(Transactions);

            Categorise();

            Phase4StackPanel.Visibility = Visibility.Visible;

            AskForCategory();
        }

        private void AskForCategory()
        {
            int remaining = Transactions.Where(t => string.IsNullOrEmpty(t.Category)).Select(t => t.Description).Distinct().Count();

            currentTransaction = Transactions.FirstOrDefault(t => string.IsNullOrEmpty(t.Category));
            if (currentTransaction == null)
            {
                //Phase 5                           
                Phase4SubmitButton.IsEnabled = false;
                Phase4SaveButton.Visibility = Visibility.Collapsed;
                Phase5ListView.Visibility = Visibility.Visible;

                Dictionary<string, decimal> categoryTotals = new Dictionary<string, decimal>();


                //// THIS IS WHERE WE WANT TO SPLIT CATEGORYTOTALS BY MONTH RATHER THAN JUST A LUMP SUM
                var months = Transactions.GroupBy(t => t.Date.Month);

                foreach (var category in MatchedCategories.Values.Distinct())
                {
                    //Totals the values associated with each category     
                    categoryTotals.Add(category, Transactions.Where(t => t.Category == category).Sum(t => t.Amount));                    

                }

                Phase5ListView.ItemsSource = categoryTotals;
                Phase5SaveButton.Visibility = Visibility.Visible;
                return;
            }
            Phase4Remaining.Text = "Unknown transactions remaining: " + remaining;
            Phase4Date.Text = "Date: " + currentTransaction.Date;
            Phase4Type.Text = "Type: " + currentTransaction.Type;
            Phase4Description.Text = "Description: " + currentTransaction.Description;
            Phase4Debit.Text = "Debit amount: " + currentTransaction.Debit.ToString();
            Phase4Credit.Text = "Credit amount: " + currentTransaction.Credit.ToString();
        }

        private void SubmitCategory_Click(object sender, RoutedEventArgs e)
        {
            string submittedCategory = CategoryInputBox.Text;

            string currentDescription = currentTransaction.Description;

            MatchedCategories.Add(currentDescription, submittedCategory);
            NewMatchedCategories.Add(currentDescription, submittedCategory);

            Phase4SaveButton.Content = "Save progress";
            Phase4SaveButton.IsEnabled = true;

            Categorise();
            AskForCategory();
        }

        /// <summary>
        /// Take MatchedCategories and save it in the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Phase5SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DatabaseConnector.SaveCategories(NewMatchedCategories, dbConnection);
            NewMatchedCategories.Clear();

            Phase5SaveButton.Content = "Successfully saved";
            Phase5SaveButton.IsEnabled = false;
        }

        private void Phase4SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DatabaseConnector.SaveCategories(NewMatchedCategories, dbConnection);
            NewMatchedCategories.Clear();

            Phase4SaveButton.Content = "Successfully saved";
            Phase4SaveButton.IsEnabled = false;
        }

        private void Phase4SubmitCategoryEnterKey(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SubmitCategory_Click(sender, e);
                CategoryInputBox.Clear();
            }            
        }
    }
}
