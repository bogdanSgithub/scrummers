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

namespace Frontend_HomeBudget
{
    /// <summary>
    /// Interaction logic for AddExpense.xaml
    /// </summary>
    public partial class AddExpense : Window, IView
    {
        private IPresenter _presenter;
        private List<Category> _categories;

        public AddExpense(string filepath)
        {
            InitializeComponent();
            _presenter = new Presenter(filepath, this);
            Category AddCategoryItem = new Category(-1, "+ Add Category");

            _categories = _presenter.GetCategories();
            _categories.Add(AddCategoryItem);

            Categorys.ItemsSource = _categories;
            Categorys.SelectedIndex = 0;
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowCompletion(string message)
        {
            MessageBox.Show(message, "Succesfull", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button_AddExpense_Click(object sender, RoutedEventArgs e)
        {   
            if (Date.SelectedDate is null)
            {
                ShowError("Must choose a date");
                return;
            }

            DateTime date = (DateTime)Date.SelectedDate;
            Category category = _categories[Categorys.SelectedIndex];

            _presenter.AddExpense(date, category.Id, Amount.Text, Description.Text);
        }

        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            Date.SelectedDate = null;
            Categorys.SelectedIndex = 0;
            Amount.Text = "Amount";
            Description.Text = "Description";
        }
    }
}
