namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a '<see cref="Console"/>'-alike class that provides some extended capabilities.
/// This class is not intended to fully replace the standard <see cref="Console"/> one.
/// </summary>
public static class ConsoleEx
{
    /// <summary>
    /// <inheritdoc cref="Console.CursorTop"/>
    /// This property is not available in some OS platforms.
    /// </summary>
    public static int CursorTop
    {
        get => Console.CursorTop;
        set => Console.CursorTop = value;
    }

    /// <summary>
    /// <inheritdoc cref="Console.CursorLeft"/>
    /// This property is not available in some OS platforms.
    /// </summary>
    public static int CursorLeft
    {
        get => Console.CursorLeft;
        set => Console.CursorLeft = value;
    }

    /// <summary>
    /// <inheritdoc cref="Console.CursorSize"/>
    /// This property is not available in some OS platforms.
    /// </summary>  
    public static int CursorSize
    {
        get => Console.CursorSize;

        [SuppressMessage("", "CA1416")]
        set => Console.CursorSize = value;
    }

    /// <summary>
    /// <inheritdoc cref="Console.KeyAvailable"/>
    /// </summary>
    public static bool KeyAvailable => Console.KeyAvailable;

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
    /// <inheritdoc cref="Console.WriteLine()"/>
    /// </summary>
    public static void WriteLine() => WriteLine(string.Empty);

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
    /// <inheritdoc cref="Console.WriteLine()"/> If requested, the new line is replicated in the
    /// not-console listerners of the DEBUG environment.
    /// </summary>
    /// <param name="debug"></param>
    public static void WriteLine(bool debug) => WriteLine(debug, string.Empty);

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
    public static ConsoleKeyInfo? ReadKey(
        bool intercept, TimeSpan timeout, ConsoleColor forecolor)
    {
        var oldfore = ForegroundColor; ForegroundColor = forecolor;
        var r = ReadKey(intercept, timeout);
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
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine(
        bool debug,
        TimeSpan timeout,
        string? source = null)
        => EditLine(debug, timeout, ForegroundColor, BackgroundColor, source);

    /// <summary>
    /// Edits in the console the given source string (or an empty one it it was '<c>null</c>'),
    /// and returns the result of that edition. Returns '<c>null</c>' if it was cancelled by the
    /// user pressing [Escape], or if the given timeout has expired. If requested, the result is
    /// replicated in the not-console listerners of the DEBUG environment.
    /// </summary>
    /// <param name="debug"></param>
    /// <param name="timeout"></param>
    /// <param name="forecolor"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? EditLine(
        bool debug,
        TimeSpan timeout,
        ConsoleColor forecolor,
        string? source = null)
        => EditLine(debug, timeout, forecolor, BackgroundColor, source);

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
        var oldfore = ForegroundColor; ForegroundColor = forecolor;
        var oldback = BackgroundColor; BackgroundColor = backcolor;
        var size = CursorSize;
        var left = CursorLeft;
        bool insert;

        var sb = new StringBuilder(); sb.Append(source ?? string.Empty);
        var pos = sb.Length;
        int len;

        SetCursorSize(insert = true);
        ShowLine(pos);

        while (true)
        {
            var info = ReadKey(intercept: true, timeout);
            info ??= new('\0', ConsoleKey.Escape, false, false, false);

            /// <summary>
            /// Special keys.
            /// </summary>
            switch (info.Value.Key)
            {
                case ConsoleKey.Enter:
                    SetCursorSize(true);
                    WriteLine();
                    ForegroundColor = oldfore;
                    BackgroundColor = oldback;
                    if (debug) WithNoListeners(() => Debug.WriteLine(sb));
                    return sb.ToString();

                case ConsoleKey.Escape:
                    SetCursorSize(true);
                    len = sb.Length; sb.Clear(); ShowLine(0, len);
                    WriteLine();
                    ForegroundColor = oldfore;
                    BackgroundColor = oldback;
                    return null;

                case ConsoleKey.Insert:
                    SetCursorSize(insert = !insert);
                    break;

                case ConsoleKey.Home:
                    pos = 0;
                    CursorLeft = left + pos;
                    break;

                case ConsoleKey.End:
                    pos = sb.Length;
                    CursorLeft = left + pos;
                    break;

                case ConsoleKey.Delete:
                    if (pos >= sb.Length) break;
                    len = sb.Length;
                    sb.Remove(pos, 1);
                    ShowLine(pos, len);
                    break;

                case ConsoleKey.Backspace:
                    if (pos == 0) break;
                    len = sb.Length;
                    sb.Remove(--pos, 1);
                    ShowLine(pos, len);
                    break;

                case ConsoleKey.LeftArrow:
                    if (info.Value.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        if (pos == 0) break;
                        var ascii = char.IsLetterOrDigit(sb[pos - 1]);
                        while (pos > 0)
                        {
                            var temp = char.IsLetterOrDigit(sb[pos - 1]);
                            if (temp == ascii) { pos--; CursorLeft--; }
                            else break;
                        }
                    }
                    else
                    {
                        if (pos > 0) { pos--; CursorLeft--; }
                    }
                    break;

                case ConsoleKey.RightArrow:
                    if (info.Value.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        if (pos >= sb.Length) break;
                        var ascii = char.IsLetterOrDigit(sb[pos]);
                        while (pos < sb.Length)
                        {
                            var temp = char.IsLetterOrDigit(sb[pos]);
                            if (temp == ascii) { pos++; CursorLeft++; }
                            else break;
                        }
                    }
                    else
                    {
                        if (pos < sb.Length) { pos++; CursorLeft++; }
                    }
                    break;
            }

            /// <summary>
            /// Standard keys.
            /// </summary>
            if (info.Value.KeyChar >= ' ')
            {
                if (insert)
                {
                    sb.Insert(pos, info.Value.KeyChar);
                    ShowLine(++pos);
                    continue;
                }
                if (pos < sb.Length)
                {
                    sb[pos] = info.Value.KeyChar;
                    ShowLine(++pos);
                }
                else
                {
                    sb.Append(info.Value.KeyChar);
                    Console.Write(info.Value.KeyChar); // Using 'Console' because char...
                    pos++;
                }
            }
        }

        /// <summary>
        /// Sets the cursor size for the insert mode ON (true) or OFF (false).
        /// </summary>
        void SetCursorSize(bool insert)
        {
            var windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (windows) CursorSize = insert ? size : 100;
        }

        /// <summary>
        /// Shows the current value clearing up to the remaining len, and sets the cursor
        /// position to the given one.
        /// </summary>
        void ShowLine(int pos, int len = 0)
        {
            CursorLeft = left;
            Write(sb.ToString());

            ForegroundColor = oldfore;
            BackgroundColor = oldback;
            len -= sb.Length;
            if (len > 0) Write(Spaces(len));
            CursorLeft = left + pos;

            ForegroundColor = forecolor;
            BackgroundColor = backcolor;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Ensures that the remaining console buffer capacity is enough to support the requested
    /// number of lines. If not, the buffer if cleared and the 'top' value reset.
    /// </summary>
    public static void EnsureTopInRange(int lines, ref int top)
    {
        var max = Console.BufferHeight - lines - 1;
        if (top >= max)
        {
            Clear();
            WriteLine(ConsoleColor.Red, "Console buffer exhausted and cleared.");
            WriteLine();
            top = CursorTop;
        }
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