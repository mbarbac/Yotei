namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a string builder for generated code.
/// </summary>
public class CodeBuilder
{
    readonly StringBuilder Builder = new();
    bool AtOrigin = true;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public CodeBuilder() { }

    /// <summary>
    /// Obtains the contents captured by this instance.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Builder.ToString();

    // ----------------------------------------------------

    /// <summary>
    /// The current length of the contents captured by this instance.
    /// </summary>
    public int Length => Builder.Length;

    /// <summary>
    /// The current indentation level.
    /// </summary>
    public int IndentLevel
    {
        get;
        set => field = value >= 0 ? value
            : throw new ArgumentException($"Invalid Indent Level: {value}");
    } = 0;

    /// <summary>
    /// The current indentation size.
    /// </summary>
    public int IndentSize
    {
        get;
        set => field = value >= 0 ? value
            : throw new ArgumentException($"Invalid Indent Size: {value}");
    } = 4;

    // ----------------------------------------------------

    /// <summary>
    /// Appends to this instance the the given message and optional arguments, using the current
    /// size and indentation level. This method takes into consideration any new line separators
    /// the contents may have embedded.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public void Append(string message, params object?[]? args)
    {
        message ??= string.Empty;
        args ??= [null];
        if (message.Length > 0 && args.Length > 0) message = string.Format(message, args);

        var numNL = message.Count(static x => x == '\n');
        if (numNL == 0)
        {
            if (AtOrigin) Builder.Append(GetIndentHeader(IndentLevel, IndentSize));
            Builder.Append(message);
            AtOrigin = message.EndsWith('\n');
        }
        else
        {
            var items = TokenizeNL(message, numNL);
            foreach (var item in items)
            {
                if (AtOrigin) Builder.Append(GetIndentHeader(IndentLevel, IndentSize));
                Builder.Append(item);
                AtOrigin = item.EndsWith('\n');
            }
        }
    }

    /// <summary>
    /// Appends to this instance a new line separator.
    /// </summary>
    public void AppendLine()
    {
        Builder.AppendLine();
        AtOrigin = true;
    }

    /// <summary>
    /// Appends to this instance the the given message and optional arguments, using the current
    /// size and indentation level, and then appends a new line separator. This method also takes
    /// into consideration any new line separators the contents may have embedded.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="message"></param>
    public void AppendLine(string message, params object?[]? args)
    {
        Append(message, args);
        AppendLine();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tokenizes the given message into both its regular parts and the new line ones, keeping
    /// them both for further processing. Takes a <paramref name="numNL"/> argument that is a
    /// hint about how many parts will be returned.
    /// </summary>
    static string[] TokenizeNL(string message, int numNL)
    {
        var list = new List<string>((numNL * 2) + 1);
        var span = message.AsSpan();
        var wind = "\r\n";
        var unix = "\n";

        int index;

        while (span.Length > 0)
        {
            // Trying first windows as it has two characters...
            index = span.IndexOf(wind);
            if (index >= 0)
            {
                list.Add(span[..index].ToString());
                list.Add(wind);

                span = span[(index + wind.Length)..];
                continue;
            }

            // Otherwise, trying the unix-like variant...
            index = span.IndexOf(unix);
            if (index >= 0)
            {
                list.Add(span[..index].ToString());
                list.Add(unix);

                span = span[(index + unix.Length)..];
                continue;
            }

            // No line terminator...
            list.Add(span.ToString());
            break;
        }

        return [.. list];
    }

    // ----------------------------------------------------

    readonly static Dictionary<int, string> _Headers = [];
    static CodeBuilder() => PopulateIndentHeaders();
    static void PopulateIndentHeaders()
    {
        _Headers[0] = string.Empty;
        _Headers[1] = new(' ', 1);
        _Headers[2] = new(' ', 2);
        _Headers[3] = new(' ', 3);
        _Headers[4] = new(' ', 4);
        _Headers[6] = new(' ', 6);
        _Headers[8] = new(' ', 8);
        _Headers[9] = new(' ', 9);
        _Headers[12] = new(' ', 12);
        _Headers[16] = new(' ', 16);
    }

    /// <summary>
    /// Obtains a spaces-only string that can be used as a line header for the given
    /// indent level and size.
    /// </summary>
    /// <param name="indentLevel"></param>
    /// <param name="indentSize"></param>
    /// <returns></returns>
    internal static string GetIndentHeader(int indentLevel, int indentSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(indentLevel, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(indentSize, 0);

        var size = indentLevel * indentSize;

        if (!_Headers.TryGetValue(size, out var header))
            _Headers[size] = header = new string(' ', size);

        return header;
    }
}