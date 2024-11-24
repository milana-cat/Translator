using NameTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator
{

    public static class CodeGenerator
    {
        private static List<string> _code;
        private static int _countLabels;

        public static List<string> Code => _code;
        public static string CurrentLabel => "label" + _countLabels;

        static CodeGenerator()
        {
            _code = new List<string>();
            _countLabels = 0;
        }

        public static void AddLabel()
        {
            _countLabels++;
        }

        public static void AddInstruction(string instruction)
        {
            _code.Add(instruction);
        }

        public static void DeclareDataSegment()
        {
            AddInstruction("data segment");
        }

        public static void DeclareStackAndCodeSegments()
        {
            AddInstruction("PRINT_BUF DB ' ' DUP(10)");
            AddInstruction("BUFEND    DB '$'");
            AddInstruction("data ends");
            AddInstruction("stk segment stack");
            AddInstruction("db 256 dup (\"?\")");
            AddInstruction("stk ends");
            AddInstruction("code segment");
            AddInstruction("assume cs:code,ds:data,ss:stk");
            AddInstruction("main proc");
            AddInstruction("mov ax,data");
            AddInstruction("mov ds,ax");
        }

        public static void DeclareMainProcedureCompletion()
        {
            AddInstruction("mov ax,4c00h");
            AddInstruction("int 21h");
            AddInstruction("main endp");

        }

        public static void DeclareCodeCompletion()
        {
            AddInstruction("code ends");
            AddInstruction("end main");
        }

        public static void DeclareVariables()
        {
            foreach (Identifier identifier in NameTable.Identifiers)
                AddInstruction(identifier.name + "  dw    1");
        }

        public static void DeclarePrintProcedure()
        {
            AddInstruction("PRINT PROC NEAR");
            AddInstruction("MOV   CX, 10");
            AddInstruction("MOV   DI, BUFEND - PRINT_BUF");
            AddInstruction("PRINT_LOOP:");
            AddInstruction("MOV   DX, 0");
            AddInstruction("DIV   CX");
            AddInstruction("ADD   DL, '0'");
            AddInstruction("MOV   [PRINT_BUF + DI - 1], DL");
            AddInstruction("DEC   DI");
            AddInstruction("CMP   AL, 0");
            AddInstruction("JNE   PRINT_LOOP");
            AddInstruction("LEA   DX, PRINT_BUF");
            AddInstruction("ADD   DX, DI");
            AddInstruction("MOV   AH, 09H");
            AddInstruction("INT   21H");
            AddInstruction("RET");
            AddInstruction("PRINT ENDP");
        }
    }

}
