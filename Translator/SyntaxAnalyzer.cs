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

			if (LexicalAnalyzer.Lexem == expectedLexem)
			{
				LexicalAnalyzer.ParseNextLexem();
			}

			else
			{
				ErrorHandler.AddError($"Вместо {LexicalAnalyzer.Lexem} ожидалось {expectedLexem}. (Строка {Reader.LineNumber}, позиция {Reader.PositionInLine}, символ '{Reader.Character}')");
			}
		}
	}

    private static void ParseVariableDeclaration()
		{

			CheckLexem(Lexem.Type);

			if (LexicalAnalyzer.Lexem == Lexem.Name)
			{
				NameTable.AddIdentifier(LexicalAnalyzer.Name, tCat.Var, tType.Int); /////////////////////////
				LexicalAnalyzer.ParseNextLexem();
			}
			else
			{
				ErrorHandler.AddError($"Ожидалось название переменной. (Строка {Reader.LineNumber}, позиция {Reader.PositionInLine}, символ '{Reader.Character}')");
			}

			while (LexicalAnalyzer.Lexem == Lexem.Comma)
			{
				LexicalAnalyzer.ParseNextLexem();
				if (LexicalAnalyzer.Lexem == Lexem.Name)
				{
					NameTable.AddIdentifier(LexicalAnalyzer.Name, tCat.Var, tType.Int); /////////////////////////
					LexicalAnalyzer.ParseNextLexem();
				}
				else
				{
					ErrorHandler.AddError($"Ожидалось название переменной. (Строка {Reader.LineNumber}, позиция {Reader.PositionInLine}, символ '{Reader.Character}')");
				}
			}

			CheckLexem(Lexem.Delimiter);
		}


    private static void ParseSequenceOfInstructions()
    {
        ParseInstruction();
        while (LexicalAnalyzer.Lexem == Lexem.Delimiter)
        {
            LexicalAnalyzer.ParseNextLexem();
            ParseInstruction();
        }
    }

    private static void ParseInstruction()
    {
        if (LexicalAnalyzer.Lexem == Lexem.Name)
        {
            Identifier? x = NameTable.FindIdentifierByName(LexicalAnalyzer.Name);
            if (x != null)
            {
                ParseAssingInstruction(x.Value.type);
                CodeGenerator.AddInstruction("pop ax");
                CodeGenerator.AddInstruction(" mov " + x.Value.name + ", ax");
            }
            else
            {
                ErrorHandler.AddError($"Не удалось найти переменную с именем {LexicalAnalyzer.Name}. (Строка {Reader.LineNumber}, позиция {Reader.PositionInLine}, символ '{Reader.Character}')");
            }
        }
        else if (LexicalAnalyzer.Lexem == Lexem.Print)
        {
            ParsePrintInstruction();
        }
        else if (LexicalAnalyzer.Lexem == Lexem.If)
        {
            ParseConditionalStatement();
        }
        else if (LexicalAnalyzer.Lexem == Lexem.While)
        {
            ParseWhileLoop();
        }
        else if (LexicalAnalyzer.Lexem == Lexem.For)
        {
            ParseForLoop();
        }
        else if (LexicalAnalyzer.Lexem == Lexem.Repeat) // Добавляем поддержку цикла repeat ... until
        {
            ParseRepeatUntilLoop();
        }
    
    }

    private static void ParseAssingInstruction(tType varType)
    {
        LexicalAnalyzer.ParseNextLexem();
        if (LexicalAnalyzer.Lexem == Lexem.Assign)
        {
            LexicalAnalyzer.ParseNextLexem();
            tType t = ParseExpression();
            if (varType != t)
            {
                ErrorHandler.AddError($"Несовместимые типы при присваивании.");
            }
        }
        else
        {
            ErrorHandler.AddError($"Не удалось распарсить инструкцию присваивания. (Строка {Reader.LineNumber}, позиция {Reader.PositionInLine}, символ '{Reader.Character}')");
        }
    }

    private static tType ParseExpression()
    {
        tType t = ParseAdditionOrSubtraction();

        if (LexicalAnalyzer.Lexem == Lexem.Equal || LexicalAnalyzer.Lexem == Lexem.NotEqual ||
            LexicalAnalyzer.Lexem == Lexem.Less || LexicalAnalyzer.Lexem == Lexem.Greater ||
            LexicalAnalyzer.Lexem == Lexem.LessOrEqual || LexicalAnalyzer.Lexem == Lexem.GreaterOrEqual)
        {
            string transition = "";
            switch (LexicalAnalyzer.Lexem)
            {
                case Lexem.Equal:
                    transition = "jne";
                    break;
                case Lexem.NotEqual:
                    transition = "je";
                    break;
                case Lexem.Greater:
                    transition = "jle";
                    break;
                case Lexem.GreaterOrEqual:
                    transition = "jl";
                    break;
                case Lexem.Less:
                    transition = "jge";
                    break;
                case Lexem.LessOrEqual:
                    transition = "jg";
                    break;
            }

            LexicalAnalyzer.ParseNextLexem();
            tType t2 = ParseAdditionOrSubtraction();
            // Проверка типов для операции сравнения (например, только сравнение числовых типов)
            if (t != t2 || t != tType.Int)
            {
                ErrorHandler.AddError("Несовместимые типы для операции сравнения.");
            }

            CodeGenerator.AddInstruction("pop ax");
            CodeGenerator.AddInstruction("pop bx");
            CodeGenerator.AddInstruction("cmp bx, ax");
            CodeGenerator.AddInstruction(transition + " " + currentLabel);
            currentLabel = "";
            t = tType.Bool;
        }

        return t;
    }

    private static tType ParseAdditionOrSubtraction()
    {
        tType t;
        Lexem _operator;

        bool isNegative = false;

        // Проверяем наличие унарного минуса в начале выражения
        if (LexicalAnalyzer.Lexem == Lexem.Minus)
        {
            isNegative = true; // Устанавливаем флаг унарного минуса
            LexicalAnalyzer.ParseNextLexem(); // Пропускаем минус
        }

        t = ParseMultiplicationOrDivision(); // Получаем значение подвыражения

        // Если был унарный минус, применяем его к значению
        if (isNegative)
        {
            if (t != tType.Int)
            {
                ErrorHandler.AddError("Унарный минус может применяться только к целым числам.");
            }
            CodeGenerator.AddInstruction("pop ax");
            CodeGenerator.AddInstruction("neg ax"); // Инвертируем значение
            CodeGenerator.AddInstruction("push ax");
        }

        // Разбор остальных операций сложения/вычитания
        while (LexicalAnalyzer.Lexem == Lexem.Plus || LexicalAnalyzer.Lexem == Lexem.Minus)
        {
            _operator = LexicalAnalyzer.Lexem;
            LexicalAnalyzer.ParseNextLexem();
            tType t2 = ParseMultiplicationOrDivision();
            if (t != t2 || t != tType.Int)
            {
                ErrorHandler.AddError("Несовместимые типы для операции сложения/вычитания.");
            }

            CodeGenerator.AddInstruction("pop bx");
            CodeGenerator.AddInstruction("pop ax");

            switch (_operator)
            {
                case Lexem.Plus:
                    CodeGenerator.AddInstruction("add ax, bx");
                    break;
                case Lexem.Minus:
                    CodeGenerator.AddInstruction("sub ax, bx");
                    break;
            }

            CodeGenerator.AddInstruction("push ax");
        }

        return t;
    }

    private static tType ParseMultiplicationOrDivision()
    {
        Lexem _operator;
        tType t = ParseSubexpression();

        while (LexicalAnalyzer.Lexem == Lexem.Multiplication || LexicalAnalyzer.Lexem == Lexem.Division)
        {
            _operator = LexicalAnalyzer.Lexem;
            LexicalAnalyzer.ParseNextLexem();
            tType t2 = ParseSubexpression();

            // Проверка совместимости типов
            if (t != t2 || t != tType.Int)
            {
                ErrorHandler.AddError("Несовместимые типы для операции умножения/деления.");
            }

            switch (_operator)
            {
                case Lexem.Multiplication:
                    CodeGenerator.AddInstruction("pop bx");
                    CodeGenerator.AddInstruction("pop ax");
                    CodeGenerator.AddInstruction("mul bx");
                    CodeGenerator.AddInstruction("push ax");
                    break;
                case Lexem.Division:
                    CodeGenerator.AddInstruction("pop bx");
                    CodeGenerator.AddInstruction("pop ax");
                    CodeGenerator.AddInstruction("cwd");
                    CodeGenerator.AddInstruction("div bl");
                    CodeGenerator.AddInstruction("push ax");
                    break;
            }
        }

        return t;
    }

    private static tType ParseSubexpression()
    {
        Identifier? x;
        tType t = tType.None;

        if (LexicalAnalyzer.Lexem == Lexem.Name)
        {
            x = NameTable.FindIdentifierByName(LexicalAnalyzer.Name);
            if (x != null)
            {
                CodeGenerator.AddInstruction("mov ax, " + LexicalAnalyzer.Name);
                CodeGenerator.AddInstruction("push ax");
                LexicalAnalyzer.ParseNextLexem();
                return x.Value.type;
            }
            else
            {
                ErrorHandler.AddError($"Не удалось найти переменную с именем {LexicalAnalyzer.Name}. (Строка {Reader.LineNumber}, позиция {Reader.PositionInLine}, символ '{Reader.Character}')");
            }
        }
        else if (LexicalAnalyzer.Lexem == Lexem.Number)
        {
            CodeGenerator.AddInstruction("mov ax, " + LexicalAnalyzer.Number);
            CodeGenerator.AddInstruction("push ax");
            LexicalAnalyzer.ParseNextLexem();
            return tType.Int;
        }
        else if (LexicalAnalyzer.Lexem == Lexem.LeftBracket)
        {
            LexicalAnalyzer.ParseNextLexem();
            t = ParseExpression();
            CheckLexem(Lexem.RightBracket);
        }
        else
        {
            ErrorHandler.AddError($"Недопустимое выражение. (Строка {Reader.LineNumber}, позиция {Reader.PositionInLine}, символ '{Reader.Character}')");
        }

        return t;
    }

    private static void ParsePrintInstruction()
    {
        CheckLexem(Lexem.Print);
        if (LexicalAnalyzer.Lexem == Lexem.Name)
        {
            Identifier? x = NameTable.FindIdentifierByName(LexicalAnalyzer.Name);
            CodeGenerator.AddInstruction("push ax");
            CodeGenerator.AddInstruction("mov ax, " + LexicalAnalyzer.Name);
            CodeGenerator.AddInstruction("CALL PRINT");
            CodeGenerator.AddInstruction("pop ax");
            LexicalAnalyzer.ParseNextLexem();
        }
        else
        {
            ErrorHandler.AddError($"Не удалось распарсить выражение вывода. (Строка {Reader.LineNumber}, позиция {Reader.PositionInLine}, символ '{Reader.Character}')");
        }
    }

    private static void ParseConditionalStatement()
    {
        CheckLexem(Lexem.If);
        CodeGenerator.AddLabel();
        string lowLabel = CodeGenerator.CurrentLabel;
        currentLabel = lowLabel;
        CodeGenerator.AddLabel();
        string labelForExit = CodeGenerator.CurrentLabel;
        ParseExpression();
        CheckLexem(Lexem.Then);
        ParseSequenceOfInstructions();
        CodeGenerator.AddInstruction("jmp " + labelForExit);
        while (LexicalAnalyzer.Lexem == Lexem.ElseIf)
        {
            CodeGenerator.AddInstruction(lowLabel + ":");
            CodeGenerator.AddLabel();
            lowLabel = CodeGenerator.CurrentLabel;
            currentLabel = lowLabel;

            LexicalAnalyzer.ParseNextLexem();
            ParseExpression();
            CheckLexem(Lexem.Then);
            ParseSequenceOfInstructions();
            CodeGenerator.AddInstruction("jmp " + labelForExit);
        }
        if (LexicalAnalyzer.Lexem == Lexem.Else)
        {
            CodeGenerator.AddInstruction(lowLabel + ":");
            LexicalAnalyzer.ParseNextLexem();
            ParseSequenceOfInstructions();
        }
        else
        {
            CodeGenerator.AddInstruction(lowLabel + ":");
        }
        CheckLexem(Lexem.EndIf);
        CodeGenerator.AddInstruction(labelForExit + ":");
    }



    private static void ParseForLoop()
    {
        CheckLexem(Lexem.For); // Ожидаем начало цикла for

        // Шаг 1: Инициализация переменной цикла
        if (LexicalAnalyzer.Lexem == Lexem.Name)
        {
            Identifier? loopVar = NameTable.FindIdentifierByName(LexicalAnalyzer.Name);
            if (loopVar == null)
            {
                ErrorHandler.AddError($"Переменная {LexicalAnalyzer.Name} не объявлена.");
                return;
            }

            string loopVariable = LexicalAnalyzer.Name; // Имя переменной цикла
            LexicalAnalyzer.ParseNextLexem();

            CheckLexem(Lexem.Assign); // Ожидаем присваивание переменной (например, i = 1)
            ParseExpression(); // Разбор выражения для начального значения
            CodeGenerator.AddInstruction("pop ax");
            CodeGenerator.AddInstruction($"mov {loopVariable}, ax");

            // Шаг 2: Разбор условия (to или downto)
            CodeGenerator.AddLabel();
            string endLabel = CodeGenerator.CurrentLabel; // Метка конца цикла
            CodeGenerator.AddLabel();
            string startLabel = CodeGenerator.CurrentLabel; // Метка начала цикла
            CodeGenerator.AddInstruction($"{startLabel}:");

            CheckLexem(Lexem.To); // Поддерживаем только "to", можно добавить "downto" для убывающего цикла
            ParseExpression(); // Разбор конечного значения цикла

            CodeGenerator.AddInstruction("pop bx"); // Конечное значение в bx
            CodeGenerator.AddInstruction($"mov ax, {loopVariable}"); // Текущее значение переменной в ax
            CodeGenerator.AddInstruction("cmp ax, bx"); // Сравниваем текущее и конечное значение
            CodeGenerator.AddInstruction($"jg {endLabel}"); // Переход на конец, если достигли значения

            CheckLexem(Lexem.Delimiter);
            //CheckLexem(Lexem.Begin); // Начало тела цикла

            // Шаг 3: Разбор тела цикла
            ParseSequenceOfInstructions();

            CheckLexem(Lexem.EndFor); // Конец тела цикла

            // Шаг 4: Инкремент переменной цикла
            CodeGenerator.AddInstruction($"inc {loopVariable}"); // Увеличиваем значение переменной
            CodeGenerator.AddInstruction($"jmp {startLabel}"); // Переход в начало цикла
            CodeGenerator.AddInstruction($"{endLabel}:"); // Метка конца цикла
        }
        else
        {
            ErrorHandler.AddError("Ожидалось имя переменной цикла.");
        }
    }

    private static void ParseWhileLoop()
    {
        CheckLexem(Lexem.While);
        CodeGenerator.AddLabel();
        string upLabel = CodeGenerator.CurrentLabel;
        CodeGenerator.AddLabel();
        string lowLabel = CodeGenerator.CurrentLabel;
        currentLabel = lowLabel;
        CodeGenerator.AddInstruction(upLabel + ":");
        ParseExpression();
        ParseSequenceOfInstructions();
        CheckLexem(Lexem.EndWhile);
        CodeGenerator.AddInstruction("jmp " + upLabel);
        CodeGenerator.AddInstruction(lowLabel + ":");
    }

    private static void ParseRepeatUntilLoop()
    {
        CheckLexem(Lexem.Repeat); // Ожидаем ключевое слово repeat

        // Генерация метки для начала цикла
        CodeGenerator.AddLabel();
        string startLabel = CodeGenerator.CurrentLabel;

        CodeGenerator.AddLabel();
        string checkLabel = CodeGenerator.CurrentLabel;
        currentLabel = checkLabel;

        CodeGenerator.AddInstruction(startLabel + ":");
        // Разбор последовательности инструкций внутри цикла
        ParseSequenceOfInstructions();

        CheckLexem(Lexem.When); // Ожидаем ключевое слово until

        // Генерация метки для проверки условия завершения цикла


        // Разбор условия завершения цикла
        ParseExpression();


        // Добавляем метку для завершения цикла
        CodeGenerator.AddInstruction("jmp " + startLabel);
        CodeGenerator.AddInstruction($"{checkLabel}:");
    }

    public static void Compile()
    {
        CodeGenerator.DeclareDataSegment();
        ParseVariableDeclaration();
        CodeGenerator.DeclareVariables();
        CodeGenerator.DeclareStackAndCodeSegments();


        ParseSequenceOfInstructions();

        CodeGenerator.DeclareMainProcedureCompletion();
        CodeGenerator.DeclarePrintProcedure();
        CodeGenerator.DeclareCodeCompletion();
    }
}
	public class ErrorHandler
	{
		private static List<string> _errors;

		public static List<string> Errors => _errors;

		static ErrorHandler()
		{
			_errors = new List<string>();
		}

		public static void AddError(string error)
		{
			_errors.Add(error);
		}
	} 

