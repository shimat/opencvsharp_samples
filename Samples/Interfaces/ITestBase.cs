using System;

namespace SampleBase.Interfaces
{
    /// <summary>
    /// Basic interface for test calsses
    /// </summary>
    public interface ITestBase
    {
        /// <summary>
        /// Test name, which is used to distinguish between different test cases
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Print normal message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="newLine"></param>
        void PrintInfo(string message, bool newLine = true);

        /// <summary>
        /// Print onject info
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="newLine"></param>
        /// <param name="consoleColor"></param>
        void PrintObject(object obj, bool newLine = true, ConsoleColor consoleColor = ConsoleColor.White);

        /// <summary>
        /// Print warnning message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="newLine"></param>
        void PrintWarning(string message, bool newLine = true);

        /// <summary>
        /// Print error message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="newLine"></param>
        void PrintError(string message, bool newLine = true);

        /// <summary>
        /// Print success message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="newLine"></param>
        void PrintSuccess(string message, bool newLine = true);

        /// <summary>
        /// Print message printer of current test class
        /// </summary>
        /// <returns></returns>
        IMessagePrinter GetMessagePrinter();

        /// <summary>
        /// Run current test
        /// </summary>
        void RunTest();

        /// <summary>
        /// Waiting for input to complete, and take it as return value
        /// </summary>
        /// <returns></returns>
        string? WaitToInput();

        /// <summary>
        /// Show a tip message and wait util input anything
        /// </summary>
        /// <param name="tip">Information string to be shown</param>
        void WaitToContinue(string? tip = null);
    }
}
