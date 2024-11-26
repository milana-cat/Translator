﻿using System.IO;
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
            if (!File.Exists(Path))
            {
                string path = "text.txt";
                File.WriteAllText(path, SelectTextBox.Text);
                SaveTextBox.Text = File.ReadAllText(path);
                Translation.Reader.Initialize(path);
            }
            else
            {
                Translation.Reader.Initialize(Path);
            }
            SaveTextBox.Text = string.Empty;
            //LexicalAnalyzer lexems = new LexicalAnalyzer();
            //NameTables.NameTable idef = new NameTables.NameTable();
            ErrorHandler errors = new ErrorHandler();
            
            SyntaxAnalyzer.SyntaxAnalyzer.Compile();
            if (errors.Errors.Count > 0) {
                foreach (var error in errors.Errors)
                {
                    SaveTextBox.Text += error + '\n';
                }
            }
            else
            {
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

           // Console.WriteLine(string.Join('\n', ErrorHandler.Errors));

            foreach (var CodePointer in CodeGenerator.Code)
                SaveTextBox.Text+= CodePointer + '\n';

            //SaveTextBox.Text = SelectTextBox.Text;
            }

        }
    }
}