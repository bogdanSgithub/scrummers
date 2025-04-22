using Budget;
using BudgetPresenter;

namespace TestPresenter
{
    public class TestBudgetPresenter
    {
        public string DbFilePath = "C:\\Users\\2276038\\Source\\Repos\\scrummers\\TestPresenter\\currentDatabase.db";
        private const string EXPENSE_ADDED_MESSAGE = "Expense succesfully added";


        [Fact]
        public void Test_AddExpense_Valid()
        {   
            DateTime date = DateTime.Now;
            int categoryId = 1;
            string amount = "5";
            string description = "test";

            TestView testView = new TestView(DbFilePath);
            Presenter presenter = testView.Presenter;
            presenter.AddExpense(date, categoryId, amount, description);
            Assert.Equal(EXPENSE_ADDED_MESSAGE, testView.Messages[0]);
            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();
            Expense expense = expenses[^1];
            Assert.Equal(date, expense.Date);
            Assert.Equal(categoryId, expense.Category);
            Assert.Equal(double.Parse(amount), expense.Amount);
            Assert.Equal(date, expense.Date);
        }
    }
}