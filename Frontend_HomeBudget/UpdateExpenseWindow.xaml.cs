using Budget;
using BudgetPresenter;
using System.Windows;

namespace Frontend_HomeBudget
{
    /// <summary>
    /// Interaction logic for UpdateExpenseWindow.xaml
    /// </summary>
    public partial class UpdateExpenseWindow : Window
    {
        private IView _view;
        private BudgetItem _item;

        public UpdateExpenseWindow(IView view, BudgetItem item)
        {
            InitializeComponent();

            _view = view;
            _item = item;

            newDate.SelectedDate = item.Date;
            newDesc.Text = item.ShortDescription;
            newAmount.Text = item.Amount.ToString();
        }

        private void UpdateButton_Clicked(object sender, RoutedEventArgs e)
        {
            _view.Presenter.ProcessUpdateExpense(_item.ExpenseID, newDate.SelectedDate, Categories.SelectedIndex, newAmount.Text, newDesc.Text);

            _view.CloseUpdateExpenseWindow();
        }

        private void CloseButton_Clicked(object sender, RoutedEventArgs e)
        {
            _view.CloseUpdateExpenseWindow();
        }

    }
}
