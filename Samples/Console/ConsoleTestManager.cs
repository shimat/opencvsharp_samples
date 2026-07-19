using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SampleBase.Interfaces;

namespace SampleBase.Console;

public class ConsoleTestManager
{
    private readonly List<ITestBase> tests = [];
    private readonly ConsoleMessagePrinter msgPrinter;
    private readonly bool isHeadless;

    public ConsoleTestManager(bool isHeadless = false)
    {
        msgPrinter = new ConsoleMessagePrinter();
        this.isHeadless = isHeadless;
    }

    public void AddTest(ITestBase test)
    {
        if (!tests.Contains(test))
            tests.Add(test);
    }

    public void AddTests(params ITestBase[] tests)
    {
        foreach (var test in tests)
        {
            AddTest(test);
        }
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
        SampleCategory? currentCategory = null;
        foreach (var x in tests)
        {
            var category = TestDiscovery.GetCategory(x);
            if (category != currentCategory)
            {
                msgPrinter.PrintInfo($"[{category}]");
                currentCategory = category;
            }

            string name = GetNameOfTest(x);
            msgPrinter.PrintInfo($"  {testNumber} {name}");
            testNumber++;

        };
        msgPrinter.PrintLine();
    }

    #region ShowTestEntrance

    private const int exitCode = 0;
    private const string inputClear = "c";
    private const string inputHelp = "h";
    private static readonly string helpMessage =
        $"""
        Follow these steps to use the testing framework:
        1 Create a class that inherits from {nameof(ConsoleTestBase)}
        2 Override the [{nameof(ConsoleTestBase.RunTest)}()] method of the class to execute your logic
        3 Register it with a {nameof(ConsoleTestManager)} instance via {nameof(AddTest)}/{nameof(AddTests)}
        4 Start the tests selection by running the [{nameof(ShowTestEntrance)}()] method of the {nameof(ConsoleTestManager)} instance
        """;

    /// <summary>
    /// Output prompt message and start reading input (start again)
    /// </summary>
    private string? PrintNamesAndRead()
    {
        msgPrinter.PrintSuccess(
            $"Please enter a number to select the test to run.{Environment.NewLine}Enter {exitCode} to exit, Enter {inputClear} to clear history, Enter {inputHelp} to show help info.");
        ShowTestNames();
        return System.Console.ReadLine();
    }

    /// <summary>
    /// Output error message and re-read input
    /// </summary>
    /// <param name="message"></param>
    private string? PrintErrorAndRead(string message)
    {
        msgPrinter.PrintError(message);
        return System.Console.ReadLine();
    }

    private static string GetNameOfTest(object test) => test switch
    {
        ITestBase testA => testA.Name,
        Func<ITestBase> fun => fun().Name,
        _ => "",
    };

    public virtual void ShowTestEntrance()
    {
        var input = PrintNamesAndRead();

        while (true)
        {
            if (input?.ToLower() == inputClear)
            {
                // Console.Clear() throws when stdout is redirected (e.g. piped/CI).
                if (!System.Console.IsOutputRedirected)
                    System.Console.Clear();
                input = PrintNamesAndRead();
                continue;
            }
            if (input?.ToLower() == inputHelp)
            {
                msgPrinter.PrintSuccess(helpMessage);
                input = PrintNamesAndRead();
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

                var display = new DisplayHelper(testName, isHeadless);
                try
                {
                    var watch = Stopwatch.StartNew();
                    test.RunTest(display);
                    watch.Stop();
                    msgPrinter.PrintSuccess($"{testName} completed, time cost:{watch.ElapsedMilliseconds}ms\n");
                }
                catch (Exception ex)
                {
                    msgPrinter.PrintError(ex.Message);
                    msgPrinter.PrintError(ex.StackTrace ?? "");
                }
                finally
                {
                    display.DestroyAll();
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
