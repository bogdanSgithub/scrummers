using BudgetPresenter;
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

namespace Frontend_HomeBudget
{
    /// <summary>
    /// Interaction logic for UpdateExpenseWindow.xaml
    /// </summary>
    public partial class UpdateExpenseWindow : Window
    {
        private IView _view;

        public UpdateExpenseWindow(IView view)
        {
            _view = view;

            InitializeComponent();
        }
    }
}
