﻿using System.Windows;
using System.Windows.Media.Imaging;
using BudgetPresenter;
using System.IO;
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
            Icon = new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image.png")));
        }

        /// <summary>
        /// Adds a category to the database with the values inside of the combobox and textbox.
        /// </summary>
        private void AddCategoryButton_Clicked(object sender, RoutedEventArgs e)
        {
            _view.Presenter.ProcessAddCategory(CategoryTypes.SelectedIndex, Description.Text);
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
