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
        public void StartProgram();
        public List<Category> GetCategories();
        public void AddExpense(DateTime dateInput, int categoryInput, string amountInput, string descriptionInput);
    }
}
