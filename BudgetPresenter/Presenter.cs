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


    public class Presenter : IPresenter
    {
        public string FilePath { get; private set; }

        public List<Category> GetCategories()
        {
            return _homeBudget.categories.List();
        }

        private HomeBudget _homeBudget;
        private IView _view;

        public Presenter(IView view)
        {
            _view = view;
        }

        public void StartProgram()
        {   
            _view.ShowFileSelectWindow();
        }

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
    }
}
