﻿#pragma warning disable IDE0079
#pragma warning disable CA2208

using static Yotei.Tools.Diagnostics.DebugEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_AsyncLock
{
#if DEBUG
    static readonly int NUM = 4;
    static readonly int DEEP = 4;
    static readonly int WAIT = 50;
#else
    static readonly int NUM = 10;
    static readonly int DEEP = 10;
    static readonly int WAIT = 100;
#endif
    static readonly int TIMEOUT = WAIT * (DEEP + NUM);

    /// <summary>
    /// Wait until all tasks are completed, or until one of them throws an exception.
    /// </summary>
    /// <param name="tasks"></param>
    static void ThrowOrWaitAll(Task[] tasks)
    {
        while (true)
        {
            var done = 0;
            foreach (var task in tasks)
            {
                if (task is null) throw new ArgumentNullException(nameof(task));
                if (task.IsCompleted) { done++; continue; }
                if (task.IsFaulted) throw task.Exception ?? new Exception("Unknown exception");
            }

            if (done == tasks.Length) break;
        }
    }

    // ----------------------------------------------------

    public class Info : IDisposable, IAsyncDisposable
    {
        public AsyncLock Locker = new();
        public bool Captured = false;
        public Random Random = new();

        /// <summary>
        /// Sentinel value.
        /// </summary>
        public int Value = 0;

        /// <summary>
        /// Generates a new sentinel value, without capturing it.
        /// </summary>
        /// <returns></returns>
        public int NewValue()
        {
            while (true)
            {
                var value = Random.Next(1, 99);
                if (value != Value) return value;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Locker.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await Locker.DisposeAsync().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }
    }

    // ----------------------------------------------------

    static void Create_Sync_Sync(Info info, string id, int level, int timeout)
    {
        WriteLine(true, Blue, $"Locking: {info.Locker}, Id: {id}");

        var span = TimeSpan.FromMilliseconds(timeout);
        try
        {
            using var disposable = info.Locker.Lock(span, id);

            WriteLine(true, Magenta, $"Locked: {info.Locker}, Id: {id}");

            var old = info.Value;
            var temp = info.Value = info.NewValue();

            if (level < DEEP)
            {
                level++;
                var str = $"{id}.{level}";
                Create_Sync_Sync(info, str, level, timeout);

                if (info.Value != temp)
                    throw new Exception($"Value:{info.Value}, Old:{old}, Temp:{temp}");
            }

            Thread.Sleep(WAIT);
            info.Value = old;

            WriteLine(Green, $"Unlocking: {disposable}");
        }
        catch (TimeoutException)
        {
            if (timeout >= 0)
            {
                info.Captured = true;
                WriteLine(true, Red, $"** Timeout exception: {info.Locker}, Id: {id}");
            }
            else throw;
        }
    }

    //[Enforced]
    [Fact]
    public static void Test_Sync_Sync()
    {
        using var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Task.Run(() => Create_Sync_Sync(info, str, 0, -1));
            tasks.Add(task);
        }
        ThrowOrWaitAll(tasks.ToArray());
    }

    //[Enforced]
    [Fact]
    public static void Test_Sync_Sync_Timeout()
    {
        using var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Task.Run(() => Create_Sync_Sync(info, str, 0, TIMEOUT));
            tasks.Add(task);
        }
        ThrowOrWaitAll(tasks.ToArray());

        if (!info.Captured)
        {
            WriteLine(true, Red, $"**** Timeout exceptions expected but not captured!");
            throw new InvalidOperationException();
        }
    }

    // ----------------------------------------------------

    static async Task Create_Async_Async(Info info, string id, int level, int timeout)
    {
        WriteLine(true, Blue, $"Locking: {info.Locker}, Id: {id}");

        var span = TimeSpan.FromMilliseconds(timeout);
        try
        {
            await using var disposable =
                await info.Locker.LockAsync(span, CancellationToken.None, id).ConfigureAwait(false);

            WriteLine(true, Magenta, $"Locked: {info.Locker}, Id: {id}");

            var old = info.Value;
            var temp = info.Value = info.NewValue();

            if (level < DEEP)
            {
                level++;
                var str = $"{id}.{level}";
                await Create_Async_Async(info, str, level, timeout).ConfigureAwait(false);

                if (info.Value != temp)
                    throw new Exception($"Value:{info.Value}, Old:{old}, Temp:{temp}");
            }

            await Task.Delay(WAIT).ConfigureAwait(false);
            info.Value = old;

            WriteLine(Green, $"Unlocking: {disposable}");
        }
        catch (TimeoutException)
        {
            if (timeout >= 0)
            {
                info.Captured = true;
                WriteLine(true, Red, $"** Timeout exception: {info.Locker}, Id: {id}");
            }
            else throw;
        }
    }

    //[Enforced]
    [Fact]
    public static void Test_Async_Async()
    {
        using var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Task.Run(() => Create_Async_Async(info, str, 0, -1));
            tasks.Add(task);
        }
        ThrowOrWaitAll(tasks.ToArray());
    }

    //[Enforced]
    [Fact]
    public static void Test_Async_Async_Timeout()
    {
        using var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Task.Run(() => Create_Async_Async(info, str, 0, TIMEOUT));
            tasks.Add(task);
        }
        ThrowOrWaitAll(tasks.ToArray());

        if (!info.Captured)
        {
            WriteLine(true, Red, $"**** Timeout exceptions expected but not captured!");
            throw new InvalidOperationException();
        }
    }

    // ----------------------------------------------------

    static void Create_Sync_Async(Info info, string id, int level, int timeout)
    {
        WriteLine(true, Blue, $"Locking: {info.Locker}, Id: {id}");

        var span = TimeSpan.FromMilliseconds(timeout);
        try
        {
            using var disposable = info.Locker.Lock(span, id);

            WriteLine(true, Magenta, $"Locked: {info.Locker}, Id: {id}");

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

            WriteLine(Green, $"Unlocking: {disposable}");
        }
        catch (TimeoutException)
        {
            if (timeout >= 0)
            {
                info.Captured = true;
                WriteLine(true, Red, $"** Timeout exception: {info.Locker}, Id: {id}");
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
        ThrowOrWaitAll(tasks.ToArray());
    }

    //[Enforced]
    [Fact]
    public static void Test_Sync_Async_Timeout()
    {
        using var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Task.Run(() => Create_Sync_Async(info, str, 0, TIMEOUT));
            tasks.Add(task);
        }
        ThrowOrWaitAll(tasks.ToArray());

        if (!info.Captured)
        {
            WriteLine(true, Red, $"**** Timeout exceptions expected but not captured!");
            throw new InvalidOperationException();
        }
    }

    // ----------------------------------------------------

    static async Task Create_Async_Sync(Info info, string id, int level, int timeout)
    {
        WriteLine(true, Blue, $"Locking: {info.Locker}, Id: {id}");

        var span = TimeSpan.FromMilliseconds(timeout);
        try
        {
            await using var disposable =
                await info.Locker.LockAsync(span, CancellationToken.None, id).ConfigureAwait(false);

            WriteLine(true, Magenta, $"Locked: {info.Locker}, Id: {id}");

            var old = info.Value;
            var temp = info.Value = info.NewValue();

            if (level < DEEP)
            {
                level++;
                var str = $"{id}.{level}";

                await Task.Run(()
                    => Create_Sync_Async(info, str, level, timeout)).ConfigureAwait(false);

                if (info.Value != temp)
                    throw new Exception($"Value:{info.Value}, Old:{old}, Temp:{temp}");
            }

            await Task.Delay(WAIT).ConfigureAwait(false);
            info.Value = old;

            WriteLine(Green, $"Unlocking: {disposable}");
        }
        catch (TimeoutException)
        {
            if (timeout >= 0)
            {
                info.Captured = true;
                WriteLine(true, Red, $"** Timeout exception: {info.Locker}, Id: {id}");
            }
            else throw;
        }
    }

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
        ThrowOrWaitAll(tasks.ToArray());
    }

    //[Enforced]
    [Fact]
    public static void Test_Async_Sync_Timeout()
    {
        using var info = new Info();
        var tasks = new List<Task>(); for (int i = 0; i < NUM; i++)
        {
            var str = $"{i + 1}";
            var task = Create_Async_Sync(info, str, 0, TIMEOUT);
            tasks.Add(task);
        }
        ThrowOrWaitAll(tasks.ToArray());

        if (!info.Captured)
        {
            WriteLine(true, Red, $"**** Timeout exceptions expected but not captured!");
            throw new InvalidOperationException();
        }
    }
}