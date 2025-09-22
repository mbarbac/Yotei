namespace Benchmark;

// ========================================================
//[DisassemblyDiagnoser]
[MemoryDiagnoser(displayGenColumns: false)]
[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD", "y")]
public partial class Test_StringSplitter
{
    [Benchmark]
    public void StringSplit() => _ = ".ab . . cd.".Split('.');

    [Benchmark]
    public void StringSplitter()
    {
        var options = new StringSplitterOptions();
        var iter = ".ab . . cd.".Split(options, ".");
        foreach (var (_, _) in iter) ;
    }
}