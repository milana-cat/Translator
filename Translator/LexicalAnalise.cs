using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Translation;

namespace LexicalAnalise
{
    public struct Keyword
    {
        public string word;
        public Lexem lexem;

        public Keyword()
        {
            word = string.Empty;
            lexem = Lexem.None;
        }

        public Keyword(string word, Lexem lexem)
        {
            this.word = word;
            this.lexem = lexem;
        }
    }
    public  enum Lexem
    {
        None,

        EOF, Delimiter, Comma, Assign, LeftBracket, RightBracket,

        Number, Name, Type, Int,

        Less, LessOrEqual, Greater, GreaterOrEqual, Equal, NotEqual,

        Plus, Minus, Multiplication, Division,

        Begin, End, Print,

        If, Then, ElseIf, Else, EndIf,

        While, EndWhile, For, To, Do, Repeat, When, EndFor,

        Not,
        Or,
        Xor,
        And
    }
    public static class LexicalAnalyzer
    {
        private static List<Keyword> _keywords;
        private static Lexem _lexem;
        private static string _name;
        private static int _number;

       static LexicalAnalyzer()
        {
            _keywords = new List<Keyword>();
            _lexem = Lexem.None;
            _name = string.Empty;
            _number = 0;

            AddKeyword("var", Lexem.Type);

            AddKeyword("print", Lexem.Print);


            AddKeyword("if", Lexem.If);
            AddKeyword("then", Lexem.Then);
            AddKeyword("elseif", Lexem.ElseIf);
            AddKeyword("else", Lexem.Else);
            AddKeyword("endif", Lexem.EndIf);
            AddKeyword("while", Lexem.While);
            AddKeyword("endwhile", Lexem.EndWhile);
            AddKeyword("for", Lexem.For);
            AddKeyword("to", Lexem.To);
            AddKeyword("do", Lexem.Do);
            AddKeyword("repeat", Lexem.Repeat);
            AddKeyword("when", Lexem.When);
            AddKeyword("endfor", Lexem.EndFor);
            AddKeyword("not", Lexem.Not);
            AddKeyword("or", Lexem.Or);
            AddKeyword("xor", Lexem.Xor);
            AddKeyword("and", Lexem.And);

            ParseNextLexem();
        }

        public static  Lexem Lexem => _lexem;
        public static string Name => _name;
        public static int Number => _number;

        private static void AddKeyword(string word, Lexem lexem)
        {
            _keywords.Add(new Keyword(word, lexem));
        }

        private static Lexem GetKeyword(string word)
        {
            foreach (Keyword keyword in _keywords)
                if (keyword.word == word)
                    return keyword.lexem;

            return Lexem.Name;
        }

        public static void ParseNextLexem()
        {
            if (!Reader.IsInitialized)
            {
                throw new Exception($"Ошибка! Объект {nameof(Reader)} не был инициализирован!");
            }

            if (Reader.EOF)
            {
                _lexem = Lexem.EOF;
                return ;
            }

            while (Reader.Character == ' ')
            {
                Reader.ReadNextCharacter();
            }

            if (char.IsLetter(Reader.Character))
            {
                ParseName();
            }
            else if (char.IsDigit(Reader.Character))
            {
                ParseNumber();
            }
            else if (Reader.Character == '\n')
            {
                Reader.ReadNextCharacter();
                _lexem = Lexem.Delimiter;
            }
            else if (Reader.Character == ',')
            {
                Reader.ReadNextCharacter();
                _lexem = Lexem.Comma;
            }
            else if (Reader.Character == '(')
            {
                Reader.ReadNextCharacter();
                _lexem = Lexem.LeftBracket;
            }
            else if (Reader.Character == ')')
            {
                Reader.ReadNextCharacter();
                _lexem = Lexem.RightBracket;
            }
            else if (Reader.Character == '=')
            {
                Reader.ReadNextCharacter();
                if (Reader.Character == '=')
                {
                    Reader.ReadNextCharacter();
                    _lexem = Lexem.Equal;
                }
                else
                {
                    _lexem = Lexem.Assign;
                }
            }
            /*
            else if (Reader.Character == '!')
            {
                Reader.ReadNextCharacter();
                if (Reader.Character == '=')
                {
                    Reader.ReadNextCharacter();
                    _lexem = Lexem.NotEqual;
                }
                else
                {
                    throw new Exception("Ошибка! Недопустимый символ!");
                }
            }
            else if (Reader.Character == '<')
            {
                Reader.ReadNextCharacter();
                if (Reader.Character == '=')
                {
                    Reader.ReadNextCharacter();
                    _lexem = Lexem.LessOrEqual;
                }
                else
                {
                    _lexem = Lexem.Less;
                }
            }
            else if (Reader.Character == '>')
            {
                Reader.ReadNextCharacter();
                if (Reader.Character == '=')
                {
                    Reader.ReadNextCharacter();
                    _lexem = Lexem.GreaterOrEqual;
                }
                else
                {
                    _lexem = Lexem.Greater;
                }
            }*/
            else if (Reader.Character == '+')
            {
                Reader.ReadNextCharacter();
                _lexem = Lexem.Plus;
            }
            else if (Reader.Character == '-')
            {
                Reader.ReadNextCharacter();
                _lexem = Lexem.Minus;
            }
            else if (Reader.Character == '*')
            {
                Reader.ReadNextCharacter();
                _lexem = Lexem.Multiplication;
            }
            else if (Reader.Character == '/')
            {
                Reader.ReadNextCharacter();
                _lexem = Lexem.Division;
            }
            else
            {
                throw new Exception("Ошибка! Недопустимый символ!");
            }
            
        }

        private static void ParseName()
        {
            string name = string.Empty;

            while (char.IsLetter(Reader.Character))
            {
                name += Reader.Character;
                Reader.ReadNextCharacter();
            }

            _name = name;
            _lexem = GetKeyword(name);
        }

        private static void ParseNumber()
        {
            string number = string.Empty;

            while (char.IsDigit(Reader.Character))
            {
                number += Reader.Character;
                Reader.ReadNextCharacter();
            }

            if (!int.TryParse(number, out int result))
            {
                throw new Exception("Ошибка! Переполнение типа var!");
            }

            _number = result;
            _lexem = Lexem.Number;
        }
    }
}
