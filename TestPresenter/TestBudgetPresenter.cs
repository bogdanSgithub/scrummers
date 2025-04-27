using Budget;
using BudgetPresenter;
using System.Data.SQLite;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestPresenter
{
    public class TestBudgetPresenter
    {
        public string DbFilePath = "../../../newdb.db";
        // copy from TestProject
        private const string EXPENSE_ADDED_MESSAGE = "Showed Completion: Expense was succesfully added!";

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
            Assert.True(testView.Messages.Count == 2);
        }

        [Fact]
        public void Test_ProcessSelectedFile_Invalid()
        {
            TestView testView = new TestView(DbFilePath);

            testView.Presenter.ProcessSelectedFile("invalidFile.db");
        }

        [Fact]
        public void Test_GetPreviousFile()
        {
            TestView testView = new TestView(DbFilePath);

            testView.Presenter.ProcessSelectedFile("database.db");
            testView.Presenter.GetPreviousFile();
            Assert.Equal("database.db", testView.Presenter.FilePath);
            Assert.Equal("Showed AddExpenseWindow", testView.Messages[0]);
            Assert.Equal("Closed FileSelectWindow", testView.Messages[1]);
            Assert.True(testView.Messages.Count == 4);
        }
        /*
        [Fact]
        public void Test_IsFirstTimeUser()
        {
            TestView testView = new TestView(DbFilePath);
            File.Delete("../../../info.json");
            if (File.Exists("../../../info.json"))
            {
                File.Delete("../../../info.json");
            }
            Assert.True(testView.Presenter.IsFirstTimeUser());

            testView.Presenter.ProcessSelectedFile("database.db");

            Assert.False(testView.Presenter.IsFirstTimeUser());
        }*/

        [Fact]
        public void Test_GetCategoryTypes()
        {
            TestView testView = new TestView(DbFilePath);

            Category.CategoryType[] categoryTypes = testView.Presenter.GetCategoryTypes();

            Category.CategoryType[] VALID_CATEGORY_TYPES = [Category.CategoryType.Income, Category.CategoryType.Expense, Category.CategoryType.Credit, Category.CategoryType.Savings];
            Assert.Equal(VALID_CATEGORY_TYPES, categoryTypes);
        }
        

        [Fact]
        public void Test_ProcessAddExpense_Valid()
        {   
            DateTime date = DateTime.Now;
            int categoryId = 1;
            string amount = "5";
            string description = "test";

            TestView testView = new TestView(DbFilePath);
            testView.OpenFileDialog();
            testView.Presenter.ProcessAddExpense(date, categoryId, amount, description);
            Assert.Equal(EXPENSE_ADDED_MESSAGE, testView.Messages[3]);

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();
            Expense expense = expenses[^1];
            Assert.Equal(date, expense.Date);
            Assert.Equal(categoryId, expense.Category);
            Assert.Equal(double.Parse(amount), expense.Amount);
            Assert.Equal(date, expense.Date);

            homeBudget.expenses.Delete(expenses[^1].Id);
        }

        [Fact]
        public void Test_ProcessAddExpense_Invalid_CategoryId()
        {
            DateTime date = DateTime.Now;
            int categoryId = 500; // invalid categoryId
            string amount = "6";
            string description = "test";

            TestView testView = new TestView(DbFilePath);
            testView.OpenFileDialog();
            testView.Presenter.ProcessAddExpense(date, categoryId, amount, description);
            Assert.Equal("Showed Error: Error: Invalid categoryId.", testView.Messages[3]);

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();
            Assert.Equal(12, expenses.Count);
        }

        [Fact]
        public void Test_ProcessAddExpense_Invalid_Amount()
        {
            DateTime date = DateTime.Now;
            int categoryId = 1;
            string amount = "-5"; // invalid amount
            string description = "test";

            TestView testView = new TestView(DbFilePath);
            testView.OpenFileDialog();
            testView.Presenter.ProcessAddExpense(date, categoryId, amount, description);
            Assert.Equal("Showed Error: Amount must be a positive number", testView.Messages[3]);

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();
            Assert.Equal(12, expenses.Count);
        }

        [Fact]
        public void Test_ProcessAddCategory_Valid()
        {
            TestView testView = new TestView(DbFilePath);
            testView.OpenFileDialog();
            testView.Presenter.ProcessAddCategory(Category.CategoryType.Income, "plz work");
            Assert.Equal("Showed Completion: Category was successfully added.", testView.Messages[3]);

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Category> categories = homeBudget.categories.List();
            Category category = categories[^1];
            Assert.Equal("plz work", category.Description);
            Assert.Equal(Category.CategoryType.Income, category.Type);

            homeBudget.categories.Delete(categories[^1].Id);
        }
    }
}