namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a string builder for generated code.
/// </summary>
internal class CodeBuilder
{
    readonly StringBuilder Builder = new();
    bool AtOrigin = true;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public CodeBuilder() { }

    /// <summary>
    /// Returns a string with the contents of this builder.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Builder.ToString();

    /// <summary>
    /// The current indentation level in this instance.
    /// </summary>
    public int IndentLevel
    {
        get;
        set => field = value >= 0 ? value
            : throw new ArgumentException($"Invalid Indent Level: {value}");

    } = 0;

    /// <summary>
    /// The current indentation size used by this instance.
    /// </summary>
    public int IndentSize
    {
        get;
        set => field = value >= 0 ? value
            : throw new ArgumentException($"Invalid Indent Size: {value}");

    } = 2;

    /// <summary>
    /// The actual size of this instance.
    /// </summary>
    public int Length => Builder.Length;

    // ----------------------------------------------------

    /// <summary>
    /// Appends to this instance the given contents, using the current indentation level and size,
    /// and taking into consideration the new line separators embedded in the given contents.
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
            if (AtOrigin) Builder.Append(Header());
            Builder.Append(message);
            AtOrigin = message.EndsWith('\n');
        }
        else
        {
            var items = TokenizeNL(message, numNL);
            foreach (var item in items)
            {
                if (AtOrigin) Builder.Append(Header());
                Builder.Append(item);
                AtOrigin = item.EndsWith('\n');
            }
        }
    }

    /// <summary>
    /// Appends to this instance a new line separator.
    /// </summary>
    public void AppendLine() => AppendLine(string.Empty);

    /// <summary>
    /// Appends to this instance the given contents, using the current indentation level and size,
    /// and taking into consideration the new line separators embedded in the given contents. Then,
    /// appends a new line separator.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public void AppendLine(string message, params object?[]? args)
    {
        Append(message, args);
        Append("\n");
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tokenizes the given message in both the regular parts and the new line ones, keeping them
    /// both for further processing.
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
            index = span.IndexOf(wind);
            if (index >= 0)
            {
                list.Add(span[..index].ToString());
                list.Add(wind);

                span = span[(index + wind.Length)..];
                continue;
            }

            index = span.IndexOf(unix);
            if (index >= 0)
            {
                list.Add(span[..index].ToString());
                list.Add(unix);

                span = span[(index + unix.Length)..];
                continue;
            }

            list.Add(span.ToString());
            break;
        }

        return [.. list];
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns an appropriate header using the current indent size and level.
    /// </summary>
    string Header()
    {
        var num = IndentLevel * IndentSize;

        if (!_Headers.TryGetValue(num, out var header))
        {
            header = new string(' ', num);
            _Headers[num] = header;
        }
        return header;
    }
    readonly static Dictionary<int, string> _Headers = [];
}