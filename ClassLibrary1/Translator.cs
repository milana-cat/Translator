using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics.Eventing.Reader;
using System.ComponentModel.Design;

namespace Translation
{
    public class TranslatorClass
    {
    }


    public class Reader
    {
        static private int StringNum;
        static private int PlaceInString;
        static public char CurrentChar;
        static private StreamReader streamReader;

        private const int FileEnd = -1;
        private const char NewString = '\n';
        private const char Back = '\r';
        private const char Tab = '\t';


        public void init(string путьКФайлу)
        {
            if (File.Exists(путьКФайлу))
            {
                streamReader = new StreamReader(путьКФайлу);
                StringNum = 1;
                PlaceInString = 0;
                NextChartReading();
            }
            else
            {
                throw new FileNotFoundException("Файл не найден.", путьКФайлу);
            }
        }

        public static void NextChartReading()
        {
            if (streamReader != null)
            {
                CurrentChar = (char)streamReader.Read();
                if (CurrentChar == FileEnd)
                {
                    // Достигнут конец файла
                    Close();
                }
                else if (CurrentChar == NewString)
                {
                    StringNum++;
                    PlaceInString = 0;
                }
                else if (CurrentChar == Back || CurrentChar == Tab)
                {
                    NextChartReading();
                }
                else
                {
                    PlaceInString++;
                }
            }
            else
            {
                Close();
            }

        }
    

        public static void Close()
        {
            streamReader?.Close();
        }
    }
}
