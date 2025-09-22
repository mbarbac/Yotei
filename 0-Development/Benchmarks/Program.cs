namespace Benchmarks;

// ========================================================
internal class Program
{
    public static void Main(string[] args)
        => BenchmarkSwitcher.FromAssembly(Assembly.GetCallingAssembly()).Run(args);
}