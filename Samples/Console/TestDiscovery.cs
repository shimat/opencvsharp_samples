using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SampleBase.Interfaces;

namespace SampleBase.Console
{
    /// <summary>
    /// Discovers <see cref="ITestBase"/> implementations via reflection, so that new samples
    /// are picked up automatically without hand-editing a registration list.
    /// </summary>
    public static class TestDiscovery
    {
        public static IEnumerable<ITestBase> DiscoverTests(Assembly? assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly();

            return assembly.GetTypes()
                .Where(t => typeof(ITestBase).IsAssignableFrom(t) && t is { IsClass: true, IsAbstract: false })
                .Select(t =>
                {
                    var ctor = t.GetConstructor(Type.EmptyTypes)
                        ?? throw new InvalidOperationException(
                            $"{t.Name} implements {nameof(ITestBase)} but has no public parameterless constructor.");
                    return (ITestBase)ctor.Invoke(null);
                });
        }
    }
}
