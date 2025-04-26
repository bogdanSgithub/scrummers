using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget;

namespace BudgetPresenter
{
    public interface IPresenter
    {
        public string FilePath { get; }
        public void GetPreviousFile();
        public void ProcessSelectedFile(string fileName);
        public void ProcessAddExpense(DateTime dateInput, int categoryInput, string amountInput, string descriptionInput);
        public void ProcessAddCategory(Category.CategoryType categoryType, string description);
        public bool IsFirstTimeUser();
        public void StartApplication();
        public List<Category> GetCategories();
        public Category.CategoryType[] GetCategoryTypes();
        public void CloseApp();
    }
}
