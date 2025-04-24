using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetPresenter;
using System.Windows;
using System.Windows.Controls;
using Budget;
using Microsoft.Win32;

namespace Frontend_HomeBudget
{
    class View : IView
    {

        public IPresenter presenter { get; }
        private GetFileWindow fileWindow;
        private AddExpenseWindow expenseWindow;
        private AddCategoryWindow addCategoryWindow;

        public View()
        {
            presenter = new Presenter(this);
            presenter.StartProgram();
        }

        public void ShowFileSelectWindow()
        {
            fileWindow = new GetFileWindow(this);
            fileWindow.Show();
        }

        public void ShowAddExpenseWindow()
        {
            expenseWindow = new AddExpenseWindow(this);
            expenseWindow.Show();
            fileWindow.Close();
        }

        public void ShowAddCategoryWindow()
        {
            addCategoryWindow = new AddCategoryWindow(this);
            addCategoryWindow.ShowDialog();
        }

        public void OpenFileDialog()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "DB files (*.db)|*.db";


            if (fileDialog.ShowDialog() == true)
            {
               presenter.GetSelectedFile(fileDialog.FileName);
            }
        }

        public void ShowCompletion(string message)
        {
            MessageBox.Show(message, "Succesfull", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void CloseAddCategoryWindow()
        {
            addCategoryWindow.Close();
        }
    }
}
