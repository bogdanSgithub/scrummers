using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetPresenter;
using System.Windows;

namespace Frontend_HomeBudget
{
    class View : IView
    {   
        public View()
        {
            var app = new Application();
            AddExpense expenseWindow = new AddExpense("uiwqhuiduhid");
            app.Run(expenseWindow);
        }

        public void ShowCompletion(string message)
        {
            throw new NotImplementedException();
        }

        public void ShowError(string message)
        {
            throw new NotImplementedException();
        }
    }
}
