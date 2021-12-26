using SampleBase.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SampleBase
{
    public class ConsoleTestManager : ITestManager
    {
        private readonly List<ITestBase> tests;
        private readonly IMessagePrinter msgPrinter;

        public ConsoleTestManager()
        {
            tests = new List<ITestBase>();
            msgPrinter = new ConsoleMessagePrinter();
        }

        public void AddTest(ITestBase test)
        {
            if (!tests.Contains(test))
                tests.Add(test);
        }

        public void RemoveTest(ITestBase test)
        {
            tests.Remove(test);
        }

        public void ClearTests()
        {
            tests.Clear();
        }

        public virtual void ShowTestNames()
        {
            msgPrinter.PrintLine();
            int testNumber = 1;
            foreach (var x in tests)
            {
                string name = GetNameOfTest(x);

                msgPrinter.PrintInfo($"{testNumber} {name}");
                testNumber++;

            };
            msgPrinter.PrintLine();
        }

        #region ShowTestEntrance

        private const int exitCode = 0;
        private const string inputClear = "c";
        private const string inputHelp = "h";
        private static readonly string helpMessage =
            $"Follow these steps to use the testing framework: " +
            $"1 Create class that inherit from the [{nameof(ITestBase)}]{Environment.NewLine}" +
            $"2 Override the [{nameof(ConsoleTestBase.RunTest)}()] method of the class to execute your logic{Environment.NewLine}" +
            $"3 Manage all the test classes by an instance that inherits from {nameof(ITestManager)}{Environment.NewLine}" +
            $"4 Start the tests selection by runnig the [{nameof(ShowTestEntrance)}()] method of the ITestManager instance{Environment.NewLine}";

        /// <summary>
        /// Output prompt message and start reading input (start again)
        /// </summary>
        private string? PrintNamesAndRead()
        {
            msgPrinter.PrintSuccess(
                $"Please enter a number to select the test to run.{Environment.NewLine}Enter {exitCode} to exit, Enter {inputClear} to clear history, Enter {inputHelp} to show help info.");
            ShowTestNames();
            return Console.ReadLine();
        }

        /// <summary>
        /// Output error message and re-read input
        /// </summary>
        /// <param name="message"></param>
        private string? PrintErrorAndRead(string message)
        {
            msgPrinter.PrintError(message);
            return Console.ReadLine();
        }

        private string GetNameOfTest(object test)
        {
            var name = "";
            if (test is ITestBase testA)
                name = testA.Name;
            if (test is Func<ITestBase> fun)
                name = fun().Name;

            return name;
        }

        public virtual void ShowTestEntrance()
        {
            var input = PrintNamesAndRead();

            while (true)
            {
                if (input?.ToLower() == inputClear)
                {
                    Console.Clear();
                    PrintNamesAndRead();
                    continue;
                }
                if (input?.ToLower() == inputHelp)
                {
                    msgPrinter.PrintSuccess(helpMessage);
                    PrintNamesAndRead();
                    continue;
                }
                if (int.TryParse(input, out int number))
                {
                    if (number == exitCode)
                        break;

                    if (number < 0 || number > tests.Count)
                    {
                        PrintErrorAndRead($"The number is out of range. Please reenter(enter {exitCode} to exit）");
                        continue;
                    }
                    var test = tests[number - 1];
                    var testName = GetNameOfTest(test);
                    msgPrinter.PrintSuccess($"{testName} start executing...");

                    try
                    {
                        var watch = Stopwatch.StartNew();
                        test.RunTest();
                        watch.Stop();
                        msgPrinter.PrintSuccess($"{testName} completed, time cost:{watch.ElapsedMilliseconds}ms\n");
                    }
                    catch (Exception ex)
                    {
                        msgPrinter.PrintError(ex.Message);
                        msgPrinter.PrintError(ex.StackTrace ?? "");
                    }

                    input = PrintNamesAndRead();

                }
                else
                {
                    input = PrintErrorAndRead($"The input({input}) is invalid. Please reenter(enter {exitCode} to exit）");
                }
            }
        }

        #endregion

        public ITestBase? GetTest(string testName)
        {
            return tests.FirstOrDefault(t => t.Name == testName);
        }

        public IReadOnlyList<ITestBase> GetAllTests()
        {
            return tests;
        }
    }
}
