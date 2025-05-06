using Budget;
using BudgetPresenter;
using System.Collections;

namespace TestPresenter
{
    public class TestBudgetPresenter
    {
        public string DbFilePath = "../../../newdb.db";
        // copy from TestProject the newdb.db
        private const string EXPENSE_ADDED_MESSAGE = "Showed Completion: Expense was succesfully added!";
        private const string CATEGORY_ADDED_ERROR_MESSAGE = "Showed Error: Description cannot be empty.";
        private const int NB_CATEGORIES = 16;
        private const int NB_EXPENSES = 0;
        private string INFO_JSON_PATH = "../../../info.json";

        [Fact]
        public void Test_IsFirstTimeUser()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            if (File.Exists(INFO_JSON_PATH))
            {
                File.Delete(INFO_JSON_PATH);
            }

            // Assert
            Assert.True(testView.Presenter.IsFirstTimeUser());

            testView.Presenter.ProcessSelectedFile("database.db");
            Assert.False(testView.Presenter.IsFirstTimeUser());
        }

        [Fact]
        public void Test_StartApplication()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.Presenter.StartApplication();

            // Assert
            Assert.Equal("Showed FileSelectWindow", testView.Messages[0]);
            Assert.Single(testView.Messages);
        }

        [Fact]
        public void Test_ProcessSelectedFile_Valid()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.Presenter.ProcessSelectedFile(DbFilePath);

            // Assert
            Assert.Equal(DbFilePath, testView.Presenter.FilePath);
            Assert.True(File.Exists(INFO_JSON_PATH));
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[0]);
            Assert.Equal("Closed FileSelectWindow", testView.Messages[1]);
            Assert.True(testView.Messages.Count == 2);
        }


        [Fact]
        public void Test_ProcessSelectedFile_Invalid_Null()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.Presenter.ProcessSelectedFile(null);

            // Assert
            Assert.Equal("Showed Error: invalid filename, cannot be null or empty.", testView.Messages[0]);
        }

        [Fact]
        public void Test_ProcessSelectedFile_Invalid_EMPTY()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.Presenter.ProcessSelectedFile("");

            // Assert
            Assert.Equal("Showed Error: invalid filename, cannot be null or empty.", testView.Messages[0]);
        }

        [Fact]
        public void Test_GetPreviousFile()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.Presenter.GetPreviousFile();

            // Assert
            Assert.True(File.Exists(INFO_JSON_PATH));
            Assert.Equal(DbFilePath, testView.Presenter.FilePath);
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[0]);
            Assert.Equal("Closed FileSelectWindow", testView.Messages[1]);
        }

        [Fact]
        public void Test_Close()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.Presenter.CloseApp();

            // Assert
            Assert.Equal("Closed App", testView.Messages[0]);
        }

        [Fact]
        public void Test_ProcessAddExpense_Valid()
        {
            // Arrange
            DateTime date = DateTime.Now;
            int categoryId = 1;
            string amount = "5";
            string description = "test";

            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();
            testView.Presenter.ProcessAddExpense(date, categoryId, amount, description);

            // Assert
            Assert.Equal(EXPENSE_ADDED_MESSAGE, testView.Messages[3]);

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();
            Expense expense = expenses[^1];
            Assert.Equal(date, expense.Date);
            Assert.Equal(categoryId, expense.Category);
            Assert.Equal(double.Parse(amount), expense.Amount);
            Assert.Equal(date, expense.Date);

            homeBudget.expenses.Delete(expenses[^1].Id);
            expenses = homeBudget.expenses.List();
            Assert.Equal(NB_EXPENSES, expenses.Count);
        }

        [Fact]
        public void Test_ProcessAddExpense_Invalid_CategoryId()
        {
            // Arrange
            DateTime date = DateTime.Now;
            int categoryId = 500; // invalid categoryId
            string amount = "6";
            string description = "test";

            // Act
            TestView testView = new TestView(DbFilePath);
            testView.OpenFileDialog();
            testView.Presenter.ProcessAddExpense(date, categoryId, amount, description);
            
            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();

            // Assert
            Assert.Equal("Showed Error: Error: Invalid categoryId.", testView.Messages[3]);
            Assert.Equal(NB_EXPENSES, expenses.Count);
        }

        [Fact]
        public void Test_ProcessAddExpense_Invalid_Amount()
        {
            // Arrange
            DateTime date = DateTime.Now;
            int categoryId = 1;
            string amount = "-5"; // invalid amount
            string description = "test";

            TestView testView = new TestView(DbFilePath);
            
            // Act
            testView.OpenFileDialog();
            testView.Presenter.ProcessAddExpense(date, categoryId, amount, description);

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();

            // Assert
            Assert.Equal("Showed Error: Amount must be a positive number", testView.Messages[3]);
            Assert.Equal(NB_EXPENSES, expenses.Count);
        }

        [Fact]
        public void Test_ProcessAddExpense_Invalid_Amount_Zero()
        {
            // Arrange
            DateTime date = DateTime.Now;
            int categoryId = 1;
            string amount = "0"; // invalid amount
            string description = "test";

            TestView testView = new TestView(DbFilePath);
            
            // Act
            testView.OpenFileDialog();
            testView.Presenter.ProcessAddExpense(date, categoryId, amount, description);

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();

            // Assert
            Assert.Equal("Showed Error: Amount must be a positive number", testView.Messages[3]);
            Assert.Equal(NB_EXPENSES, expenses.Count);
        }

        [Fact]
        public void Test_ProcessAddExpense_Invalid_Amount_NotNumber()
        {
            // Arrange
            DateTime date = DateTime.Now;
            int categoryId = 1;
            string amount = "ya"; // invalid amount
            string description = "test";

            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();
            testView.Presenter.ProcessAddExpense(date, categoryId, amount, description);

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();

            // Assert
            Assert.Equal("Showed Error: Amount must be a positive number", testView.Messages[3]);
            Assert.Equal(NB_EXPENSES, expenses.Count);
        }

        [Fact]
        public void Test_ProcessAddCategory_Invalid_Description()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);
            testView.OpenFileDialog();
            testView.Presenter.ProcessAddCategory(Category.CategoryType.Income, "");

            // Act
            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Category> categories = homeBudget.categories.List();

            // Assert
            Assert.Equal(NB_CATEGORIES, categories.Count);
            Assert.Equal(CATEGORY_ADDED_ERROR_MESSAGE, testView.Messages[3]);
        }

        [Fact]
        public void Test_ProcessAddCategory_Valid()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();
            testView.Presenter.ProcessAddCategory(Category.CategoryType.Income, "plz work");
            
            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Category> categories = homeBudget.categories.List();
            Category category = categories[^1];

            // Assert
            Assert.Equal("Showed Completion: Category was successfully added.", testView.Messages[3]);
            Assert.Equal("plz work", category.Description);
            Assert.Equal(Category.CategoryType.Income, category.Type);

            homeBudget.categories.Delete(category.Id);
            categories = homeBudget.categories.List();
            Assert.Equal(NB_CATEGORIES, categories.Count);
        }

        [Fact]
        public void Test_ProcessRefreshBudgetItems_Normal()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();
            testView.Presenter.ProcessRefreshBudgetItems(null, null, false, 0, false, false);

            HomeBudget hb = new HomeBudget(DbFilePath);
            ArrayList budgetItems = new ArrayList(hb.GetBudgetItems(null, null, false, 0));

            // Assert
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Refresh the budget items and categories", testView.Messages[3]);
            Assert.Equal(budgetItems, testView.BudgetItems);
        }

        [Fact]
        public void Test_ProcessRefreshBudgetItems_ByMonth()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();
            testView.Presenter.ProcessRefreshBudgetItems(null, null, false, 0, true, false);

            HomeBudget hb = new HomeBudget(DbFilePath);
            ArrayList budgetItems = new ArrayList(hb.GetBudgetItemsByMonth(null, null, false, 0));

            // Assert
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Refresh the budget items and categories", testView.Messages[3]);
            Assert.Equal(budgetItems, testView.BudgetItems);
        }

        [Fact]
        public void Test_ProcessRefreshBudgetItems_ByCategory()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();
            testView.Presenter.ProcessRefreshBudgetItems(null, null, false, 0, false, true);

            HomeBudget hb = new HomeBudget(DbFilePath);
            ArrayList budgetItems = new ArrayList(hb.GetBudgetItemsByCategory(null, null, false, 0));

            // Assert
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Refresh the budget items and categories", testView.Messages[3]);
            Assert.Equal(budgetItems, testView.BudgetItems);
        }

        [Fact]
        public void Test_ProcessRefreshBudgetItems_ByCategoryAndMonth()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();
            testView.Presenter.ProcessRefreshBudgetItems(null, null, false, 0, true, true);

            HomeBudget hb = new HomeBudget(DbFilePath);
            ArrayList budgetItems = new ArrayList(hb.GetBudgetDictionaryByCategoryAndMonth(null, null, false, 0));

            // Assert
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Refresh the budget items and categories", testView.Messages[3]);
            Assert.Equal(budgetItems, testView.BudgetItems);
        }

        [Fact]
        public void Test_ProcessRefreshCategories()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();
            testView.Presenter.ProcessRefreshCategories();

            HomeBudget hb = new HomeBudget(DbFilePath);
            ArrayList categories = new ArrayList(hb.categories.List());
            Category addCategory = (Category) testView.Categories[testView.Categories.Count - 1]; // the last one is the addCategoryButton
            testView.Categories.RemoveAt(testView.Categories.Count - 1); // remove the last element


            // Assert
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Refresh the categories", testView.Messages[3]);
            
            Assert.Equal(categories.Count, testView.Categories.Count);
            Assert.Equal(-1, addCategory.Id);
            Assert.Equal("+ Add Category", addCategory.Description.Trim());
        }

        [Fact]
        public void Test_ProcessRefreshCategoryTypes()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();
            testView.Presenter.ProcessRefreshCategoryTypes();

            HomeBudget hb = new HomeBudget(DbFilePath);
            ArrayList categoryTypes = new ArrayList((Category.CategoryType[])Enum.GetValues(typeof(Category.CategoryType)));

            // Assert
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Refresh the category types", testView.Messages[3]);

            Assert.Equal(categoryTypes.Count, testView.CategoryTypes.Count);
            Assert.Equal(categoryTypes, testView.CategoryTypes);
        }

        [Fact]
        public void Test_ProcessCategorySelection()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();
            testView.Presenter.ProcessRefreshCategories();
            testView.Presenter.ProcessCategorySelection(testView.Categories.Count-1);

            HomeBudget hb = new HomeBudget(DbFilePath);
            ArrayList categoryTypes = new ArrayList((Category.CategoryType[])Enum.GetValues(typeof(Category.CategoryType)));

            // Assert
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Refresh the categories", testView.Messages[3]);
            Assert.Equal("Showed AddCategoryWindow", testView.Messages[4]);
            Assert.Equal("Refresh the categories", testView.Messages[5]);
        }
    }
}