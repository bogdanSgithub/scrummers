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

            if (File.Exists("../../../info.json"))
                returningUserBtn.Visibility = Visibility.Visible;


        }

        private void FileSelect_Clicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "DB files (*.db)|*.db";

            if (fileDialog.ShowDialog() == true)
            {
                try
                {
                    string escapedFilePath = fileDialog.FileName.Replace("\\", "\\\\");

                    File.WriteAllText("../../../info.json", $"{{ \"current\": \"{ escapedFilePath }\" }}");  
                }
                catch (Exception ex)  
                {
                    MessageBox.Show(ex.Message);
                }
                
            }

        }

        public class DeserializedFileInfo
        {
            public string current { get; set; }
        }



        private void PreviousFile_Clicked(object sender, RoutedEventArgs e)
        {
            string jsonFileContent = File.ReadAllText("../../../info.json");

            DeserializedFileInfo data = JsonSerializer.Deserialize<DeserializedFileInfo>(jsonFileContent);

            MessageBox.Show(data.current);
        }

    }
}