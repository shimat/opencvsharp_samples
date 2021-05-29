using System;
using System.Collections.Generic;

namespace Sample.Test.Interfaces
{
    /// <summary>
    /// Basic interface for manager class of test calsses
    /// </summary>
    public interface ITestManager
    {
        /// <summary>
        /// Add a <see cref="ITestBase"/> to the management collection
        /// </summary>
        /// <param name="test"></param>
        void AddTest(ITestBase test);

        /// <summary>
        /// Add a <see cref="Func"/> to the management collection
        /// </summary>
        /// <param name="test"></param>
        void AddTest(Func<ITestBase> test);

        /// <summary>
        /// Remove a <see cref="ITestBase"/> from the management collection
        /// </summary>
        /// <param name="test"></param>
        void RemoveTest(ITestBase test);

        /// <summary>
        /// Remove all <see cref="ITestBase"/> in management collection
        /// </summary>

        void ClearTests();

        /// <summary>
        /// Show a list of all tests
        /// </summary>
        void ShowTestNames();

        /// <summary>
        /// Show a acess list of all tests
        /// </summary>
        void ShowTestEntrance();

        /// <summary>
        /// Get a <see cref="ITestBase"/> by test name
        /// </summary>
        /// <param name="testName"></param>
        /// <returns></returns>
        ITestBase GetTest(string testName);

        /// <summary>
        /// Get all <see cref="ITestBase"/> by test name
        /// </summary>
        /// <returns></returns>
        IList<ITestBase> GetAllTests();
    }
}
