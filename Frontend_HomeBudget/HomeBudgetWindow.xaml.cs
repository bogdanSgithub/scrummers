using BudgetPresenter;
using System;
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
            Categories.ItemsSource = _view.Presenter.GetCategories();
            RefreshBudgetItems();
        }

        private void RefreshBudgetItems()
        {
            BudgetItems.ItemsSource = _view.Presenter.GetBudgetItems(StartDate.SelectedDate, EndDate.SelectedDate, (bool)FilterCat.IsChecked, Categories.SelectedIndex + 1, false, false);
        }

        private void RefreshFilter(object sender, RoutedEventArgs e)
        {
            RefreshBudgetItems();
        }


        private void Button_AddExpense_Click(object sender, RoutedEventArgs e)
        {
            _view.ShowAddExpenseWindow();
            RefreshBudgetItems();
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
