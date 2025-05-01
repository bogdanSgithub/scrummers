using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BudgetPresenter
{
    public interface IView
    {
        public void ShowAddExpenseWindow();
        public void OpenFileDialog();
        public IPresenter Presenter { get; }
        public void ShowFileSelectWindow();
        public void ShowError(string message);
        public void ShowCompletion(string message);
        public void CloseApp();
        public void CloseAddCategoryWindow();
        public void CloseFileSelectWindow();
        public void ShowAddCategoryWindow();
        public void ShowHomeBudgetWindow();
        public void RefreshBudgetItems(ArrayList budgetItems);
    }
}
