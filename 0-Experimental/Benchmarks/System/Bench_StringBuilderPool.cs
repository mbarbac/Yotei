using Microsoft.Extensions.ObjectPool;

namespace Benchmarks;

// ========================================================
[DisassemblyDiagnoser]
[MemoryDiagnoser(displayGenColumns: false)]
[HideColumns("Job", "Error", "StdDev", "Median")]
public partial class Bench_StringBuilderPool
{
    static void UseValue(string str) { if (str.Length == 0) throw new Exception(); }

    [SuppressMessage("", "CA1822")]
    [Benchmark]
    public void Standard()
    {
        for (int i = 0; i < 100_000; i++)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Example #{i}");
            UseValue(builder.ToString());
        }
    }

    [SuppressMessage("", "CA1822")]
    [Benchmark]
    public void Pooled()
    {
        for (int i = 0; i < 100_000; i++)
        {
            var builder = pool.Get();
            builder.AppendLine($"Example #{i}");
            UseValue(builder.ToString());
            pool.Return(builder);
        }
    }
    static readonly StringBuilderPooledObjectPolicy policy = new();
    static readonly DefaultObjectPool<StringBuilder> pool = new(policy);

    [SuppressMessage("", "CA1822")]
    [Benchmark]
    public void DisposableBuilder()
    {
        for (int i = 0; i < 100_000; i++)
        {
            var builder = new DisposableStringBuilder();
            builder.AppendLine($"Example #{i}");
            UseValue(builder.ToString());
        }
    }
}