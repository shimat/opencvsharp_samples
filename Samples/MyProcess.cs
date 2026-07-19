using System.Diagnostics;

namespace SampleBase;

/// <summary>
///
/// </summary>
public static class MyProcess
{
    /// <summary>
    /// Physical memory usage
    /// </summary>
    /// <returns></returns>
    public static long WorkingSet64
    {
        get
        {
            using var proc = GetCurrentProcess();
            return proc.WorkingSet64;
        }
    }

    /// <summary>
    /// Virtual memory usage
    /// </summary>
    /// <returns></returns>
    public static long VirtualMemorySize64
    {
        get
        {
            using var proc = GetCurrentProcess();
            return proc.VirtualMemorySize64;
        }
    }

    /// <summary>
    /// Peak physical memory usage
    /// </summary>
    /// <returns></returns>
    public static long PeakPagedMemorySize64
    {
        get
        {
            using var proc = GetCurrentProcess();
            return proc.PeakPagedMemorySize64;
        }
    }

    /// <summary>
    /// Peak virtual memory usage
    /// </summary>
    /// <returns></returns>
    public static long PeakVirtualMemorySize64
    {
        get
        {
            using var proc = GetCurrentProcess();
            return proc.PeakVirtualMemorySize64;
        }
    }

    private static Process GetCurrentProcess()
    {
        var proc = Process.GetCurrentProcess();
        proc.Refresh();
        return proc;
    }
}
