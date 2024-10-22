using System.Text;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using LexicalAnalise;
using NameTables;
using SyntaxAnalyzer;


namespace Translator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string Path = string.Empty;
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
                Path = openFileDlg.FileName;
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

        private void CompilationButton_Click(object sender, RoutedEventArgs e)
        {
            SaveTextBox.Text = string.Empty;    
            Translation.Reader.Initialize(Path);
            LexicalAnalyzer lexems = new LexicalAnalyzer();
            NameTables.NameTable idef = new NameTables.NameTable();
            ErrorHandler errors = new ErrorHandler();
            
            while (!Translation.Reader.EOF){
                //var lexem = lexems;
                
                if (lexems.Lexem == Lexem.Name && NameTables.NameTable.FindIdentifierByName(lexems.Name.ToString())==null)
                    NameTables.NameTable.AddIdentifier(lexems.Name.ToString(), tCat.Var);
                SyntaxAnalyzer.SyntaxAnalyzer.Compile();
                LexicalAnalyzer.ParseNextLexem();
                if (lexems.Lexem == Lexem.Delimiter)
                {
                    SaveTextBox.Text += '\n';
                }
                SaveTextBox.Text += lexems.Lexem.ToString() +" ";
            }
            SaveTextBox.Text += "\nVar:";
            foreach (Identifier id in idef.Identifiers)
                SaveTextBox.Text += id.name.ToString() + "; ";
            
            if( errors.Errors != null )
                foreach (string  error in errors.Errors)
                    SaveTextBox.Text += error;
            Translation.Reader.Close();
           
            //SaveTextBox.Text = SelectTextBox.Text;

        }
    }
}