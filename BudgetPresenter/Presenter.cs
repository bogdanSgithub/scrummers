using Budget;

namespace BudgetPresenter
{
    public class Presenter : IPresenter
    {
        private HomeBudget _homeBudget;
        private IView _view;

        public IView View
        { 
            get { return _view; } 
        }

        public Presenter(string filepath, IView view)
        {
            _homeBudget = new HomeBudget(filepath);
            _view = view;
        }

        public void ValidateAndAddExpense(DateTime dateInput, int categoryInput, string amountInput, string descriptionInput)
        {
            double amount;
            if (!(double.TryParse(amountInput, out amount) && amount >= 0))
            {
                View.Alert("Amount must not be negative");
            }

            try
            {
                _homeBudget.expenses.Add(dateInput, categoryInput, amount, descriptionInput);
            }
            catch (Exception ex)
            {
                View.Alert("An error occured while adding the expense");
            }
        }
    }
}
