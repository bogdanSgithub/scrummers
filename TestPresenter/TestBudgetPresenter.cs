using Budget;
using BudgetPresenter;
using System.Collections;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private string INFO_JSON_PATH = "C:\\Users\\Public\\info.json";

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
            testView.Presenter.ProcessAddCategory((int)Category.CategoryType.Income, "");

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
            testView.Presenter.ProcessAddCategory((int)Category.CategoryType.Income, "plz work");
            
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

            testView.Presenter.ProcessAddExpense(DateTime.Now, 1, "5", "expense");

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();

            testView.Presenter.ProcessRefreshBudgetItems(null, null, false, 0, false, false);
            ArrayList budgetItems = new ArrayList(homeBudget.GetBudgetItems(null, null, false, 0));

            // Assert
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Refresh the budget items and categories", testView.Messages[4]);

            for (int i = 0; i < budgetItems.Count; i++)
            {
                BudgetItem goodOne = (BudgetItem)budgetItems[i];
                BudgetItem toTest = (BudgetItem)testView.BudgetItems[i];
                Assert.Equal(goodOne.Balance, toTest.Balance);
                Assert.Equal(goodOne.Date, toTest.Date);
                Assert.Equal(goodOne.Amount, toTest.Amount);
                Assert.Equal(goodOne.Category, toTest.Category);
                Assert.Equal(goodOne.ShortDescription, toTest.ShortDescription);
            }

            homeBudget.expenses.Delete(expenses[^1].Id); // gotta delete it cause we are using the same database and expect there to be nothing in there
        }

        [Fact]
        public void Test_ProcessRefreshBudgetItems_ByMonth()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();

            testView.Presenter.ProcessAddExpense(DateTime.Now, 1, "5", "expense");

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();

            testView.Presenter.ProcessRefreshBudgetItems(null, null, false, 0, true, false);
            ArrayList budgetItems = new ArrayList(homeBudget.GetBudgetItemsByMonth(null, null, false, 0));

            // Assert
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Refresh the budget items and categories", testView.Messages[4]);

            /*Assert.True(budgetItems.Cast<object>().OrderBy(x => x)
                    .SequenceEqual(testView.BudgetItems.Cast<object>().OrderBy(x => x)));
            */
            for (int i = 0; i < budgetItems.Count; i++)
            {
                BudgetItemsByMonth goodOne = (BudgetItemsByMonth)budgetItems[i];
                BudgetItemsByMonth toTest = (BudgetItemsByMonth)testView.BudgetItems[i];
                Assert.Equal(goodOne.Total, toTest.Total);
                Assert.Equal(goodOne.Month, toTest.Month);
            }
            homeBudget.expenses.Delete(expenses[^1].Id); // gotta delete it cause we are using the same database and expect there to be nothing in there
        }

        [Fact]
        public void Test_ProcessRefreshBudgetItems_ByCategory()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();

            testView.Presenter.ProcessAddExpense(DateTime.Now, 1, "5", "expense");

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();

            testView.Presenter.ProcessRefreshBudgetItems(null, null, false, 0, false, true);
            ArrayList budgetItems = new ArrayList(homeBudget.GetBudgetItemsByCategory(null, null, false, 0));

            // Assert
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Refresh the budget items and categories", testView.Messages[4]);

            for (int i = 0; i < budgetItems.Count; i++)
            {
                BudgetItemsByCategory goodOne = (BudgetItemsByCategory)budgetItems[i];
                BudgetItemsByCategory toTest = (BudgetItemsByCategory)testView.BudgetItems[i];
                Assert.Equal(goodOne.Total, toTest.Total);
                Assert.Equal(goodOne.Category, toTest.Category);
            }
            homeBudget.expenses.Delete(expenses[^1].Id); // gotta delete it cause we are using the same database and expect there to be nothing in there
        }

        [Fact]
        public void Test_ProcessRefreshBudgetItems_ByCategoryAndMonth()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();

            testView.Presenter.ProcessAddExpense(DateTime.Now, 1, "5", "expense");

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();

            testView.Presenter.ProcessRefreshBudgetItems(null, null, false, 0, true, true);
            ArrayList budgetItems = new ArrayList(homeBudget.GetBudgetDictionaryByCategoryAndMonth(null, null, false, 0));

            // Assert
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Refresh the budget items and categories", testView.Messages[4]);

            homeBudget.expenses.Delete(expenses[^1].Id); // gotta delete it cause we are using the same database and expect there to be nothing in there
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

        [Fact]
        public void Test_ProcessUpdateExpense_Valid()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            int EXPENSE_ID = 1;
            DateTime newDate = new DateTime(2000, 01, 01);
            string newAmount = "10";
            string newDescription = "newExpense";
            int newCategory = 2;


            // Act
            testView.OpenFileDialog();
            testView.Presenter.ProcessAddExpense(DateTime.Now, 1, "5", "expense");

            testView.Presenter.ProcessUpdateExpense(EXPENSE_ID, newDate, newCategory, newAmount, newDescription);

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();
            Expense expense = expenses[^1]; // the updated expense must be what we passed to it

            homeBudget.expenses.Delete(expenses[^1].Id); // gotta delete it cause we are using the same database and expect there to be nothing in there
            expenses = homeBudget.expenses.List();

            // Assert
            Assert.Equal(NB_EXPENSES, expenses.Count);
            Assert.Equal(newDate, expense.Date);
            Assert.Equal(newCategory + 1, expense.Category);
            Assert.Equal(double.Parse(newAmount), expense.Amount);
            Assert.Equal(newDescription, expense.Description);
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal(EXPENSE_ADDED_MESSAGE, testView.Messages[3]);
            Assert.Equal("Showed Completion: Expense was succesfully updated.", testView.Messages[4]);
        }

        [Fact]
        public void Test_ProcessUpdateExpense_InvalidDescription()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            int EXPENSE_ID = 1;
            DateTime newDate = new DateTime(2000, 01, 01);
            string newAmount = "10";
            string newDescription = ""; // invalid description
            int newCategory = 2;


            // Act
            testView.OpenFileDialog();
            testView.Presenter.ProcessAddExpense(DateTime.Now, 1, "5", "expense");

            testView.Presenter.ProcessUpdateExpense(EXPENSE_ID, newDate, newCategory, newAmount, newDescription);

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();
            Expense expense = expenses[^1]; // the updated expense must be what we passed to it

            homeBudget.expenses.Delete(expenses[^1].Id); // gotta delete it cause we are using the same database and expect there to be nothing in there
            expenses = homeBudget.expenses.List();

            // Assert
            Assert.Equal(NB_EXPENSES, expenses.Count);
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal(EXPENSE_ADDED_MESSAGE, testView.Messages[3]);
            Assert.Equal("Showed Error: New description cannot be null.", testView.Messages[4]);
        }

        [Fact]
        public void Test_ProcessUpdateExpense_InvalidDate()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            int EXPENSE_ID = 1;
            DateTime? newDate = null; // invalid date
            string newAmount = "10";
            string newDescription = "hello"; 
            int newCategory = 2;


            // Act
            testView.OpenFileDialog();
            testView.Presenter.ProcessAddExpense(DateTime.Now, 1, "5", "expense");

            testView.Presenter.ProcessUpdateExpense(EXPENSE_ID, newDate, newCategory, newAmount, newDescription);

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();
            Expense expense = expenses[^1]; // the updated expense must be what we passed to it

            homeBudget.expenses.Delete(expenses[^1].Id); // gotta delete it cause we are using the same database and expect there to be nothing in there
            expenses = homeBudget.expenses.List();

            // Assert
            Assert.Equal(NB_EXPENSES, expenses.Count);
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal(EXPENSE_ADDED_MESSAGE, testView.Messages[3]);
            Assert.Equal("Showed Error: New date cannot be null.", testView.Messages[4]);
        }

        [Fact]
        public void Test_ProcessUpdateExpense_InvalidAmount()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            int EXPENSE_ID = 1;
            DateTime? newDate = new DateTime(2000, 01, 01);
            string newAmount = "bla"; // invalid amount
            string newDescription = "hello";
            int newCategory = 2;


            // Act
            testView.OpenFileDialog();
            testView.Presenter.ProcessAddExpense(DateTime.Now, 1, "5", "expense");

            testView.Presenter.ProcessUpdateExpense(EXPENSE_ID, newDate, newCategory, newAmount, newDescription);

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();
            Expense expense = expenses[^1]; // the updated expense must be what we passed to it

            homeBudget.expenses.Delete(expenses[^1].Id); // gotta delete it cause we are using the same database and expect there to be nothing in there
            expenses = homeBudget.expenses.List();

            // Assert
            Assert.Equal(NB_EXPENSES, expenses.Count);
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal(EXPENSE_ADDED_MESSAGE, testView.Messages[3]);
            Assert.Equal("Showed Error: Amount must be a positive number", testView.Messages[4]);
        }

        [Fact]
        public void Test_ProcessUpdateExpense_InvalidNewCategory()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            int EXPENSE_ID = 1;
            DateTime? newDate = new DateTime(2000, 01, 01);
            string newAmount = "5"; // invalid amount
            string newDescription = "hello";
            int newCategory = 1000; // we dont have 1000 categories


            // Act
            testView.OpenFileDialog();
            testView.Presenter.ProcessAddExpense(DateTime.Now, 1, "5", "expense");

            testView.Presenter.ProcessUpdateExpense(EXPENSE_ID, newDate, newCategory, newAmount, newDescription);

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();
            Expense expense = expenses[^1]; // the updated expense must be what we passed to it

            homeBudget.expenses.Delete(expenses[^1].Id); // gotta delete it cause we are using the same database and expect there to be nothing in there
            expenses = homeBudget.expenses.List();

            // Assert
            Assert.Equal(NB_EXPENSES, expenses.Count);
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal(EXPENSE_ADDED_MESSAGE, testView.Messages[3]);
            Assert.Equal("Showed Error: Error: Invalid categoryId.", testView.Messages[4]);
        }

        [Fact]
        public void Test_ProcessSearch_ValidDescription()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();

            testView.Presenter.ProcessAddExpense(DateTime.Now, 1, "5", "expense");

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();

            
            ArrayList budgetItems = new ArrayList(homeBudget.GetBudgetItems(null, null, false, 0));
            //budgetItems

            testView.Presenter.ProcessSearch("e", budgetItems, 0); // this refreshes the budget items
            
            // Assert
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Refresh the budget items and categories", testView.Messages[4]);
            
            for (int i = 0; i < budgetItems.Count; i++)
            {
                BudgetItem goodOne = (BudgetItem)budgetItems[i];
                BudgetItem toTest = (BudgetItem)testView.BudgetItems[i];
                Assert.Equal(goodOne.Balance, toTest.Balance);
                Assert.Equal(goodOne.Date, toTest.Date);
                Assert.Equal(goodOne.Amount, toTest.Amount);
                Assert.Equal(goodOne.Category, toTest.Category);
                Assert.Equal(goodOne.ShortDescription, toTest.ShortDescription);
            }

            homeBudget.expenses.Delete(expenses[^1].Id); // gotta delete it cause we are using the same database and expect there to be nothing in there
        }

        [Fact]
        public void Test_ProcessSearch_ValidAmount()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();

            testView.Presenter.ProcessAddExpense(DateTime.Now, 1, "5", "expense");

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();


            ArrayList budgetItems = new ArrayList(homeBudget.GetBudgetItems(null, null, false, 0));
            //budgetItems

            testView.Presenter.ProcessSearch("5", budgetItems, 0); // this refreshes the budget items

            // Assert
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Refresh the budget items and categories", testView.Messages[4]);

            for (int i = 0; i < budgetItems.Count; i++)
            {
                BudgetItem goodOne = (BudgetItem)budgetItems[i];
                BudgetItem toTest = (BudgetItem)testView.BudgetItems[i];
                Assert.Equal(goodOne.Balance, toTest.Balance);
                Assert.Equal(goodOne.Date, toTest.Date);
                Assert.Equal(goodOne.Amount, toTest.Amount);
                Assert.Equal(goodOne.Category, toTest.Category);
                Assert.Equal(goodOne.ShortDescription, toTest.ShortDescription);
            }

            homeBudget.expenses.Delete(expenses[^1].Id); // gotta delete it cause we are using the same database and expect there to be nothing in there
        }

        [Fact]
        public void Test_ProcessSearch_NotBudgetItem()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            ArrayList budgetItems = new ArrayList(homeBudget.GetBudgetDictionaryByCategoryAndMonth(null, null, false, 0));
            //budgetItems

            testView.Presenter.ProcessSearch("e", budgetItems, 0); // will hit the first return since its a non budget item

            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Closed FileSelectWindow", testView.Messages[2]);
            Assert.Equal(3, testView.Messages.Count);
        }

        [Fact]
        public void Test_ProcessSearch_EmptyBudgetItems()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            ArrayList budgetItems = new ArrayList(homeBudget.GetBudgetItems(null, null, false, 0));

            testView.Presenter.ProcessSearch("a", budgetItems, 0); // will hit the first return since its a non budget item

            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Closed FileSelectWindow", testView.Messages[2]);
            Assert.Equal(3, testView.Messages.Count);
        }

        [Fact]
        public void Test_ProcessSearch_NoResultsDescription()
        {   
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();

            testView.Presenter.ProcessAddExpense(DateTime.Now, 1, "5", "expense");

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();


            ArrayList budgetItems = new ArrayList(homeBudget.GetBudgetItems(null, null, false, 0));
            //budgetItems

            testView.Presenter.ProcessSearch("a", budgetItems, 0); // since "expense" doesnt contain a, it wont find anything

            // Assert
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Closed FileSelectWindow", testView.Messages[2]);
            Assert.Equal("Played No Results Search", testView.Messages[4]);
            Assert.Single(budgetItems);
            Assert.Empty(testView.BudgetItems);

            homeBudget.expenses.Delete(expenses[^1].Id); // gotta delete it cause we are using the same database and expect there to be nothing in there
        }

        [Fact]
        public void Test_ProcessSearch_NoResultsAmount()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();

            testView.Presenter.ProcessAddExpense(DateTime.Now, 1, "5", "expense");

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();


            ArrayList budgetItems = new ArrayList(homeBudget.GetBudgetItems(null, null, false, 0));
            //budgetItems

            testView.Presenter.ProcessSearch("1", budgetItems, 0); // since "expense" nor amount doesnt contain 1, it wont find anything

            // Assert
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Closed FileSelectWindow", testView.Messages[2]);
            Assert.Equal("Played No Results Search", testView.Messages[4]);
            Assert.Single(budgetItems); // there is a budget item but doesnt show up in filter
            Assert.Empty(testView.BudgetItems);

            homeBudget.expenses.Delete(expenses[^1].Id); // gotta delete it cause we are using the same database and expect there to be nothing in there
        }

        [Fact]
        public void Test_ProcessRefreshPiechart()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();

            testView.Presenter.ProcessAddExpense(DateTime.Now, 1, "5", "expense");

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();


            ArrayList budgetItems = new ArrayList(homeBudget.GetBudgetItems(null, null, false, 0));
            //budgetItems

            testView.Presenter.ProcessRefreshPiechart(null, null, false, 0);


            // Assert
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Closed FileSelectWindow", testView.Messages[2]);
            Assert.Equal("Piechart was refreshed", testView.Messages[4]);

            List<Category> categories = homeBudget.categories.List();
            for (int i=0; i<categories.Count; i++)
            {
                Assert.Equal(categories[i].Description, testView.PieChartCategories[i]);
            }

            homeBudget.expenses.Delete(expenses[^1].Id); // gotta delete it cause we are using the same database and expect there to be nothing in there
        }

        [Fact]
        public void Test_ProcessDelete_True()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();

            testView.Presenter.ProcessAddExpense(DateTime.Now, 1, "5", "expense");

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();


            ArrayList budgetItems = new ArrayList(homeBudget.GetBudgetItems(null, null, false, 0));
            //budgetItems

            testView.Presenter.ProcessDeleteExpense(1, true);

            // Assert
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Closed FileSelectWindow", testView.Messages[2]);
            Assert.Equal("Showed Completion: Expense was successfully deleted.", testView.Messages[4]);

            Assert.Equal(0, homeBudget.expenses.List().Count);
        }

        [Fact]
        public void Test_ProcessDelete_False()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();

            testView.Presenter.ProcessAddExpense(DateTime.Now, 1, "5", "expense");

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();


            ArrayList budgetItems = new ArrayList(homeBudget.GetBudgetItems(null, null, false, 0));
            //budgetItems

            testView.Presenter.ProcessDeleteExpense(1, false);

            // Assert
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Closed FileSelectWindow", testView.Messages[2]);

            Assert.Equal(1, homeBudget.expenses.List().Count); // did not delete it
            homeBudget.expenses.Delete(expenses[^1].Id); //stii gotta delete it tho after
        }

        [Fact]
        public void Test_ProcessDelete_InvalidId()
        {
            // Arrange
            TestView testView = new TestView(DbFilePath);

            // Act
            testView.OpenFileDialog();

            testView.Presenter.ProcessAddExpense(DateTime.Now, 1, "5", "expense");

            HomeBudget homeBudget = new HomeBudget(DbFilePath);
            List<Expense> expenses = homeBudget.expenses.List();


            ArrayList budgetItems = new ArrayList(homeBudget.GetBudgetItems(null, null, false, 0));
            //budgetItems

            testView.Presenter.ProcessDeleteExpense(-1, true); // wont crash/delete since that doesnt exist

            // Assert
            Assert.Equal("Showed HomeBudgetWindow", testView.Messages[1]);
            Assert.Equal("Closed FileSelectWindow", testView.Messages[2]);

            Assert.Equal(1, homeBudget.expenses.List().Count);
            homeBudget.expenses.Delete(expenses[^1].Id); //stii gotta delete it tho after
        }
    }
}