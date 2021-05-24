using Sample.Test.Interfaces;
using System;
using System.Threading.Tasks;

namespace Sample.Test
{
    public abstract class ConsoleTestBase : ITestBase
    {
        IMessagePrinter _msgPrinter;

        public string Name { get; set; }

        public ConsoleTestBase(string title)
        {
            Name = title;
            _msgPrinter = new ConsoleMessagePrinter();
        }

        public ConsoleTestBase()
        {
            Name = this.GetType().Name;
            _msgPrinter = new ConsoleMessagePrinter();
        }

        public  void PrintInfo(string message, bool newLine = true)
        {
            _msgPrinter.PrintInfo(message, newLine);
        }

        public  void PrintObject(object obj, bool newLine = true, ConsoleColor consoleColor = ConsoleColor.White)
        {
            _msgPrinter.PrintObject(obj, newLine, consoleColor);
        }
        public  void PrintWarning(string message, bool newLine = true)
        {
            _msgPrinter.PrintWarning(message, newLine);
        }

        public void PrintError(string message, bool newLine = true)
        {
            _msgPrinter.PrintError(message, newLine);
        }

        public void PrintSuccess(string message, bool newLine = true)
        {
            _msgPrinter.PrintSuccess(message, newLine);
        }

        public IMessagePrinter GetMessagePrinter()
        {
            return _msgPrinter;
        }

        public abstract void RunTest();


        public string WaitToInput()
        {
            return Console.ReadLine();
        }

        public void WaitToContinue(string tip = null)
        {
            if (tip != null)
                Console.WriteLine(tip);
            Console.ReadLine();
        }
    }
}
