using BudgetPresenter;
﻿using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics.Tracing;
using System.Text.Json;


namespace Frontend_HomeBudget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GetFileWindow : Window
    {
        private IView _view;

        public GetFileWindow(IView view)
        {
            _view = view;
            InitializeComponent();

            // if the user is returning (i.e info.json exists), display the use previous file button.
            if (_view.presenter.IsFirstTimeUser())
                returningUserBtn.Visibility = Visibility.Visible;
        }

        private void FileSelect_Clicked(object sender, RoutedEventArgs e)
        {
            _view.OpenFileDialog();
        }


        private void PreviousFile_Clicked(object sender, RoutedEventArgs e)
        {
            _view.presenter.GetPreviousFile();
        }
    }
}