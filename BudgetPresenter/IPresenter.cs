using System.Collections;

namespace BudgetPresenter
{
    public interface IPresenter
    {
        public string FilePath { get; }
        public void GetPreviousFile();
        public void ProcessSelectedFile(string fileName);
        public void ProcessAddExpense(DateTime dateInput, int categoryInput, string amountInput, string descriptionInput);
        public void ProcessAddCategory(int categoryTypeIndex, string description);
        public void ProcessDeleteExpense(int id, bool answer);
        public bool IsFirstTimeUser();
        public void StartApplication();
        public void CloseApp();
        public void ProcessRefreshBudgetItems(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID, bool ByMonth, bool ByCategory);
        public void ProcessUpdateExpense(int id, DateTime? newDate, int newCategory, string newAmount, String newDescription);
        public void ProcessRefreshCategories();
        public void ProcessRefreshCategoryTypes();
        public void ProcessCategorySelection(int selectionIndex);
        public void ProcessSearch(string searchQuery, ArrayList budgetItems, int startingIndex);
    }
}
