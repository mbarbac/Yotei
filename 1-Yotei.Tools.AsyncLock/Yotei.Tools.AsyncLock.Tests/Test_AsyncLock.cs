using static Yotei.Tools.ConsoleExtensions;
using static System.Console;
using static System.ConsoleColor;

namespace Yotei.Tools.AsyncLock.Tests;

// ========================================================
//[Enforced]
public static class Test_AsyncLock
{
    static readonly ConsoleColor ForeColor = White;

    static void WaitOrThrow(Task[] tasks)
    {
        while (true)
        {
            var done = 0;
            foreach (var task in tasks)
            {
                if (task.IsCompleted) { done++; continue; }
                if (task.IsFaulted) throw task.Exception ?? new Exception("Unknown exception");
            }

            if (done == tasks.Length) break;
        }
    }

    // ----------------------------------------------------

    static readonly int NUM = 4;
    static readonly int DEEP = 3;
    static readonly int WAIT = 200;
    static readonly int TIMEOUT = WAIT * (NUM + DEEP);

    class Info
    {
        public override string ToString() => Syncroot.ToString();
        public AsyncLock Syncroot = new();
        public bool Captured = false;

        public Random Random = new();
        public int Value = 0;
        public int NewValue()
        {
            while (true)
            {
                var temp = Random.Next(1, 99);
                if (temp != Value) return temp;
            }
        }
    }

    // ----------------------------------------------------

    static void Do_SyncSync(Info info, string id, int level, int timeout)
    {
        WriteLineEx(true, ForeColor, $"Entering:'{id}' root:'{info}'");

        var span = TimeSpan.FromMilliseconds(timeout);
        try
        {
            using var disp = info.Syncroot.Enter(span, id);

            var old = info.Value;
            var temp = info.Value = info.NewValue();

            if (level < DEEP)
            {
                level++;
                var str = $"{id}.{level}";
                Do_SyncSync(info, str, level, timeout);

                if (info.Value != temp)
                    throw new Exception($"Value:{info.Value}, Old:{old}, Temp:{temp}");
            }

            Thread.Sleep(WAIT);
            info.Value = old;
        }
        catch (TimeoutException)
        {
            if (timeout >= 0)
            {
                WriteLineEx(true, Red, $"*** Timeout at:'{id}' root:{info}");
                info.Captured = true;
            }
            else throw;
        }
    }

