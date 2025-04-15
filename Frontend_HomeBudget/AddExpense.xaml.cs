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
    /// Interaction logic for AddExpense.xaml
    /// </summary>
    public partial class AddExpense : Window
    {
        public AddExpense()
        {
            InitializeComponent();
            List<string> items = new List<string>
            {
                "Option A",
                "Option B",
                "Option C"
            };
            items.Add("+ Add Category");

            Categorys.ItemsSource = items;
            Categorys.SelectedIndex = 0;
        }

        private void Button_AddExpense_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
