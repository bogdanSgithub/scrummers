using Budget;
using BudgetPresenter;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Frontend_HomeBudget
{
    /// <summary>
    /// Logique d'interaction pour HomeBudget.xaml
    /// </summary>
    public partial class HomeBudgetWindow : Window
    {
        private IView _view;
        public HomeBudgetWindow(IView view)
        {
            InitializeComponent();
            _view = view;

            //display current used file
            CurrentFile.Text = $"Current File: {System.IO.Path.GetFileName(_view.Presenter.FilePath)}";
        }

        private void RefreshFilter(object sender, RoutedEventArgs e)
        {
            int index = Categories.SelectedIndex;
            Categories.SelectionChanged -= RefreshFilter;
            _view.Presenter.ProcessRefreshBudgetItems(StartDate.SelectedDate, EndDate.SelectedDate, (bool)FilterCat.IsChecked, Categories.SelectedIndex + 1, false, false);
            Categories.SelectedIndex = index;
            Categories.SelectionChanged += RefreshFilter;
        }


        private void Button_AddExpense_Click(object sender, RoutedEventArgs e)
        {
            _view.ShowAddExpenseWindow();
            RefreshFilter(sender, new RoutedEventArgs());
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "CategoryID" || e.PropertyName == "ExpenseID")
            {
                e.Cancel = true;
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            _view.Presenter.CloseApp();
        }

        private void UpdateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (BudgetItems.SelectedItem is not null)
            {
                _view.ShowUpdateExpenseWindow(BudgetItems.SelectedItem as BudgetItem);
            }

            RefreshFilter(sender, new RoutedEventArgs());
        }
    }
}
