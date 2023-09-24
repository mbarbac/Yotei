using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Locker
{
#if DEBUG
    static int NUM = 5;
    static int DEEP = 5;
    static int WAIT = 100;
#else
    static int NUM = 10;
    static int DEEP = 10;
    static int WAIT = 150;
#endif
    static int TIMEOUT = WAIT * (DEEP + NUM / 2);

    public class Info : IDisposable, IAsyncDisposable
    {
        public Locker Locker = new Locker();
        public Random Random = new Random();
        public int Value = 0;
        public bool TimeoutCaptured = false;

        public void Dispose()
        {
            Locker.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await Locker.DisposeAsync().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        public int NewValue()
        {
            while (true)
            {
                var value = Random.Next(1, 99);
                if (value != Value) return value;
            }
        }

        public static void ThrowOrWaitAll(Task[] tasks)
        {
            while (true)
            {
                var done = 0; foreach (var task in tasks)
                {
                    if (task is null) { done++; continue; }
                    if (task.IsCompleted) { done++; continue; }
                    if (task.IsFaulted) throw task.Exception!;
                }
                if (done == tasks.Length) break;
            }
        }
    }

    // ====================================================

    static void Create_Sync(Info info, string id, int level, int timeout = -1)
    {
        Locker.Print(White, $"Entering  : {info.Locker}, Id: {id}");
        try
        {
            var span = TimeSpan.FromMilliseconds(timeout);
            using var disp = info.Locker.Lock(id, span);

            var old = info.Value;
            var temp = info.Value = info.NewValue();

            if (level < DEEP)
            {
                level++;
                var str = $"{id}.{level}";
                Create_Sync(info, str, level, timeout);

                if (info.Value != temp)
                    throw new Exception($"Value:{info.Value}, Old:{old}, Temp:{temp}");
            }

            Thread.Sleep(WAIT);
            info.Value = old;
            Locker.Print(Gray, $"Exiting   : {disp}");
        }
        catch (TimeoutException)
        {
            if (timeout >= 0)
            {
                info.TimeoutCaptured = true;
                Locker.Print(Red, $"** Timeout Exception: {info.Locker}, Id: {id}");
            }
            else throw;
        }
    }

    //[Enforced]
    [Fact]
    public static void Test_Sync()
    {
        using var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Task.Run(() => Create_Sync(info, str, 0, -1));
            tasks.Add(task);
        }
        Info.ThrowOrWaitAll(tasks.ToArray());
    }

    // ====================================================

    static async Task Create_Async(Info info, string id, int level, int timeout = -1)
    {
        Locker.Print(White, $"Entering  : {info.Locker}, Id: {id}");
        try
        {
            var span = TimeSpan.FromMilliseconds(timeout);
            await using var disp = await info.Locker.LockAsync(id, span).ConfigureAwait(false);

            var old = info.Value;
            var temp = info.Value = info.NewValue();

            if (level < DEEP)
            {
                level++;
                var str = $"{id}.{level}";
                await Create_Async(info, str, level).ConfigureAwait(false);

                if (info.Value != temp)
                    throw new Exception($"Value:{info.Value}, Old:{old}, Temp:{temp}");
            }

            await Task.Delay(WAIT).ConfigureAwait(false);
            info.Value = old;
            Locker.Print(Gray, $"Exiting   : {disp}");
        }
        catch (TimeoutException)
        {
            if (timeout >= 0)
            {
                info.TimeoutCaptured = true;
                Locker.Print(Red, $"** Timeout Exception: {info.Locker}, Id: {id}");
            }
            else throw;
        }
    }

    //[Enforced]
    [Fact]
    public static void Test_Async()
    {
        using var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Create_Async(info, str, 0, -1);
            tasks.Add(task);
        }
        Info.ThrowOrWaitAll(tasks.ToArray());
    }

    // ====================================================

    static void Create_Sync_Async(Info info, string id, int level, int timeout = -1)
    {
        Locker.Print(White, $"Entering  : {info.Locker}, Id: {id}");
        try
        {
            var span = TimeSpan.FromMilliseconds(timeout);
            using var disp = info.Locker.Lock(id, span);

            var old = info.Value;
            var temp = info.Value = info.NewValue();

            if (level < DEEP)
            {
                level++;
                var str = $"{id}.{level}";
                var task = Create_Async_Sync(info, str, level, timeout);
                task.GetAwaiter().GetResult();

                if (info.Value != temp)
                    throw new Exception($"Value:{info.Value}, Old:{old}, Temp:{temp}");
            }

            Thread.Sleep(WAIT);
            info.Value = old;
            Locker.Print(Gray, $"Exiting   : {disp}");
        }
        catch (TimeoutException)
        {
            if (timeout >= 0)
            {
                info.TimeoutCaptured = true;
                Locker.Print(Red, $"** Timeout Exception: {info.Locker}, Id: {id}");
            }
            else throw;
        }
    }

    static async Task Create_Async_Sync(Info info, string id, int level, int timeout = -1)
    {
        Locker.Print(White, $"Entering  : {info.Locker}, Id: {id}");
        try
        {
            var span = TimeSpan.FromMilliseconds(timeout);
            await using var disp = await info.Locker.LockAsync(id, span).ConfigureAwait(false);

            var old = info.Value;
            var temp = info.Value = info.NewValue();

            if (level < DEEP)
            {
                level++;
                var str = $"{id}.{level}";
                await Task.Run(() => Create_Sync_Async(info, str, level, timeout));

                if (info.Value != temp)
                    throw new Exception($"Value:{info.Value}, Old:{old}, Temp:{temp}");
            }

            await Task.Delay(WAIT).ConfigureAwait(false);
            info.Value = old;
            Locker.Print(Gray, $"Exiting   : {disp}");
        }
        catch (TimeoutException)
        {
            if (timeout >= 0)
            {
                info.TimeoutCaptured = true;
                Locker.Print(Red, $"** Timeout Exception: {info.Locker}, Id: {id}");
            }
            else throw;
        }
    }

    //[Enforced]
    [Fact]
    public static void Test_Sync_Async()
    {
        using var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Task.Run(() => Create_Sync_Async(info, str, 0, -1));
            tasks.Add(task);
        }
        Info.ThrowOrWaitAll(tasks.ToArray());
    }

    // ====================================================

    //[Enforced]
    [Fact]
    public static void Test_Async_Sync()
    {
        using var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Create_Async_Sync(info, str, 0, -1);
            tasks.Add(task);
        }
        Info.ThrowOrWaitAll(tasks.ToArray());
    }

    // ====================================================

    //[Enforced]
    [Fact]
    public static void Test_Mixing()
    {
        using var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            Task task;
            if ((i % 2) == 0) task = Task.Run(() => Create_Sync_Async(info, str, 0, -1));
            else
            {
                Thread.Sleep(200);
                task = Create_Async_Sync(info, str, 0, -1);
            }
            tasks.Add(task);
        }
        Info.ThrowOrWaitAll(tasks.ToArray());
    }

    // ====================================================

    //[Enforced]
    [Fact]
    public static void Test_Sync_Timeout()
    {
        using var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Task.Run(() => Create_Sync(info, str, 0, TIMEOUT));
            tasks.Add(task);
        }
        Info.ThrowOrWaitAll(tasks.ToArray());

        if (!info.TimeoutCaptured) throw new Exception(
            "Timeout exceptions expected but were not captured.");
    }

    // ====================================================

    //[Enforced]
    [Fact]
    public static void Test_Async_Timeout()
    {
        using var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Create_Async(info, str, 0, TIMEOUT);
            tasks.Add(task);
        }
        Info.ThrowOrWaitAll(tasks.ToArray());

        if (!info.TimeoutCaptured) throw new Exception(
            "Timeout exceptions expected but were not captured.");
    }

    // ====================================================

    //[Enforced]
    [Fact]
    public static void Test_Mixing_Timeout()
    {
        using var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            Task task;
            if ((i % 2) == 0) task = Task.Run(() => Create_Sync_Async(info, str, 0, TIMEOUT));
            else
            {
                Thread.Sleep(200);
                task = Create_Async_Sync(info, str, 0, TIMEOUT);
            }
            tasks.Add(task);
        }
        Info.ThrowOrWaitAll(tasks.ToArray());

        if (!info.TimeoutCaptured) throw new Exception(
            "Timeout exceptions expected but were not captured.");
    }
}