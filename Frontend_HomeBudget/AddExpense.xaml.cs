using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Budget;
using BudgetPresenter;

namespace Frontend_HomeBudget
{
    /// <summary>
    /// Interaction logic for AddExpense.xaml
    /// </summary>
    public partial class AddExpense : Window, IView
    {
        public AddExpense(IPresenter presenter)
        {
            InitializeComponent();

            Category AddCategoryItem = new Category(-1, "+ Add Category");

            List<Category> categories = presenter.GetCategories();

            categories.Add(AddCategoryItem);

            Categorys.ItemsSource = categories;
            Categorys.SelectedIndex = 0;
        }

        private void Button_AddExpense_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
