using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Translation;

namespace LexicalAnalise
{
    public enum Lexems
    {
        None, Name, Number, Begin, End, If, Then, Multiplication, Division, Plus,
        Equal, Less, LessOrEqual, Semi, Assign, LeftBracket, EOF,Separator
    };
     struct Keyword
    {
        public string Word;
        public Lexems Lex;
    };


    public class KeywordManager
    {
        private Keyword[] keywords;
        private int keywordsPointer;
        static string currentName;
        static Lexems currentLexem;
        static private int StringNum;
        static private int PlaceInString;
        static public char CurrentChar;
        static private StreamReader streamReader;

        public string init(string text)
        {
            string AllLexem = string.Empty;
            int i = 0;
            while (i < text.Length){
                CurrentChar = text[i];
                AllLexem += GetNextLexem() + " ";
                i++;
            }
            return AllLexem;
        }
        public KeywordManager(int size)
        {
            keywords = new Keyword[size];
            keywordsPointer = 0;
        }

        public void AddKeyword(string KeyWord, Lexems lexem)
        {
            Keyword kw = new Keyword();
            kw.Word = KeyWord;
            kw.Lex =lexem;
            keywords[keywordsPointer++] = kw;
        }

        public Lexems GetLexeme(string ключевоеСлово)
        {
            for (int i = keywordsPointer - 1; i >= 0; i--)
            {
                if (keywords[i].Word == ключевоеСлово)
                {
                    return keywords[i].Lex;
                }
            }
            return Lexems.None; // Или любое другое значение по умолчанию
        }
        public string GetNextLexem()
        {
            // Пропускаем пробелы
            while (Reader.CurrentChar == ' ')
            {
                Reader.NextChartReading();
            }

            if (char.IsLetter(Reader.CurrentChar))
            {
                GetIdent();
            }
            else if (char.IsDigit(Reader.CurrentChar))
            {
                //РазобратьЧисло();
            }
            else if (Reader.CurrentChar == '\n') // перевод строки
            {
                Reader.NextChartReading();
                currentLexem = Lexems.Separator;
            }
            else if (Reader.CurrentChar == '<')
            {
                Reader.NextChartReading();
                if (Reader.CurrentChar == '=')
                {
                    Reader.NextChartReading();
                    currentLexem = Lexems.LessOrEqual;
                }
                else
                {
                    currentLexem = Lexems.Less;
                }
            }
            else if (Reader.CurrentChar == '+')
            {
                Reader.NextChartReading();
                currentLexem = Lexems.Plus;
            }
            else
            {
                throw new Exception("Ошибка: Недопустимый символ");
            }
            return currentLexem.ToString();
        }
        private void GetIdent()
        {
            string Ident = string.Empty;

            do
            {
                Ident += Reader.CurrentChar;
                Reader.NextChartReading();
            } while (char.IsLetter(Reader.CurrentChar));

            currentName = Ident;
            //currentLexem = GetLexeme(Ident);
            
        }
    }

  


    internal class LexicalAnalise
    {

    }
}
