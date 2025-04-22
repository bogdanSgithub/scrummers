using Budget;
using System.Text;
using System.Threading.Tasks;
using BudgetPresenter;
using System.Windows;
using Budget;


namespace BudgetPresenter
{
    public class Presenter : IPresenter
    {
        public List<Category> GetCategories()
        {
            return _homeBudget.categories.List();
        }

        private HomeBudget _homeBudget;
        private IView _view;

        public Presenter(string filepath, IView view)
        {
            _homeBudget = new HomeBudget(filepath);
            _view = view;
        }

        public void StartProgram()
        {
            _view.ShowFileSelectWindow();
        }

        public void AddExpense(DateTime dateInput, int categoryInput, string amountInput, string descriptionInput)
        {
            double amount;
            if (!(double.TryParse(amountInput, out amount) && amount >= 0))
            {
                _view.ShowError("Amount must be a non negative number");
                return;
            }

            try
            {
                _homeBudget.expenses.Add(dateInput, categoryInput, amount, descriptionInput);
                _view.ShowCompletion("Expense was succesfully added!");
            }
            catch (Exception ex)
            {
                _view.ShowError("Error: " + ex.Message);
            }
        }
    }
}
