using Budget;
using System.Text;
using System.Threading.Tasks;
using BudgetPresenter;
using System.Windows;
using Budget;
using System.Text.Json;
using System.Collections;


namespace BudgetPresenter
{
    /// <summary>
    /// Represents all the different attributes inside of the json file.
    /// </summary>
    public class DeserializedFileInfo
    {
        /// <summary>
        /// The current or last file used by the Home Budget.
        /// </summary>
        public string current { get; set; }
    }

    /// <summary>
    /// Class that implements the IPresenter interface
    /// </summary>
    public class Presenter : IPresenter
    {
        private HomeBudget _homeBudget;
        private IView _view;
        private ArrayList _categories;

        /// <summary>
        /// The FilePath of the database file
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        /// <param name="view">The reference to the view</param>
        public Presenter(IView view)
        {
            _view = view;
        }

        /// <summary>
        /// Calls the ShowFileSelectWindow since that's how our app must start
        /// </summary>
        public void StartApplication()
        {   
            _view.ShowFileSelectWindow();
        }

        /// <summary>
        /// Processes a fileName that represents the database file path.
        /// </summary>
        /// <param name="fileName">The fileName, aka full path to the database file</param>
        public void ProcessSelectedFile(string fileName)
        {
            try
            {
                if (fileName is null || fileName == "")
                {
                    throw new ArgumentException("invalid filename, cannot be null or empty.");
                }

                // add \\ to each slash because we need to escape those characters in the file path.
                string escapedFilePath = fileName.Replace("\\", "\\\\");

                // write to json file
                File.WriteAllText("../../../info.json", $"{{ \"current\": \"{escapedFilePath}\" }}");

                FilePath = fileName;

                _homeBudget = new HomeBudget(FilePath);

                _view.ShowHomeBudgetWindow();
                _view.CloseFileSelectWindow();
            }
            catch (Exception ex)
            {
                _view.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Gets the last file that was used by the user which is stored in info.json
        /// </summary>
        public void GetPreviousFile()
        {
            string jsonFileContent = File.ReadAllText("../../../info.json");

            // read the data inside of the file, and set the "current" field of the object to the "current" field of the json file
            DeserializedFileInfo data = JsonSerializer.Deserialize<DeserializedFileInfo>(jsonFileContent);

            FilePath = data.current;

            _homeBudget = new HomeBudget(FilePath);

            _view.ShowHomeBudgetWindow();
            _view.CloseFileSelectWindow();
        }

        /// <summary>
        /// Checks if there is an info.json
        /// </summary>
        /// <returns>whether we have an info.json or not</returns>
        public bool IsFirstTimeUser()
        {
            return !File.Exists("../../../info.json");
        }

        /// <summary>
        /// Takes the fields of an expense, Validates them and calls the expenses.Add method of the homeBudget API.
        /// If a field is invalid, it will tell the view to show the error.
        /// </summary>
        /// <param name="dateInput">DateTime that represents the date of the expense</param>
        /// <param name="categoryInput">int CategoryId that represents the id of the category of the expense</param>
        /// <param name="amountInput">string of Amount that represents the amount of the expense</param>
        /// <param name="descriptionInput">description that represents the description of the expense</param>
        public void ProcessAddExpense(DateTime dateInput, int categoryInput, string amountInput, string descriptionInput)
        {
            double amount;
            if (!(double.TryParse(amountInput, out amount) && amount > 0))
            {
                _view.ShowError("Amount must be a positive number");
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

        /// <summary>
        /// Closes the application
        /// </summary>
        public void CloseApp()
        {
            _view.CloseApp();
        }

        /// <summary>
        /// Process Adding a category, the description cannot be null and it must be a valid categoryType
        /// </summary>
        /// <param name="categoryType"></param>
        /// <param name="description"></param>
        public void ProcessAddCategory(Category.CategoryType categoryType, string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                _view.ShowError("Description cannot be empty.");
                return;
            }

            try
            {
                _homeBudget.categories.Add(description, categoryType);
                _view.ShowCompletion("Category was successfully added.");
                _view.CloseAddCategoryWindow();
            }
            catch (Exception ex)
            {
                _view.ShowError("Error: " + ex.Message);
            }
        }

        public void ProcessRefreshBudgetItems(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID, bool ByMonth, bool ByCategory)
        {
            ArrayList budgetItems;

            if(ByMonth && ByCategory)
            {
                budgetItems = new ArrayList(_homeBudget.GetBudgetDictionaryByCategoryAndMonth(Start, End, FilterFlag, CategoryID));
            }
            else if (ByCategory)
            {
                budgetItems = new ArrayList(_homeBudget.GetBudgetItemsByCategory(Start, End, FilterFlag, CategoryID));
            }
            else if (ByMonth)
            {
                budgetItems = new ArrayList(_homeBudget.GetBudgetItemsByMonth(Start, End, FilterFlag, CategoryID));
            }
            else
            {
                budgetItems = new ArrayList(_homeBudget.GetBudgetItems(Start, End, FilterFlag, CategoryID));
            }

            _view.RefreshBudgetItems(budgetItems);
        }

        public void ProcessRefreshCategories()
        {
            _categories = new ArrayList(_homeBudget.categories.List());

            Category AddCategoryItem = new Category(-1, "+ Add Category");
            _categories.Add(AddCategoryItem);

            _view.RefreshCategories(_categories);
        }
        public void ProcessRefreshCategoryTypes()
        {
            ArrayList categoryTypes;
            Category.CategoryType[] values = (Category.CategoryType[])Enum.GetValues(typeof(Category.CategoryType));
            categoryTypes = new ArrayList(values);

            _view.RefreshCategoryTypes(categoryTypes);
        }

        public void ProcessCategorySelection(int selectionIndex)
        {   
            List<Category> categories = _homeBudget.categories.List();

            if (selectionIndex == _categories.Count - 1)
            {
                _view.ShowAddCategoryWindow();
                _view.Presenter.ProcessRefreshCategories();
            }
        }
    }
}
