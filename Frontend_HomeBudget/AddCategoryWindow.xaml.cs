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
    /// Interaction logic for AddCategoryWindow.xaml
    /// </summary>
    public partial class AddCategoryWindow : Window
    {
        private IView _view;

        public AddCategoryWindow(IView view)
        {
            InitializeComponent();

            _view = view;

            CategoryTypes.SelectedIndex = 0;

            CategoryTypes.ItemsSource = _view.Presenter.GetCategoryTypes();
        }

        private void AddCategoryButton_Clicked(object sender, RoutedEventArgs e)
        {
            _view.Presenter.ProcessAddCategory((Category.CategoryType)CategoryTypes.SelectedIndex, Description.Text);
        }

        private void CancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            _view.CloseAddCategoryWindow();
        }
    }
}
