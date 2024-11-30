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
using System.Reflection;



namespace Translator
{

    ///Исправление на форме сделать
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
            //LexicalAnalyzer lexems = new LexicalAnalyzer();
            //NameTables.NameTable idef = new NameTables.NameTable();
            ErrorHandler errors = new ErrorHandler();
            SyntaxAnalyzer.SyntaxAnalyzer.Compile();
            /*while (!Translation.Reader.EOF){
                //var lexem = lexems;
               
                if (lexems.Lexem == Lexem.Name && NameTables.NameTable.FindIdentifierByName(lexems.Name.ToString())==null)
                    NameTables.NameTable.AddIdentifier(lexems.Name.ToString(), tCat.Var);
                
                LexicalAnalyzer.ParseNextLexem();
                if (lexems.Lexem == Lexem.Delimiter)
                {
                    SaveTextBox.Text += '\n';
                }
                SaveTextBox.Text += lexems.Lexem.ToString() +" ";
        }
            SaveTextBox.Text += "\n Var:";
            foreach (Identifier id in idef.Identifiers)
                SaveTextBox.Text += id.name.ToString() + "; ";*/

            Console.WriteLine(string.Join('\n', ErrorHandler.Errors));

            foreach (var CodePointer in CodeGenerator.Code)
                SaveTextBox.Text+= CodePointer + '\n';

            //SaveTextBox.Text = SelectTextBox.Text;
            }
            Translation.Reader.Close();

public static class StaticDataCleaner
{
    public static void ClearStaticData()
    {
        // Получаем все типы в текущей сборке
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();

        foreach (Type type in types)
        {
            // Проверяем, является ли тип статическим классом
            if (type.IsClass && type.IsAbstract && type.IsSealed)
            {
                // Получаем все статические поля класса
                FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                foreach (FieldInfo field in fields)
                {
                    // Устанавливаем значение по умолчанию для статических полей
                    object defaultValue = field.FieldType.IsValueType ? Activator.CreateInstance(field.FieldType) : null;
                    field.SetValue(null, defaultValue);
                }
                // Попытка повторной инициализации (вызов метода инициализации)
                MethodInfo initializeMethod = type.GetMethod(type.Name, BindingFlags.Static | BindingFlags.Public);
                if (initializeMethod != null)
                {
                    initializeMethod.Invoke(null, null);
                }
            }
        }
    }
}