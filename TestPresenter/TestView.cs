using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget;
using BudgetPresenter;

namespace TestPresenter
{
    internal class TestView : IView
    {
        public List<string> Messages = new List<string>();
        public List<Category> Categories = new List<Category>();
        public Presenter Presenter;

        public TestView(string filepath) 
        {
            Presenter = new Presenter(filepath, this);
            Category AddCategoryItem = new Category(-1, "+ Add Category");

            Categories = Presenter.GetCategories();
            Categories.Add(AddCategoryItem);
        }

        public void ShowCompletion(string message)
        {
            Messages.Add(message);
        }

        public void ShowError(string message)
        {
            Messages.Add(message);
        }
    }
}
