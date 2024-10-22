using LexicalAnalise;
using NameTables;
using System.Reflection.Emit;
using Translation;
using Translator;

namespace SyntaxAnalyzer;

public static class SyntaxAnalyzer
{
	private static string currentLabel = "";

	private static void CheckLexem(Lexem expectedLexem)
	{
		{
			LexicalAnalyzer lexems = new LexicalAnalyzer();

			if (lexems.Lexem == expectedLexem)
			{
				LexicalAnalyzer.ParseNextLexem();
			}
			else if (lexems.Lexem == Lexem.EOF)
			{
				return;
			}
			else
			{
				ErrorHandler.AddError($"Вместо {lexems.Lexem} ожидалось {expectedLexem}. (Строка {Reader.LineNumber}, позиция {Reader.PositionInLine}, символ '{Reader.Character}')");
			}
		}
	}
		private static void ParseVariableDeclaration()
		{
			LexicalAnalyzer lexems = new LexicalAnalyzer();
			CheckLexem(Lexem.Type);

			if (lexems.Lexem == Lexem.Name)
			{
				NameTable.AddIdentifier(lexems.Name, tCat.Var, tType.Int); /////////////////////////
				LexicalAnalyzer.ParseNextLexem();
			}
			else
			{
				ErrorHandler.AddError($"Ожидалось название переменной. (Строка {Reader.LineNumber}, позиция {Reader.PositionInLine}, символ '{Reader.Character}')");
			}

			while (lexems.Lexem == Lexem.Comma)
			{
				LexicalAnalyzer.ParseNextLexem();
				if (lexems.Lexem == Lexem.Name)
				{
					NameTable.AddIdentifier(lexems.Name, tCat.Var, tType.Int); /////////////////////////
					LexicalAnalyzer.ParseNextLexem();
				}
				else
				{
					ErrorHandler.AddError($"Ожидалось название переменной. (Строка {Reader.LineNumber}, позиция {Reader.PositionInLine}, символ '{Reader.Character}')");
				}
			}

			CheckLexem(Lexem.Delimiter);
		}

		public static void Compile()
		{
			LexicalAnalyzer lexems = new LexicalAnalyzer();
			ParseVariableDeclaration();

		}


	}
	public class ErrorHandler
	{
		private static List<string> _errors;

		public  List<string> Errors => _errors;

		static ErrorHandler()
		{
			_errors = new List<string>();
		}

		public static void AddError(string error)
		{
			_errors.Add(error);
		}
	} 

