using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetPresenter;
using System.Windows;
using System.Windows.Controls;
using Budget;

namespace Frontend_HomeBudget
{
    class View : IView
    {

        public IPresenter presenter { get; }
        private string _filepath = "C:\\Users\\2276038\\Source\\Repos\\scrummers\\TestPresenter\\currentDatabase.db";
        AddExpenseWindow expenseWindow;
        private Application _app;

        public View()
        {
            presenter = new Presenter(_filepath, this);
            _app = new Application();
            presenter.StartProgram();
        }

        public void ShowFileSelectWindow()
        {
            expenseWindow = new AddExpenseWindow(_filepath, this);
            _app.Run(expenseWindow);
        }

        public void ShowCompletion(string message)
        {
            MessageBox.Show(message, "Succesfull", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
