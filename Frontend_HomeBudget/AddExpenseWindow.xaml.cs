using System;
using System.Collections.Generic;
using System.Globalization;
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
using Budget;
using BudgetPresenter;
using System.IO;

namespace Frontend_HomeBudget
{
    /// <summary>
    /// Interaction logic for AddExpense.xaml
    /// </summary>
    public partial class AddExpenseWindow : Window
    {
        private List<Category> _categories;
        private IView _view;

        public AddExpenseWindow(IView view)
        {
            InitializeComponent();
            _view = view;

            CurrentFile.Text = $"Current File {System.IO.Path.GetFileName(_view.presenter.FilePath)}";


            RefreshCategories();

        }

        private void RefreshCategories()
        {
            _categories = _view.presenter.GetCategories();
            Category AddCategoryItem = new Category(-1, "+ Add Category");
            _categories.Add(AddCategoryItem);


            Categorys.ItemsSource = _categories;
            Categorys.SelectedIndex = 0;

        }

        private void Button_AddExpense_Click(object sender, RoutedEventArgs e)
        {

            if (Date.SelectedDate is null)
            {
                _view.ShowError("Must choose a date");
                return;
            }

            DateTime date = (DateTime)Date.SelectedDate;
            Category category = _categories[Categorys.SelectedIndex];
            _view.presenter.ProcessAddExpense(date, category.Id, Amount.Text, Description.Text);
        }

        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            Date.SelectedDate = null;
            Categorys.SelectedIndex = 0;
            Amount.Text = "Amount";
            Description.Text = "Description";
        }


        private void Categorys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Categorys.SelectedIndex == _categories.Count - 1)
            {
                _view.ShowAddCategoryWindow();
                RefreshCategories();
            }
        }
    }
}
