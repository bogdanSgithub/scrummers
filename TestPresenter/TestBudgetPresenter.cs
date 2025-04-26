using Budget;
using BudgetPresenter;

namespace TestPresenter
{
    public class TestBudgetPresenter
    {
        public string DbFilePath = "C:\\Users\\Bogdan\\Source\\Repos\\scrummers\\BudgetSolution\\newdb.db";
        private const string EXPENSE_ADDED_MESSAGE = "Expense succesfully added";

        [Fact]
        public void Test_GetCategories()
        {
            TestView testView = new TestView(DbFilePath);
            testView.OpenFileDialog();
            HomeBudget hb = new HomeBudget(DbFilePath);

            Assert.Equal(hb.categories.List(), testView.Presenter.GetCategories());
        }

        [Fact]
        public void Test_StartApplication()
        {
            TestView testView = new TestView(DbFilePath);
            testView.Presenter.StartApplication();

            Assert.Equal("Showed FileSelectWindow", testView.Messages[0]);
            Assert.Single(testView.Messages);
        }

        [Fact]
        public void Test_ProcessSelectedFile_Valid()
        {
            TestView testView = new TestView(DbFilePath);
            testView.Presenter.ProcessSelectedFile(DbFilePath);

            Assert.Equal(DbFilePath, testView.Presenter.FilePath);
            Assert.True(File.Exists("../../../info.json"));
            Assert.Equal("Showed AddExpenseWindow", testView.Messages[0]);
            Assert.Equal("Closed FileSelectWindow", testView.Messages[1]);
        }

        [Fact]
        public void Test_ProcessSelectedFile_Invalid()
        {
            TestView testView = new TestView(DbFilePath);

            Assert.Equal(DbFilePath, testView.Presenter.FilePath);
            Assert.True(File.Exists("../../../info.json"));
            Assert.Equal("Showed AddExpenseWindow", testView.Messages[0]);
            Assert.Equal("Closed FileSelectWindow", testView.Messages[1]);
        }

        [Fact]
        public void Test_AddExpense_Valid()
        {   
            DateTime date = DateTime.Now;
            int categoryId = 1;
            string amount = "5";
            string description = "test";

            TestView testView = new TestView(DbFilePath);
            Presenter presenter = (Presenter)testView.Presenter;
            /*
            presenter.ProcessAddExpense(date, categoryId, amount, description);
            Assert.Equal(EXPENSE_ADDED_MESSAGE, testView.Messages[0]);
            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();
            Expense expense = expenses[^1];
            Assert.Equal(date, expense.Date);
            Assert.Equal(categoryId, expense.Category);
            Assert.Equal(double.Parse(amount), expense.Amount);
            Assert.Equal(date, expense.Date);
            */
        }
    }
}