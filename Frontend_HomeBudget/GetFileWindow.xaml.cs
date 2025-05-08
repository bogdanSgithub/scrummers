using BudgetPresenter;
using System.Windows;

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
            // initialize view
            _view = view;
            InitializeComponent();

            // if the user is returning (i.e info.json exists), display the use previous file button.
            if (!_view.Presenter.IsFirstTimeUser())
                returningUserBtn.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Opens the file dialog for selecting the db file on click.
        /// </summary>
        private void FileSelect_Clicked(object sender, RoutedEventArgs e)
        {
            _view.OpenFileDialog();
        }

        /// <summary>
        /// Reloads the previously used db file from the .json file to use as the main db.
        /// </summary>
        private void PreviousFile_Clicked(object sender, RoutedEventArgs e)
        {
            _view.Presenter.GetPreviousFile();
        }

    }
}