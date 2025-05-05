using BudgetPresenter;
using System.Collections;

namespace TestPresenter
{
    public class TestView : IView
    {
        public List<string> Messages = new List<string>();
        public IPresenter Presenter { get; }
        public string FilePath { get; }

        public TestView(string filePath)
        {
            FilePath = filePath;
            Presenter = new Presenter(this);
        }

        public void CloseAddCategoryWindow()
        {
            Messages.Add("Closed AddCategoryWindow");
        }

        public void CloseApp()
        {
            Messages.Add("Closed App");
        }

        public void CloseFileSelectWindow()
        {
            Messages.Add("Closed FileSelectWindow");
        }

        public void OpenFileDialog()
        {
            Messages.Add("Opened File Dialog");
            Presenter.ProcessSelectedFile(FilePath);
        }

        public void ShowAddCategoryWindow()
        {
            Messages.Add("Showed AddCategoryWindow");
        }

        public void ShowAddExpenseWindow()
        {
            Messages.Add("Showed AddExpenseWindow");
        }

        public void ShowCompletion(string message)
        {
            Messages.Add("Showed Completion: " + message);
        }

        public void ShowError(string message)
        {
            Messages.Add("Showed Error: " + message);
        }

        public void ShowFileSelectWindow()
        {
            Messages.Add("Showed FileSelectWindow");
        }

        public void ShowHomeBudgetWindow()
        {
            Messages.Add("Showed HomeBudgetWindow");
        }

        public void RefreshBudgetItems(ArrayList budgetItems)
        {
            Messages.Add($"Refresh the budget items, they look like this: {budgetItems}");
        }

        public void RefreshCategories(ArrayList categories)
        {
            Messages.Add($"Refresh the categories, they look like this: {categories}");
        }

        public void RefreshCategoryTypes(ArrayList categoryTypes)
        {
            Messages.Add($"Refresh the category types, they look like this: {categoryTypes}");
        }
    }
}
