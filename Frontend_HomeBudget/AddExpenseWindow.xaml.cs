using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BudgetPresenter;

namespace Frontend_HomeBudget
{
    /// <summary>
    /// Interaction logic for AddExpense.xaml
    /// </summary>
    public partial class AddExpenseWindow : Window
    {
        private IView _view;

        public AddExpenseWindow(IView view)
        {
            InitializeComponent();
            _view = view;
            Icon = new BitmapImage(new Uri("../../../assets/Image.png", UriKind.Relative));
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
            _view.Presenter.ProcessAddExpense(date, Categorys.SelectedIndex + 1, Amount.Text, Description.Text);
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
            Close();
        }

        /// <summary>
        /// Checks if the add expense option was clicked, if it was display the add category window and refresh the categories on add.
        /// </summary>
        private void Categorys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _view.Presenter.ProcessCategorySelection(Categorys.SelectedIndex);
        }
    }
}
