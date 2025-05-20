using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetPresenter;
using System.Windows;
using System.Windows.Controls;
using Budget;
using Microsoft.Win32;
using System.Collections;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.Common;

namespace Frontend_HomeBudget
{
    class View : IView
    {

        /// <summary>
        /// IPresenter presenter, the view's reference to the presenter
        /// </summary>
        public IPresenter Presenter { get; }
        private GetFileWindow _fileWindow;
        private AddExpenseWindow _expenseWindow;
        private AddCategoryWindow _addCategoryWindow;
        private HomeBudgetWindow _homeBudgetWindow;
        private UpdateExpenseWindow _updateExpenseWindow;

        /// <summary>
        /// Constructor. Creates a Presenter, saves a reference of that presenter and starts the application.
        /// </summary>
        public View()
        {
            Presenter = new Presenter(this);
            Presenter.StartApplication();
        }

        /// <summary>
        /// Displays the FileSelectWindow
        /// </summary>
        public void ShowFileSelectWindow()
        {
            _fileWindow = new GetFileWindow(this);
            _fileWindow.Show();
        }

        /// <summary>
        /// Displays the AddExpenseWindow
        /// </summary>
        public void ShowAddExpenseWindow()
        {
            _expenseWindow = new AddExpenseWindow(this);
            Presenter.ProcessRefreshCategories();
            _expenseWindow.ShowDialog();
        }

        /// <summary>
        /// Closes the FileSelectWindow
        /// </summary>
        public void CloseFileSelectWindow()
        {
            _fileWindow.Close();
        }

        /// <summary>
        /// Displays the AddCategoryWindow
        /// </summary>
        public void ShowAddCategoryWindow()
        {
            _addCategoryWindow = new AddCategoryWindow(this);
            Presenter.ProcessRefreshCategoryTypes();
            _addCategoryWindow.ShowDialog();
        }

        public void ShowHomeBudgetWindow()
        {
            _homeBudgetWindow = new HomeBudgetWindow(this);
            Presenter.ProcessRefreshBudgetItems(null, null, false, 0, false, false);
            _homeBudgetWindow.Show();
        }


        /// <summary>
        /// Opens a FileDialog and once the user has selected a file, it calls the ProcessSelectedFile of the Presenter
        /// </summary>
        public void OpenFileDialog()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "DB files (*.db)|*.db";

            if (fileDialog.ShowDialog() == true)
            {
               Presenter.ProcessSelectedFile(fileDialog.FileName);
            }
        }

        /// <summary>
        /// Shows a good message box with the provided message
        /// </summary>
        /// <param name="message"></param>
        public void ShowCompletion(string message)
        {
            MessageBox.Show(message, "Succesfull", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Shows an error message box with the provided message
        /// </summary>
        /// <param name="message"></param>
        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Closes the Application
        /// </summary>
        public void CloseApp()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Closes the Add CategoryWindow
        /// </summary>
        public void CloseAddCategoryWindow()
        {
            _addCategoryWindow.Close();
        }

        public void RefreshBudgetItemsAndCategories(ArrayList budgetItems, ArrayList categories)
        {
            _homeBudgetWindow.BudgetItems.Columns.Clear();
            _homeBudgetWindow.BudgetItems.ItemsSource = budgetItems;
            _homeBudgetWindow.Categories.ItemsSource = categories;

            bool byMonth = (bool)_homeBudgetWindow.ByMonth.IsChecked;
            bool byCategory = (bool)_homeBudgetWindow.ByCategory.IsChecked;

            if(byMonth && byCategory) 
            {
                List<string> keys = new List<string>();

                foreach (Dictionary<string, object> dict in budgetItems)
                    foreach (string key in dict.Keys)
                        if (!keys.Contains(key))
                            keys.Add(key);

                _homeBudgetWindow.BudgetItems.Columns.Clear();

                DataGridTextColumn columnMonth = new DataGridTextColumn();
                columnMonth.Header = "Month";
                columnMonth.Binding = new Binding($"[{"Month"}]");
                _homeBudgetWindow.BudgetItems.Columns.Add(columnMonth);

               

                foreach (var category in categories)
                {
                    DataGridTextColumn column = new DataGridTextColumn();
                    string categoryName = ((Category)category).Description;
                    column.Header = categoryName;

                    if (keys.Contains(column.Header))
                    {
                        string key = categoryName;

                        column.Binding = new Binding($"[{key}]");

                        if (key != "Month" && key != "Date" && key != "Category" && key != "Short Description")
                            column.Binding.StringFormat = "{0:C}";

                        if (!key.StartsWith("details"))
                            _homeBudgetWindow.BudgetItems.Columns.Add(column);
                    }
                    else
                    {
                        _homeBudgetWindow.BudgetItems.Columns.Add(column);
                    }
                }

                DataGridTextColumn columnTotal = new DataGridTextColumn();
                columnTotal.Header = "Total";
                columnTotal.Binding = new Binding($"[{"Total"}]");
                _homeBudgetWindow.BudgetItems.Columns.Add(columnTotal);
            }
        }

        public void RefreshCategories(ArrayList categories)
        {
            if (_expenseWindow is not null)
            {
                _expenseWindow.Categorys.ItemsSource = categories;
                _expenseWindow.Categorys.SelectedIndex = 0;
            }

            if (_updateExpenseWindow is not null)
            {
                categories.RemoveAt(categories.Count - 1);
                _updateExpenseWindow.Categories.ItemsSource = categories;
                _updateExpenseWindow.Categories.SelectedIndex = 0;
            }
        }

        public void RefreshCategoryTypes(ArrayList categories)
        {
            _addCategoryWindow.CategoryTypes.ItemsSource = categories;
        }

        public void ShowUpdateExpenseWindow(BudgetItem item)
        {
           _updateExpenseWindow = new UpdateExpenseWindow(this, item);
            Presenter.ProcessRefreshCategories();
            _updateExpenseWindow.Categories.SelectedIndex = item.CategoryID - 1;
           _updateExpenseWindow.ShowDialog();
        }

        public void CloseUpdateExpenseWindow()
        {
            _updateExpenseWindow.Close();
        }

        public void RefreshPiechart(List<Dictionary<string, object>> budgetItems, List<string> categories)
        {
            _homeBudgetWindow.DataPieChart.InitializeByCategoryAndMonthDisplay(categories);
            _homeBudgetWindow.DataPieChart.DataSource = budgetItems; 
        }
    }
}
