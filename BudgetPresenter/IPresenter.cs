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
        public void GetSelectedFile(string fileName);
        public bool IsFirstTimeUser();
        public void StartProgram();
        public List<Category> GetCategories();
        public void ProcessAddExpense(DateTime dateInput, int categoryInput, string amountInput, string descriptionInput);
        public void CloseApp();
        public void ProcessAddCategory(Category.CategoryType categoryType, string description);
    }
}
