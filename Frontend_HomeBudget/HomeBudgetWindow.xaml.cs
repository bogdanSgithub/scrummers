using Budget;
using BudgetPresenter;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media.Imaging;

namespace Frontend_HomeBudget
{
    /// <summary>
    /// Logique d'interaction pour HomeBudget.xaml
    /// </summary>
    public partial class HomeBudgetWindow : Window
    {
        private IView _view;
        private int _selectedIndex;
        public HomeBudgetWindow(IView view)
        {
            InitializeComponent();
            _view = view;


            //display current used file
            CurrentFile.Text = $"Current File: {System.IO.Path.GetFileName(_view.Presenter.FilePath)}";
            Icon = new BitmapImage(new Uri("../../../assets/Image.png", UriKind.Relative));
        }

        private void RefreshFilter(object sender, RoutedEventArgs e)
        {
            int index = Categories.SelectedIndex;
            Categories.SelectionChanged -= RefreshFilter;
            
            _view.Presenter.ProcessRefreshBudgetItems(StartDate.SelectedDate, EndDate.SelectedDate, (bool)FilterCat.IsChecked, Categories.SelectedIndex + 1, (bool)ByMonth.IsChecked, (bool)ByCategory.IsChecked);

            Categories.SelectedIndex = index;
            Categories.SelectionChanged += RefreshFilter;

            if ((bool)ByMonth.IsChecked && (bool)ByCategory.IsChecked) 
                SwitchViewBtn.Visibility = Visibility.Visible;
            else
                SwitchViewBtn.Visibility = Visibility.Collapsed;

        }


        private void Button_AddExpense_Click(object sender, RoutedEventArgs e)
        {
            _view.ShowAddExpenseWindow();
            RefreshFilter(sender, new RoutedEventArgs());
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "CategoryID" || e.PropertyName == "ExpenseID" || e.PropertyName == "Details")
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

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult answer = MessageBox.Show("Do you really want to delete this expense?", "Delete Expense", MessageBoxButton.YesNo, MessageBoxImage.Stop);

            if (BudgetItems.SelectedItem is not null)
            {
                BudgetItem expenseToDelete = BudgetItems.SelectedItem as BudgetItem;

                bool result = answer == MessageBoxResult.Yes;

               _view.Presenter.ProcessDeleteExpense(expenseToDelete.ExpenseID, result);
            }

            RefreshFilter(sender, new RoutedEventArgs());
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _view.Presenter.ProcessRefreshBudgetItems(StartDate.SelectedDate, EndDate.SelectedDate, (bool)FilterCat.IsChecked, Categories.SelectedIndex + 1, (bool)ByMonth.IsChecked, (bool)ByCategory.IsChecked);
            if (SearchBox.Text == "")
                _selectedIndex = -1;
            _view.Presenter.ProcessSearch(SearchBox.Text, new ArrayList((ICollection)BudgetItems.ItemsSource), _selectedIndex);
        }

        private void BudgetItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BudgetItems.SelectedIndex != -1)
                _selectedIndex = BudgetItems.SelectedIndex;
        }
        private void SwitchDataView_Clicked(object sender, RoutedEventArgs e)
        {
            const int PieWidth = 1300;
            const int GridWidth = 800;

            if (BudgetItems.Visibility == Visibility.Visible)
            {
                BudgetItems.Visibility = Visibility.Hidden;
                FrontEndWindow.Width = PieWidth;
                FrontEndWindow.MaxWidth = PieWidth;
                FrontEndWindow.MinWidth = PieWidth;

                SwitchViewBtn.Content = "Switch To Grid View";

                _view.Presenter.ProcessRefreshPiechart(StartDate.SelectedDate, EndDate.SelectedDate, (bool)FilterCat.IsChecked, Categories.SelectedIndex + 1);

            }
            else
            {
                FrontEndWindow.Width = GridWidth;
                FrontEndWindow.MinWidth = GridWidth;
                FrontEndWindow.MaxWidth = GridWidth;
                BudgetItems.Visibility = Visibility.Visible;

                SwitchViewBtn.Content = "Switch To Piechart";
            }
        }
    }
}
