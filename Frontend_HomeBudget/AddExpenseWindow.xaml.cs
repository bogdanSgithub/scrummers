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
            //initialize view
            _view = view;            
            
            //display current used file
            CurrentFile.Text = $"Current File {System.IO.Path.GetFileName(_view.Presenter.FilePath)}";
            
            //set values inside of combobox on first load
            RefreshCategories();
        }


        /// <summary>
        /// Get all categories from presenter to set them into the combobox. Also adds the add category option.
        /// </summary>
        private void RefreshCategories()
        {
            _categories = _view.Presenter.GetCategories();
            Category AddCategoryItem = new Category(-1, "+ Add Category");
            _categories.Add(AddCategoryItem);


            Categorys.ItemsSource = _categories;
            Categorys.SelectedIndex = 0;

        }

        /// <summary>
        /// Adds an expense to the database on button click. Uses values from the two text boxes, combobox and the date picker.
        /// </summary>
        private void Button_AddExpense_Click(object sender, RoutedEventArgs e)
        {

            if (Date.SelectedDate is null)
            {
                _view.ShowError("Must choose a date");
                return;
            }

            DateTime date = (DateTime)Date.SelectedDate;
            Category category = _categories[Categorys.SelectedIndex];
            _view.Presenter.ProcessAddExpense(date, category.Id, Amount.Text, Description.Text);
        }


        /// <summary>
        /// Resets all of the values in the form to their defaults on button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            Date.SelectedDate = null;
            Categorys.SelectedIndex = 0;
            Amount.Text = "Amount";
            Description.Text = "Description";
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            _view.Presenter.CloseApp();
        }

        /// <summary>
        /// Checks if the add expense option was clicked, if it was display the add category window and refresh the categories on add.
        /// </summary>
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
