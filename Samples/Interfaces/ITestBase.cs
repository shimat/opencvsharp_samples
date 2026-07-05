namespace SampleBase.Interfaces
{
    /// <summary>
    /// Basic interface for test classes. Implemented by <see cref="Console.ConsoleTestBase"/>,
    /// which also provides Print*/WaitTo* helpers that aren't part of this contract.
    /// </summary>
    public interface ITestBase
    {
        /// <summary>
        /// Test name, which is used to distinguish between different test cases
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Run current test
        /// </summary>
        /// <param name="display">Per-run helper for showing results; see <see cref="DisplayHelper"/>.</param>
        void RunTest(DisplayHelper display);

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
