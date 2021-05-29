using SampleBase.Interfaces;
using System;

namespace SampleBase
{
    public abstract class ConsoleTestBase : ITestBase
    {
        private readonly IMessagePrinter msgPrinter;

        public string Name { get; set; }

        protected ConsoleTestBase(string title)
        {
            Name = title;
            msgPrinter = new ConsoleMessagePrinter();
        }

        protected ConsoleTestBase()
        {
            Name = GetType().Name;
            msgPrinter = new ConsoleMessagePrinter();
        }

        public abstract void RunTest();

        public void PrintInfo(string message, bool newLine = true)
        {
            msgPrinter.PrintInfo(message, newLine);
        }

        public void PrintObject(object obj, bool newLine = true, ConsoleColor consoleColor = ConsoleColor.White)
        {
            msgPrinter.PrintObject(obj, newLine, consoleColor);
        }

        public void PrintWarning(string message, bool newLine = true)
        {
            msgPrinter.PrintWarning(message, newLine);
        }

        public void PrintError(string message, bool newLine = true)
        {
            msgPrinter.PrintError(message, newLine);
        }

        public void PrintSuccess(string message, bool newLine = true)
        {
            msgPrinter.PrintSuccess(message, newLine);
        }

        public IMessagePrinter GetMessagePrinter()
        {
            return msgPrinter;
        }

        public string WaitToInput()
        {
            return Console.ReadLine();
        }

        public void WaitToContinue(string? tip = null)
        {
            if (tip != null)
                Console.WriteLine(tip);
            Console.ReadLine();
        }
    }
}
