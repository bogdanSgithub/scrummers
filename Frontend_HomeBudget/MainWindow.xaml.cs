using Microsoft.Win32;
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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // if the user is returning (i.e info.json exists), display the use previous file button.
            if (File.Exists("../../../info.json"))
                returningUserBtn.Visibility = Visibility.Visible;
        }

        private void FileSelect_Clicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "DB files (*.db)|*.db";

            // if the user actually selects a file and doesnt just close the window
            if (fileDialog.ShowDialog() == true)
            {
                try
                {
                    // add \\ to each slash because we need to escape those characters in the file path.
                    string escapedFilePath = fileDialog.FileName.Replace("\\", "\\\\");

                    // write to json file
                    File.WriteAllText("../../../info.json", $"{{ \"current\": \"{ escapedFilePath }\" }}");  
                }
                catch (Exception ex)  
                {
                    MessageBox.Show(ex.Message);
                }
                
            }

        }

        /// <summary>
        /// Represents all the different attributes inside of the json file.
        /// </summary>
        public class DeserializedFileInfo
        {
            /// <summary>
            /// The current or last file used by the Home Budget.
            /// </summary>
            public string current { get; set; }
        }

        private void PreviousFile_Clicked(object sender, RoutedEventArgs e)
        {
            string jsonFileContent = File.ReadAllText("../../../info.json");

            // read the data inside of the file, and set the "current" field of the object to the "current" field of the json file
            DeserializedFileInfo data = JsonSerializer.Deserialize<DeserializedFileInfo>(jsonFileContent);        
        }
    }
}