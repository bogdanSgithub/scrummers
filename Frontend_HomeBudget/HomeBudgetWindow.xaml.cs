using BudgetPresenter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        }

        private void RefreshFilter(object sender, RoutedEventArgs e)
        {
            int index = Categories.SelectedIndex;
            Categories.SelectionChanged -= RefreshFilter;
            bool byMonth = ByMonth.IsChecked ?? false;
            bool byCategory = ByCategory.IsChecked ?? false;

            if (byMonth && byCategory)
            {
                if (BudgetItems.ItemsSource is List<Dictionary<string, object>> ListDict)
                {
                    foreach (string key in ListDict[0].Keys)
                    {
                        var column = new DataGridTextColumn();
                        column.Header = key;
                        column.Binding = new Binding($"[{key}]"); // Notice the square brackets!
                        BudgetItems.Columns.Add(column);
                    }
                }
            }

            _view.Presenter.ProcessRefreshBudgetItems(StartDate.SelectedDate, EndDate.SelectedDate, (bool)FilterCat.IsChecked, Categories.SelectedIndex + 1, byMonth, byCategory);

            

            Categories.SelectedIndex = index;
            Categories.SelectionChanged += RefreshFilter;
        }


        private void Button_AddExpense_Click(object sender, RoutedEventArgs e)
        {
            _view.ShowAddExpenseWindow();
            _view.Presenter.ProcessRefreshBudgetItems(null, null, false, 0, false, false);
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
    }
}
