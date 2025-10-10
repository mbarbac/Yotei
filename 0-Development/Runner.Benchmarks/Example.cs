#pragma warning disable IDE0059

namespace Runner.Benchmarks;

// ========================================================
//[DisassemblyDiagnoser]
[MemoryDiagnoser(displayGenColumns: false)]
[HideColumns("Job", "Error", "StdDev", "Median")]
public partial class Example
{
    [Benchmark]
    public void Split_Empty()
    {
        var source = ".ab . . cd.";
        var items = source.Split('.');
    }

    [Benchmark]
    public void Split_Full()
    {
        var source = ".ab . . cd.";
        var items = source.Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    }
}
