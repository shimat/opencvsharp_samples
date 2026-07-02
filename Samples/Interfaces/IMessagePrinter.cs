using System;

namespace SampleBase.Interfaces
{
    public interface IMessagePrinter
    {
        void PrintColorInfo(string message, bool newLine = true, ConsoleColor consoleColor = ConsoleColor.White);

        void PrintObject(object obj, bool newLine = true, ConsoleColor consoleColor = ConsoleColor.White);

        void PrintInfo(string message, bool newLine = true);

        void PrintWarning(string message, bool newLine = true);

        void PrintError(string message, bool newLine = true);

        void PrintSuccess(string message, bool newLine = true);

        void PrintDateTime(DateTime? time, bool newLine = true);

        void PrintTime(DateTime? time, bool newLine = true);

        void PrintLine(bool newLine = true);
    }
}
