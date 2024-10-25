using BenchmarkDotNet.Running;

namespace ToDo_Api.PerformanceTests;

/// <summary>
/// Class responsible for performance tests using <see cref="BenchmarkDotNet"/>.
/// </summary>
internal class Startup
{
    private static void Main()
    {
        BenchmarkRunner.Run<ToDoApiPerformanceTests>();
        Console.ReadLine();
    }
}
