namespace Experimental.Benchmarks;

// ========================================================
internal class Program
{
    static void Main(string[] args)
        => BenchmarkSwitcher.FromAssembly(Assembly.GetCallingAssembly()).Run(args);
}
