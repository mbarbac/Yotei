namespace Yotei.Tools.Benchmarks;

// ========================================================
//[DisassemblyDiagnoser]
[MemoryDiagnoser(displayGenColumns: false)]
[HideColumns("Job", "Error", "StdDev", "Median")]
public partial class Bench_StringSplitter
{
    [Benchmark]
    public void Standard_No_Options()
    {
        var source = ".ab . . cd.";
        _ = source.Split('.');
    }

    [Benchmark]
    public void Splitter_No_Options()
    {
        var source = ".ab . . cd.";
        var options = new StringSplitter();
        _ = source.Split('.', options).ToArray(); // ToArray makes 21ns -> 109ns (+88ns)
    }

    [Benchmark]
    public void Splitter_Keep_Separators()
    {
        var source = ".ab . . cd.";
        var options = new StringSplitter() { KeepSeparators = true };
        _ = source.Split('.', options).ToArray();
    }
}