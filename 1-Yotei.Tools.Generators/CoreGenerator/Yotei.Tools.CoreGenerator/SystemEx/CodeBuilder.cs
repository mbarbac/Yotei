namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Maintains generated source code.
/// </summary>
internal class CodeBuilder
{
    readonly StringBuilder Builder = new();
    bool AtOrigin = true;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CodeBuilder() { }

    /// <summary>
    /// <inheritdoc/>
    /// <br/> This method can actually be used to obtain the builder contents to emit code.
    /// </summary>
    /// <returns><inheritdoc/></returns>
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

    } = 4;

    /// <summary>
    /// The actual size of this instance.
    /// </summary>
    public int Length => Builder.Length;

    /// <summary>
    /// Appends to this instance the given contents, with the current indentation level and size,
    /// taking into consideration the new line separators embedded in the given contents.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public void Append(string message, params object?[]? args)
    {
        message ??= string.Empty;
        args ??= [null];
        if (message.Length > 0 && args.Length > 0) message = string.Format(message, args);

        var count = message.Count(static x => x == '\n');
        if (count == 0)
        {
            if (AtOrigin) Builder.Append(Header());
            Builder.Append(message);
            AtOrigin = message.EndsWith('\n');
        }
        else
        {
            var parts = TokenizeNL(message, count);
            foreach (var part in parts)
            {
                if (AtOrigin) Builder.Append(Header());
                Builder.Append(part);
                AtOrigin = part.EndsWith('\n');
            }
        }
    }

    /// <summary>
    /// Appends to this instance a new line separator.
    /// </summary>
    public void AppendLine() => AppendLine(string.Empty);

    /// <summary>
    /// Appends to this instance the given contents, with the current indentation level and size,
    /// taking into consideration the new line separators embedded in the given contents. Then,
    /// appends a new line separator.
    /// </summary>
    /// <param name="message"></param>
    public void AppendLine(string message, params object?[]? args)
    {
        Append(message, args);
        Append("\n");
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tokenizes the given message in both the regular parts and the new line ones, keeping them
    /// both for further processing. If a capacity is givenm, then it is used to set the initial
    /// capacity of the internal data structures.
    /// </summary>
    static string[] TokenizeNL(string message, int capacity = 0)
    {
        List<string> list = capacity == 0 ? new() : new(capacity);
        var span = message.AsSpan();
        var wind = "\r\n";
        var unix = "\n";
        int index;
        var term = unix;

        while (span.Length > 0)
        {
            index = span.IndexOf(wind);
            if (index >= 0)
            {
                list.Add(span[..index].ToString());
                list.Add(term);

                span = span[(index + wind.Length)..];
                continue;
            }

            index = span.IndexOf(unix);
            if (index >= 0)
            {
                list.Add(span[..index].ToString());
                list.Add(term);

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
    /// Returns an appropriate header using the current debug size and level.
    /// </summary>
    string Header()
    {
        var num = IndentLevel * IndentSize;
        switch (num)
        {
            case 0: return string.Empty;
            case 1: return " ";
            case 2: return "  ";
            case 3: return "   ";
            case 4: return "    ";
            case 5: return "     ";
            case 6: return "      ";
            case 7: return "       ";
            case 8: return "        ";
        }

        if (!_Headers.TryGetValue(num, out var header))
        {
            header = new string(' ', num);
            _Headers[num] = header;
        }
        return header;
    }
    readonly static Dictionary<int, string> _Headers = [];
}