namespace Experimental.Tests;

// ========================================================
//[Enforced]
public static class Test_StringBuilderPool
{
    private static readonly Random Random = new();
    private const string Alphabet = "abcdefghijklmnopqrstuvxyz0123456789";

    private const int Loops = 1000;
    private const int MinLen = 5; // Needs to be enough to print the loop number.
    private const int MaxLen = 4000;

    //[Enforced]
    //[Fact]
    public static void Test_Produces_Memory_Leak()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        var inimem = GC.GetTotalMemory(true);

#if !PARALLEL
        for (int i = 0; i < Loops; i++)
        {
            var sb = StringBuilderPool.Rent();
            sb.Append(Create(i, MinLen, MaxLen));
            _ = StringBuilderPool.ToStringAndReturn(sb);
        }
#else
        var options = new ParallelOptions { MaxDegreeOfParallelism = 4 };
        Parallel.For(0, Loops, options, i =>
        {
            var sb = StringBuilderPool.Rent();
            sb.Append(Create(i, MinLen, MaxLen));
            _ = StringBuilderPool.ToStringAndReturn(sb);
        });
#endif
        StringBuilderPool.Clear();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        var endmem = GC.GetTotalMemory(true);

        var delta = endmem - inimem;
        Debug.WriteLine($"Memory delta: {delta}.");
        Assert.True(delta < 1024, "Memory leak.");
    }

    [SuppressMessage("", "IDE0180")]
    public static string Create(int loop, int min, int max)
    {
        int len = Random.Next(min, max);
        var span = new char[len];

        var index = 0;
        if (loop == 0)
        {
            span[0] = '0';
            span[1] = ':';
            index = 2;
        }
        else
        {
            while (loop > 0)
            {
                span[index] = (char)('0' + loop % 10);
                loop /= 10;
                index++;
            }

            int end = 0; while (end < span.Length && span[end] != '\0') end++;
            int left = 0;
            int right = end - 1;
            while (left < right)
            {
                var temp = span[left];
                span[left] = span[right];
                span[right] = temp;

                left++;
                right--;
            }

            span[index] = ':';
            index++;
        }

        for (int i = index; i < len; i++) span[i] = Alphabet[Random.Next(Alphabet.Length)];
        return new string(span);
    }
}