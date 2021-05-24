using Sample.Test.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Test
{

    public class ConsoleTestManager : ITestManager
    {
        IList<Object> _tests;

        IMessagePrinter _msgPrinter;

        public ConsoleTestManager()
        {
            _tests = new List<Object>();
            _msgPrinter = new ConsoleMessagePrinter();
        }

        public void AddTest(ITestBase test)
        {
            if (!_tests.Contains(test))
                _tests.Add(test);
        }

        public void AddTest(Func<ITestBase> test)
        {
            if (!_tests.Contains(test))
                _tests.Add(test);
        }

        public void RemoveTest(ITestBase test)
        {
            _tests.Remove(test);
        }


        public void ClearTests()
        {
            _tests.Clear();
        }

        public virtual void ShowTestNames()
        {
            _msgPrinter.PrintLine();
            int testNumber = 1;
            _tests.ToList().ForEach(x => 
            {
                string name = GetNameOfTest(x);

                _msgPrinter.PrintInfo($"{testNumber} {name}");
                testNumber++;

            });
            _msgPrinter.PrintLine();
        }
        
        #region ShowTestEntrance

        int _exitCode = 0;
        string _input;
        string _inputClear = "c";
        string _inputHelp = "h";
        string _helpMessage =
            $"1 Create class that inherit from the [{nameof(ITestBase)}]{Environment.NewLine}" +
            $"2 Override the [{nameof(ConsoleTestBase.RunTest)}()] method of the class to execute your logic{Environment.NewLine}" +
            $"3 Manage all the test classes by an instance that inherits from {nameof(ITestManager)}{Environment.NewLine}" +
            $"4 Start the tests selection by runnig the [{nameof(ShowTestEntrance)}()] method of the ITestManager instance{Environment.NewLine}";


        /// <summary>
        /// 输出提示信息并开始读取输入(重新开始)
        /// </summary>
        private void PrintNamesAndRead()
        {
            _msgPrinter.PrintSuccess($"Please enter a number to select the test to run.(Enter{_exitCode} to exit, Enter{_inputClear} to clear history,Enter{_inputHelp} to show help info.)");
            ShowTestNames();
            _input = Console.ReadLine();
        }

        /// <summary>
        /// 输出错误信息并重新读取输入
        /// </summary>
        /// <param name="message"></param>
        private void PrintErrorAndRead(string message)
        {
            _msgPrinter.PrintError(message);
            _input = Console.ReadLine();
        }

        private string GetNameOfTest(object test)
        {
            string name = "";
            if (test is ITestBase testA)
                name = testA.Name;
            if (test is Func<ITestBase> fun)
                name = fun().Name;

            return name;
        }

        public virtual void ShowTestEntrance()
        {
            PrintNamesAndRead();

            while (true)
            {
                if (_input.ToLower() == _inputClear)
                {
                    Console.Clear();
                    PrintNamesAndRead();
                    continue;
                }
                if (_input.ToLower() == _inputHelp)
                {
                    _msgPrinter.PrintSuccess(_helpMessage);
                    PrintNamesAndRead();
                    continue;
                }
                if (int.TryParse(_input, out int number))
                {
                    if (number == _exitCode)
                        break;

                    if (number < 0 || number > _tests.Count)
                    {
                        PrintErrorAndRead($"The number is out of range.Please reenter(enter{_exitCode} to exit）");
                        continue;
                    }
                    var test = _tests.Skip(number - 1).FirstOrDefault();
                    var testName = GetNameOfTest(test);
                    _msgPrinter.PrintSuccess($"{testName} start executing...");

                    try
                    {
                        DateTime start = DateTime.Now;
                        Task task = null;
                        if (test is ITestBase testA)
                            task = testA.RunTest();

                        if (test is Func<ITestBase> func)
                            task = func().RunTest();

                        Task.WaitAll(new Task[] { task });
                        _msgPrinter.PrintSuccess($"{testName} completed,time cost:{DateTime.Now.Subtract(start).TotalMilliseconds.ToString("0.0000")}ms\n");
                    }
                    catch (Exception ex)
                    {
                        _msgPrinter.PrintError(ex.Message);
                        _msgPrinter.PrintError(ex.StackTrace);
                    }

                    PrintNamesAndRead();

                }
                else
                {
                    PrintErrorAndRead($"The input is invalid.Please reenter(enter{_exitCode} to exit）");
                }
            }
        }

        #endregion

        public ITestBase GetTest(string testName)
        {
            var test = _tests.Select(x => x as ITestBase).Where(x => x != null && x.Name == testName).FirstOrDefault();
            if(test is null)
            {
                _tests.Select(x => x as Func<ITestBase>)
                    .Where(x => x != null).ToList().ForEach(x=> 
                    {
                        var t = x.Invoke();
                        if(t.Name == testName)
                        {
                            test = t;
                            return;
                        }
                    });
            }
            return test;
        }

        public IList<ITestBase> GetAllTests()
        {
            List<ITestBase> list = new List<ITestBase>();

            var tests = _tests.Select(x => x as ITestBase).Where(x => x != null);
            list.AddRange(tests);

            var funcs = _tests.Select(x => x as Func<ITestBase>).Where(x => x != null).ToList();
            funcs.ForEach(x => { list.Add(x.Invoke()); });

            return list;
        }
    }
}
