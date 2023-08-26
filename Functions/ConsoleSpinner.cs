using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlDataWebsiteToolBasic.Functions
{
    public class ConsoleSpinner
    {
        static string[,]? _sequence = null;

        private int Delay { get; set; } = 300;

        readonly int _totalSequences = 0;
        int _counter;

        public ConsoleSpinner()
        {
            _counter = 0;
            _sequence = new string[,] {
                { "/", "-", "\\", "|" },
                { ".", "o", "0", "o" },
                { "+", "x","+","x" },
                { "V", "<", "^", ">" },
                { ".   ", "..  ", "... ", "...." },
                { "=>   ", "==>  ", "===> ", "====>" },
               // ADD YOUR OWN CREATIVE SEQUENCE HERE IF YOU LIKE
            };

            _totalSequences = _sequence.GetLength(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayMsg">Message for display</param>
        /// <param name="sequenceCode"> 0 | 1 | 2 |3 | 4 | 5 </param>
        public void Turn(string displayMsg = "", int sequenceCode = 0)
        {
            _counter++;

            Thread.Sleep(Delay);

            sequenceCode = sequenceCode > _totalSequences - 1 ? 0 : sequenceCode;

            var counterValue = _counter % 4;

            var fullMessage = displayMsg + _sequence?[sequenceCode, counterValue];
            var msglength = fullMessage.Length;

            Console.Write(fullMessage);

            Console.SetCursorPosition(Console.CursorLeft - msglength, Console.CursorTop);
        }
    }
}
