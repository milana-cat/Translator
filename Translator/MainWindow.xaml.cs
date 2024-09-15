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


namespace Translator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Microsoft.Win32.OpenFileDialog openFileDlg;
        Microsoft.Win32.SaveFileDialog saveFileDlg;
        public MainWindow()
        {
            openFileDlg = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };
            saveFileDlg = new Microsoft.Win32.SaveFileDialog()
            {
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };           
            InitializeComponent();
        }

        private void SelectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (openFileDlg.ShowDialog() == true)
            {
                SelectTextBox.Text = System.IO.File.ReadAllText(openFileDlg.FileName);
            }
        }
        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {          
            if (saveFileDlg.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(saveFileDlg.FileName, SaveTextBox.Text);
            }
        }

       
    }
}