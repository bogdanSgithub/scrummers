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

            //initialize view
            _view = view;

            //set the selection in the combobox to 0
            CategoryTypes.SelectedIndex = 0;
        }

        /// <summary>
        /// Adds a category to the database with the values inside of the combobox and textbox.
        /// </summary>
        private void AddCategoryButton_Clicked(object sender, RoutedEventArgs e)
        {
            _view.Presenter.ProcessAddCategory((Category.CategoryType)CategoryTypes.SelectedIndex, Description.Text);
        }

        /// <summary>
        /// Cancels the adding operation and closes the window.
        /// </summary>
        private void CancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            _view.CloseAddCategoryWindow();
        }
    }
}
