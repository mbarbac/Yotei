namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a <see cref="Console"/>-alike class that provides some extended capabilities, but
/// without replacing it.
/// </summary>
public static class ConsoleEx
{
    /// <summary>
    /// <inheritdoc cref="Console.ForegroundColor"/>
    /// </summary>
    public static ConsoleColor ForegroundColor
    {
        get => Console.ForegroundColor;
        set => Console.ForegroundColor = value;
    }

    /// <summary>
    /// <inheritdoc cref="Console.BackgroundColor"/>
    /// </summary>
    public static ConsoleColor BackgroundColor
    {
        get => Console.BackgroundColor;
        set => Console.BackgroundColor = value;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="Console.Clear"/>
    /// </summary>
    public static void Clear() => Console.Clear();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="Console.Write(string, ReadOnlySpan{object?})"/>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        string message, params object?[]? args) => Write(false, message, args);

    /// <summary>
    /// <inheritdoc cref="Console.Write(string, ReadOnlySpan{object?})"/>
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        ConsoleColor forecolor, string message, params object?[]? args)
    {
        var oldfore = ForegroundColor; ForegroundColor = forecolor;
        Write(message, args);
        ForegroundColor = oldfore;
    }

    /// <summary>
    /// <inheritdoc cref="Console.Write(string, ReadOnlySpan{object?})"/>
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        ConsoleColor forecolor, ConsoleColor backcolor, string message, params object?[]? args)
    {
        var oldback = BackgroundColor; BackgroundColor = backcolor;
        Write(forecolor, message, args);
        BackgroundColor = oldback;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="Console.Write(string, ReadOnlySpan{object?})"/> If requested, the
    /// displayed formatted message is replicated in the not-console listerners of the DEBUG
    /// environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(bool debug, string message, params object?[]? args)
    {
        message ??= string.Empty;
        args ??= [null];
        if (args.Length > 0) message = string.Format(message, args);

        Console.Write(message);
        if (debug) WithNoListeners(() => Debug.Write(message));
    }

    /// <summary>
    /// <inheritdoc cref="Console.Write(string, ReadOnlySpan{object?})"/> If requested, the
    /// displayed formatted message is replicated in the not-console listerners of the DEBUG
    /// environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        bool debug, ConsoleColor forecolor, string message, params object?[]? args)
    {
        var oldfore = ForegroundColor; ForegroundColor = forecolor;
        Write(debug, message, args);
        ForegroundColor = oldfore;
    }

    /// <summary>
    /// <inheritdoc cref="Console.Write(string, ReadOnlySpan{object?})"/> If requested, the
    /// displayed formatted message is replicated in the not-console listerners of the DEBUG
    /// environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Write(
        bool debug, ConsoleColor forecolor, ConsoleColor backcolor,
        string message, params object?[]? args)
    {
        var oldback = BackgroundColor; BackgroundColor = backcolor;
        Write(debug, forecolor, message, args);
        BackgroundColor = oldback;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="Console.WriteLine(string, ReadOnlySpan{object?})"/>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        string message, params object?[]? args) => WriteLine(false, message, args);

    /// <summary>
    /// <inheritdoc cref="Console.WriteLine(string, ReadOnlySpan{object?})"/>
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        ConsoleColor forecolor, string message, params object?[]? args)
    {
        var oldfore = ForegroundColor; ForegroundColor = forecolor;
        WriteLine(message, args);
        ForegroundColor = oldfore;
    }

    /// <summary>
    /// <inheritdoc cref="Console.WriteLine(string, ReadOnlySpan{object?})"/>
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        ConsoleColor forecolor, ConsoleColor backcolor, string message, params object?[]? args)
    {
        var oldback = BackgroundColor; BackgroundColor = backcolor;
        WriteLine(forecolor, message, args);
        BackgroundColor = oldback;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="Console.WriteLine(string, ReadOnlySpan{object?})"/> If requested, the
    /// displayed formatted message is replicated in the not-console listerners of the DEBUG
    /// environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(bool debug, string message, params object?[]? args)
    {
        message ??= string.Empty;
        args ??= [null];
        if (args.Length > 0) message = string.Format(message, args);

        Console.WriteLine(message);
        if (debug) WithNoListeners(() => Debug.WriteLine(message));
    }

    /// <summary>
    /// <inheritdoc cref="Console.WriteLine(string, ReadOnlySpan{object?})"/> If requested, the
    /// displayed formatted message is replicated in the not-console listerners of the DEBUG
    /// environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        bool debug, ConsoleColor forecolor, string message, params object?[]? args)
    {
        var oldfore = ForegroundColor; ForegroundColor = forecolor;
        WriteLine(debug, message, args);
        ForegroundColor = oldfore;
    }

    /// <summary>
    /// <inheritdoc cref="Console.WriteLine(string, ReadOnlySpan{object?})"/> If requested, the
    /// displayed formatted message is replicated in the not-console listerners of the DEBUG
    /// environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void WriteLine(
        bool debug, ConsoleColor forecolor, ConsoleColor backcolor,
        string message, params object?[]? args)
    {
        var oldback = BackgroundColor; BackgroundColor = backcolor;
        WriteLine(debug, forecolor, message, args);
        BackgroundColor = oldback;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="Console.ReadLine"/>
    /// </summary>
    /// <returns></returns>
    public static string? ReadLine() => Console.ReadLine();

    /// <summary>
    /// <inheritdoc cref="Console.ReadLine"/>
    /// </summary>
    /// <param name="forecolor"></param>
    /// <returns></returns>
    public static string? ReadLine(ConsoleColor forecolor)
    {
        var oldfore = ForegroundColor; ForegroundColor = forecolor;
        var r = ReadLine();
        ForegroundColor = oldfore;
        return r;
    }

    /// <summary>
    /// <inheritdoc cref="Console.ReadLine"/>
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <returns></returns>
    public static string? ReadLine(ConsoleColor forecolor, ConsoleColor backcolor)
    {
        var oldback = BackgroundColor; BackgroundColor = backcolor;
        var r = ReadLine(forecolor);
        BackgroundColor = oldback;
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="Console.ReadLine"/> If requested, the read string, if any, is also
    /// replicated in the not-console listerners of the DEBUG environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <returns></returns>
    public static string? ReadLine(bool debug)
    {
        var r = ReadLine();
        if (r is not null && debug) WithNoListeners(() => Debug.WriteLine(r));
        return r;
    }

    /// <summary>
    /// <inheritdoc cref="Console.ReadLine"/> If requested, the read string, if any, is also
    /// replicated in the not-console listerners of the DEBUG environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <returns></returns>
    public static string? ReadLine(bool debug, ConsoleColor forecolor)
    {
        var oldfore = ForegroundColor; ForegroundColor = forecolor;
        var r = ReadLine(debug);
        ForegroundColor = oldfore;
        return r;
    }

    /// <summary>
    /// <inheritdoc cref="Console.ReadLine"/> If requested, the read string, if any, is also
    /// replicated in the not-console listerners of the DEBUG environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <returns></returns>
    public static string? ReadLine(bool debug, ConsoleColor forecolor, ConsoleColor backcolor)
    {
        var oldback = BackgroundColor; BackgroundColor = backcolor;
        var r = ReadLine(debug, forecolor);
        BackgroundColor = oldback;
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to obtain a string representation of the given key.
    /// </summary>
    static string KeyToString(
        this ConsoleKeyInfo info) => info.KeyChar < 32 ? $"[{info.Key}]" : $"{info.KeyChar}";

    /// <summary>
    /// <inheritdoc cref="Console.ReadKey"/>
    /// </summary>
    /// <returns></returns>
    public static ConsoleKeyInfo ReadKey()
    {
        var r = Console.ReadKey(intercept: true);
        Console.Write(r.KeyToString());
        return r;
    }

    /// <summary>
    /// <inheritdoc cref="Console.ReadKey"/>
    /// </summary>
    /// <param name="forecolor"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo ReadKey(ConsoleColor forecolor)
    {
        var oldfore = ForegroundColor; ForegroundColor = forecolor;
        var r = ReadKey();
        ForegroundColor = oldfore;
        return r;
    }

    /// <summary>
    /// <inheritdoc cref="Console.ReadKey"/>
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo ReadKey(ConsoleColor forecolor, ConsoleColor backcolor)
    {
        var oldback = BackgroundColor; BackgroundColor = backcolor;
        var r = ReadKey(forecolor);
        BackgroundColor = oldback;
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="Console.ReadKey(bool)"/>
    /// Use 'intercept=true' to prevent displaying the key.
    /// </summary>
    /// <param name="intercept"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo ReadKey(bool intercept)
    {
        var r = Console.ReadKey(intercept: true);
        if (!intercept) Console.Write(r.KeyToString());
        return r;
    }

    /// <summary>
    /// <inheritdoc cref="Console.ReadKey(bool)"/>
    /// Use 'intercept=true' to prevent displaying the key.
    /// </summary>
    /// <param name="intercept"></param>
    /// <param name="forecolor"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo ReadKey(bool intercept, ConsoleColor forecolor)
    {
        var oldfore = ForegroundColor; ForegroundColor = forecolor;
        var r = ReadKey(intercept);
        ForegroundColor = oldfore;
        return r;
    }

    /// <summary>
    /// <inheritdoc cref="Console.ReadKey(bool)"/>
    /// Use 'intercept=true' to prevent displaying the key.
    /// </summary>
    /// <param name="intercept"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo ReadKey(
        bool intercept, ConsoleColor forecolor, ConsoleColor backcolor)
    {
        var oldback = BackgroundColor; BackgroundColor = backcolor;
        var r = ReadKey(intercept, forecolor);
        BackgroundColor = oldback;
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="Console.ReadKey"/>
    /// Returns '<c>null</c>' if the timeout has expired with no pressed key.
    /// <para>
    /// Because the first parameter in the corresponding method of the <see cref="Console"/> class
    /// is a boolean one ('intercept'), then we will not use a 'debug' one. If you want to write
    /// the result into the DEBUG environment, then you must do that explicitly.
    /// </para>
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(TimeSpan timeout) => ReadKey(false, timeout);

    /// <summary>
    /// <inheritdoc cref="Console.ReadKey"/>
    /// Returns '<c>null</c>' if the timeout has expired with no pressed key.
    /// <para>
    /// Because the first parameter in the corresponding method of the <see cref="Console"/> class
    /// is a boolean one ('intercept'), then we will not use a 'debug' one. If you want to write
    /// the result into the DEBUG environment, then you must do that explicitly.
    /// </para>
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="forecolor"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(TimeSpan timeout, ConsoleColor forecolor)
    {
        var oldfore = ForegroundColor; ForegroundColor = forecolor;
        var r = ReadKey(timeout);
        ForegroundColor = oldfore;
        return r;
    }

    /// <summary>
    /// <inheritdoc cref="Console.ReadKey"/>
    /// Returns '<c>null</c>' if the timeout has expired with no pressed key.
    /// <para>
    /// Because the first parameter in the corresponding method of the <see cref="Console"/> class
    /// is a boolean one ('intercept'), then we will not use a 'debug' one. If you want to write
    /// the result into the DEBUG environment, then you must do that explicitly.
    /// </para>
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(
        TimeSpan timeout, ConsoleColor forecolor, ConsoleColor backcolor)
    {
        var oldback = BackgroundColor; BackgroundColor = backcolor;
        var r = ReadKey(timeout, forecolor);
        BackgroundColor = oldback;
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="Console.ReadKey(bool)"/> Use 'intercept=true' to prevent displaying the
    /// key. Returns '<c>null</c>' if the timeout has expired with no pressed key.
    /// <para>
    /// Because the first parameter in the corresponding method of the <see cref="Console"/> class
    /// is a boolean one ('intercept'), then we will not use a 'debug' one. If you want to write
    /// the result into the DEBUG environment, then you must do that explicitly.
    /// </para>
    /// </summary>
    /// <param name="intercept"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(bool intercept, TimeSpan timeout)
    {
        var ms = timeout.ValidatedTimeout;
        var ini = DateTime.UtcNow;

        while (true)
        {
            // Trying an available key...
            if (Console.KeyAvailable)
            {
                var info = Console.ReadKey(intercept: true);

                if (!intercept) Console.Write(info.KeyToString());
                return info;
            }

            // Waiting...
            if (ms > -1)
            {
                var now = DateTime.UtcNow;
                var span = now - ini;
                if (span >= timeout) return null;
            }
            Thread.Yield();
        }
    }

    /// <summary>
    /// <inheritdoc cref="Console.ReadKey(bool)"/> Use 'intercept=true' to prevent displaying the
    /// key. Returns '<c>null</c>' if the timeout has expired with no pressed key.
    /// <para>
    /// Because the first parameter in the corresponding method of the <see cref="Console"/> class
    /// is a boolean one ('intercept'), then we will not use a 'debug' one. If you want to write
    /// the result into the DEBUG environment, then you must do that explicitly.
    /// </para>
    /// </summary>
    /// <param name="intercept"></param>
    /// <param name="timeout"></param>
    /// <param name="forecolor"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(bool intercept, TimeSpan timeout, ConsoleColor forecolor)
    {
        var oldfore = ForegroundColor; ForegroundColor = forecolor;
        var r = ReadKey(intercept,timeout);
        ForegroundColor = oldfore;
        return r;
    }

    /// <summary>
    /// <inheritdoc cref="Console.ReadKey(bool)"/> Use 'intercept=true' to prevent displaying the
    /// key. Returns '<c>null</c>' if the timeout has expired with no pressed key.
    /// <para>
    /// Because the first parameter in the corresponding method of the <see cref="Console"/> class
    /// is a boolean one ('intercept'), then we will not use a 'debug' one. If you want to write
    /// the result into the DEBUG environment, then you must do that explicitly.
    /// </para>
    /// </summary>
    /// <param name="intercept"></param>
    /// <param name="timeout"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <returns></returns>
    public static ConsoleKeyInfo? ReadKey(
        bool intercept, TimeSpan timeout, ConsoleColor forecolor, ConsoleColor backcolor)
    {
        var oldback = BackgroundColor; BackgroundColor = backcolor;
        var r = ReadKey(intercept, timeout, forecolor);
        BackgroundColor = oldback;
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string (or an empty one it it was '<c>null</c>'),
    /// and returns the result of that edition. Returns '<c>null</c>' if it was cancelled by the
    /// user pressing [Escape].
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine(
        string? source = null)
        => EditLine(false, Timeout.InfiniteTimeSpan, ForegroundColor, BackgroundColor, source);

    /// <summary>
    /// Edits in the console the given source string (or an empty one it it was '<c>null</c>'),
    /// and returns the result of that edition. Returns '<c>null</c>' if it was cancelled by the
    /// user pressing [Escape].
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine(
        ConsoleColor forecolor, string? source = null)
        => EditLine(false, Timeout.InfiniteTimeSpan, forecolor, BackgroundColor, source);

    /// <summary>
    /// Edits in the console the given source string (or an empty one it it was '<c>null</c>'),
    /// and returns the result of that edition. Returns '<c>null</c>' if it was cancelled by the
    /// user pressing [Escape].
    /// </summary>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine(
        ConsoleColor forecolor, ConsoleColor backcolor, string? source = null)
        => EditLine(false, Timeout.InfiniteTimeSpan, forecolor, backcolor, source);

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string (or an empty one it it was '<c>null</c>'),
    /// and returns the result of that edition. Returns '<c>null</c>' if it was cancelled by the
    /// user pressing [Escape]. If requested, the result is replicated in the not-console
    /// listerners of the DEBUG environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine(
        bool debug, string? source = null)
        => EditLine(debug, Timeout.InfiniteTimeSpan, ForegroundColor, BackgroundColor, source);

    /// <summary>
    /// Edits in the console the given source string (or an empty one it it was '<c>null</c>'),
    /// and returns the result of that edition. Returns '<c>null</c>' if it was cancelled by the
    /// user pressing [Escape]. If requested, the result is replicated in the not-console
    /// listerners of the DEBUG environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine(
        bool debug, ConsoleColor forecolor, string? source = null)
        => EditLine(debug, Timeout.InfiniteTimeSpan, forecolor, BackgroundColor, source);

    /// <summary>
    /// Edits in the console the given source string (or an empty one it it was '<c>null</c>'),
    /// and returns the result of that edition. Returns '<c>null</c>' if it was cancelled by the
    /// user pressing [Escape]. If requested, the result is replicated in the not-console
    /// listerners of the DEBUG environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine(
        bool debug, ConsoleColor forecolor, ConsoleColor backcolor, string? source = null)
        => EditLine(debug, Timeout.InfiniteTimeSpan, forecolor, backcolor, source);

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string (or an empty one it it was '<c>null</c>'),
    /// and returns the result of that edition. Returns '<c>null</c>' if it was cancelled by the
    /// user pressing [Escape], or if the given timeout has expired.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine(
        TimeSpan timeout, string? source = null)
        => EditLine(false, timeout, ForegroundColor, BackgroundColor, source);

    /// <summary>
    /// Edits in the console the given source string (or an empty one it it was '<c>null</c>'),
    /// and returns the result of that edition. Returns '<c>null</c>' if it was cancelled by the
    /// user pressing [Escape], or if the given timeout has expired.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="forecolor"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine(
        TimeSpan timeout, ConsoleColor forecolor, string? source = null)
        => EditLine(false, timeout, forecolor, BackgroundColor, source);

    /// <summary>
    /// Edits in the console the given source string (or an empty one it it was '<c>null</c>'),
    /// and returns the result of that edition. Returns '<c>null</c>' if it was cancelled by the
    /// user pressing [Escape], or if the given timeout has expired.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine(
        TimeSpan timeout, ConsoleColor forecolor, ConsoleColor backcolor, string? source = null)
        => EditLine(false, timeout, forecolor, backcolor, source);

    // ----------------------------------------------------

    /// <summary>
    /// Edits in the console the given source string (or an empty one it it was '<c>null</c>'),
    /// and returns the result of that edition. Returns '<c>null</c>' if it was cancelled by the
    /// user pressing [Escape], or if the given timeout has expired. If requested, the result is
    /// replicated in the not-console listerners of the DEBUG environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="timeout"></param>
    /// <param name="forecolor"></param>
    /// <param name="backcolor"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine(
        bool debug,
        TimeSpan timeout,
        ConsoleColor forecolor, ConsoleColor backcolor,
        string? source = null)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Executes the given action in a debug/trace environment with no console listeners, and
    /// under an <see cref="Ambient"/> lock. Once the action is executed, any previous console
    /// listener is restored.
    /// </summary>
    /// <param name="action"></param>
    public static void WithNoListeners(Action action)
    {
        action.ThrowWhenNull();

        lock (Ambient.Lock)
        {
            var items = Ambient.GetConsoleListeners().ToArray();
            Ambient.RemoveListeners(items);

            Debug.Flush();
            try { action(); }
            finally { Ambient.AddListeners(items); }
            Debug.Flush();
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a string header with the requested number of spaces.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static string Spaces(int size)
    {
        if (size < 0) throw new ArgumentException($"Size cannot be negative: {size}.");

        switch (size)
        {
            case 0: return Header0;
            case 1: return Header1;
            case 2: return Header2;
            case 3: return Header3;
            case 4: return Header4;
            case 5: return Header5;
            case 6: return Header6;
            case 7: return Header7;
            case 8: return Header8;
            case 9: return Header9;
        }

        if (!Headers.TryGetValue(size, out var header))
        {
            Headers.Add(size, header = new(Space, size));
        }
        return header;
    }

    readonly static Dictionary<int, string> Headers = [];
    readonly static char Space = ' ';

    readonly static string Header0 = string.Empty;
    readonly static string Header1 = new(Space, 1);
    readonly static string Header2 = new(Space, 2);
    readonly static string Header3 = new(Space, 3);
    readonly static string Header4 = new(Space, 4);
    readonly static string Header5 = new(Space, 5);
    readonly static string Header6 = new(Space, 6);
    readonly static string Header7 = new(Space, 7);
    readonly static string Header8 = new(Space, 8);
    readonly static string Header9 = new(Space, 9);
}