    //[Enforced]
    [Fact]
    public static void Test_SyncSync()
    {
        var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Task.Run(
                () => Do_SyncSync(info, str, 0, -1),
                TestContext.Current.CancellationToken);

            tasks.Add(task);
        }
        WaitOrThrow([.. tasks]);
    }

    //[Enforced]
    [Fact]
    public static void Test_SyncSync_Timeout()
    {
        var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Task.Run(
                () => Do_SyncSync(info, str, 0, TIMEOUT),
                TestContext.Current.CancellationToken);
            
            tasks.Add(task);
        }
        WaitOrThrow([.. tasks]);

        if (!info.Captured)
        {
            var str = "*** Timeout exceptions expected but not captured!";
            WriteLineEx(true, Red, str);
            throw new UnExpectedException(str);
        }
    }

    // ----------------------------------------------------

    static async Task Do_AsyncAsync(Info info, string id, int level, int timeout)
    {
        WriteLineEx(true, ForeColor, $"Entering:'{id}' root:'{info}'");

        var span = TimeSpan.FromMilliseconds(timeout);
        try
        {
            await using var disp = await info.Syncroot.EnterAsync(span, id).ConfigureAwait(false);

            var old = info.Value;
            var temp = info.Value = info.NewValue();

            if (level < DEEP)
            {
                level++;
                var str = $"{id}.{level}";
                await Do_AsyncAsync(info, str, level, timeout).ConfigureAwait(false);

                if (info.Value != temp)
                    throw new Exception($"Value:{info.Value}, Old:{old}, Temp:{temp}");
            }

            await Task.Delay(WAIT).ConfigureAwait(false);
            info.Value = old;
        }
        catch (TimeoutException)
        {
            if (timeout >= 0)
            {
                WriteLineEx(true, Red, $"*** Timeout at:'{id}' root:{info}");
                info.Captured = true;
            }
            else throw;
        }
    }

    //[Enforced]
    [Fact]
    public static void Test_AsyncAsync()
    {
        var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Task.Run(
                async () => await Do_AsyncAsync(info, str, 0, -1),
                TestContext.Current.CancellationToken);

            tasks.Add(task);
        }
        WaitOrThrow([.. tasks]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AsyncAsync_Timeout()
    {
        var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Task.Run(
                async () => await Do_AsyncAsync(info, str, 0, TIMEOUT),
                TestContext.Current.CancellationToken);

            tasks.Add(task);
        }
        WaitOrThrow([.. tasks]);

        if (!info.Captured)
        {
            var str = "*** Timeout exceptions expected but not captured!";
            WriteLineEx(true, Red, str);
            throw new UnExpectedException(str);
        }
    }

    // ----------------------------------------------------

    static void Do_SyncAsync(Info info, string id, int level, int timeout)
    {
        WriteLineEx(true, ForeColor, $"Entering:'{id}' root:'{info}'");

        var span = TimeSpan.FromMilliseconds(timeout);
        try
        {
            using var disp = info.Syncroot.Enter(span, id);

            var old = info.Value;
            var temp = info.Value = info.NewValue();

            if (level < DEEP)
            {
                level++;
                var str = $"{id}.{level}";
                var task = Do_AsyncSync(info, str, level, timeout);
                task.GetAwaiter().GetResult();

                if (info.Value != temp)
                    throw new Exception($"Value:{info.Value}, Old:{old}, Temp:{temp}");
            }

            Thread.Sleep(WAIT);
            info.Value = old;
        }
        catch (TimeoutException)
        {
            if (timeout >= 0)
            {
                WriteLineEx(true, Red, $"*** Timeout at:'{id}' root:{info}");
                info.Captured = true;
            }
            else throw;
        }
    }

    //[Enforced]
    [Fact]
    public static void Test_SyncAsync()
    {
        var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Task.Run(
                () => Do_SyncAsync(info, str, 0, -1),
                TestContext.Current.CancellationToken);

            tasks.Add(task);
        }
        WaitOrThrow([.. tasks]);
    }

    //[Enforced]
    [Fact]
    public static void Test_SyncAsync_Timeout()
    {
        var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Task.Run(
                () => Do_SyncAsync(info, str, 0, TIMEOUT),
                TestContext.Current.CancellationToken);

            tasks.Add(task);
        }
        WaitOrThrow([.. tasks]);

        if (!info.Captured)
        {
            var str = "*** Timeout exceptions expected but not captured!";
            WriteLineEx(true, Red, str);
            throw new UnExpectedException(str);
        }
    }

    // ----------------------------------------------------

    static async Task Do_AsyncSync(Info info, string id, int level, int timeout)
    {
        WriteLineEx(true, ForeColor, $"Entering:'{id}' root:'{info}'");

        var span = TimeSpan.FromMilliseconds(timeout);
        try
        {
            await using var disp = await info.Syncroot.EnterAsync(span, id).ConfigureAwait(false);

            var old = info.Value;
            var temp = info.Value = info.NewValue();

            if (level < DEEP)
            {
                level++;
                var str = $"{id}.{level}";
                await Task.Run(() => Do_SyncAsync(info, str, level, timeout)).ConfigureAwait(false);

                if (info.Value != temp)
                    throw new Exception($"Value:{info.Value}, Old:{old}, Temp:{temp}");
            }

            await Task.Delay(WAIT).ConfigureAwait(false);
            info.Value = old;
        }
        catch (TimeoutException)
        {
            if (timeout >= 0)
            {
                WriteLineEx(true, Red, $"*** Timeout at:'{id}' root:{info}");
                info.Captured = true;
            }
            else throw;
        }
    }

    //[Enforced]
    [Fact]
    public static void Test_AsyncSync()
    {
        var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Task.Run(
                () => Do_AsyncSync(info, str, 0, -1),
                TestContext.Current.CancellationToken);

            tasks.Add(task);
        }
        WaitOrThrow([.. tasks]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AsyncSync_Timeout()
    {
        var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Task.Run(
                () => Do_AsyncSync(info, str, 0, TIMEOUT),
                TestContext.Current.CancellationToken);

            tasks.Add(task);
        }
        WaitOrThrow([.. tasks]);

        if (!info.Captured)
        {
            var str = "*** Timeout exceptions expected but not captured!";
            WriteLineEx(true, Red, str);
            throw new UnExpectedException(str);
        }
    }
}