using Budget;
using System.Text;
using System.Threading.Tasks;
using BudgetPresenter;
using System.Windows;
using Budget;
using System.Text.Json;


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

        /// <summary>
        /// The FilePath of the database file
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets all the Categories that are in the HomeBudget
        /// </summary>
        /// <returns>The list of categories in the HomeBudget</returns>
        public List<Category> GetCategories()
        {
            return _homeBudget.categories.List();
        }

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
        public void StartProgram()
        {   
            _view.ShowFileSelectWindow();
        }

        /// <summary>
        /// Processes a fileName that represents the database file path.
        /// </summary>
        /// <param name="fileName">The fileName, aka full path to the database file</param>
        public void GetSelectedFile(string fileName)
        {
            try
            {
                // add \\ to each slash because we need to escape those characters in the file path.
                string escapedFilePath = fileName.Replace("\\", "\\\\");

                // write to json file
                File.WriteAllText("../../../info.json", $"{{ \"current\": \"{escapedFilePath}\" }}");

                FilePath = fileName;

                _homeBudget = new HomeBudget(FilePath);

                _view.ShowAddExpenseWindow();
            }
            catch (Exception ex)
            {
                _view.ShowError(ex.Message);
            }

        }

        public void GetPreviousFile()
        {
            string jsonFileContent = File.ReadAllText("../../../info.json");

            // read the data inside of the file, and set the "current" field of the object to the "current" field of the json file
            DeserializedFileInfo data = JsonSerializer.Deserialize<DeserializedFileInfo>(jsonFileContent);


            FilePath = data.current;

            _homeBudget = new HomeBudget(FilePath);

            _view.ShowAddExpenseWindow();

        }


        public bool IsFirstTimeUser()
        {
            return File.Exists("../../../info.json");
        }

        public Category.CategoryType[] GetCategoryTypes()
        {
            Category.CategoryType[] values = (Category.CategoryType[]) Enum.GetValues(typeof(Category.CategoryType));

            return values;
        }

        public void ProcessAddExpense(DateTime dateInput, int categoryInput, string amountInput, string descriptionInput)
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

        public void CloseApp()
        {
            _view.CloseApp();
        }
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
    }
}
