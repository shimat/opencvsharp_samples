using Sample.Test.Interfaces;
using System;

namespace Sample.Test
{
    public class ConsoleMessagePrinter : IMessagePrinter
    {

        public ConsoleMessagePrinter()
        {
            
        }

        public void PrintColorInfo(string message, bool newLine = true, ConsoleColor consoleColor = ConsoleColor.White)
        {
            PrintObject(message, newLine, consoleColor);
        }

        public void PrintObject(object obj, bool newLine = true, ConsoleColor consoleColor = ConsoleColor.White)
        {
            Console.ForegroundColor = consoleColor;
            if (newLine)
                Console.WriteLine(obj);
            else
                Console.Write(obj);
            Console.ResetColor();
        }

        public  void PrintInfo(string message, bool newLine = true)
        {
            PrintColorInfo(message,newLine);
        }

        public  void PrintWarning(string message, bool newLine = true)
        {
            PrintColorInfo(message,newLine,ConsoleColor.Yellow);
        }

        public void PrintError(string message, bool newLine = true)
        {
            PrintColorInfo(message, newLine, ConsoleColor.DarkRed);
        }

        public void PrintSuccess(string message, bool newLine = true)
        {
            PrintColorInfo(message, newLine, ConsoleColor.DarkGreen);
        }

        public void PrintDateTime(DateTime? time, bool newLine = true)
        {
            if (!time.HasValue)
                time = DateTime.Now;
            PrintColorInfo(time.Value.ToString("yyyy-MM-dd HH:mm:ss"), newLine);
        }
        
        public void PrintTime(DateTime? time, bool newLine = true)
        {
            if (!time.HasValue)
                time = DateTime.Now;
            PrintColorInfo(time.Value.ToString("HH:mm:ss"));
        }

        public void PrintLine(bool newLine = true)
        {
            PrintColorInfo("----------------------------------------", newLine);
        }

    }
}
