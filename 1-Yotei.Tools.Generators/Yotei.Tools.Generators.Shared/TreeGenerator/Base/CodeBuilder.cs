namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a container for emitted source code.
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
    /// The current indentation level on this instance.
    /// </summary>
    public int IndentLevel
    {
        get => _IndentLevel;
        set => _IndentLevel = value >= 0
            ? value
            : throw new ArgumentException(nameof(value), $"Invalid indent level: {value}");
    }
    int _IndentLevel = 0;

    /// <summary>
    /// The indentation size (in spaces) for each level.
    /// </summary>
    public int IndentSize
    {
        get => _IndentSize;
        set => _IndentSize = value >= 1
            ? value
            : throw new ArgumentException(nameof(value), $"Invalid indent size: {value}");
    }
    int _IndentSize = 4;

    /// <summary>
    /// Appends the given contents to this instance, using the current indentation level, and any
    /// new line separators embedded in the given message.
    /// </summary>
    /// <param name="message"></param>
    public void Append(string message)
    {
        if (message == null) return;
        if (message.Length == 0) return;

        var count = message.Count(x => x == '\n');
        if (count == 0)
        {
            if (AtOrigin) Builder.Append(Header());
            Builder.Append(message);

            AtOrigin = message.EndsWith("\n");
        }
        else
        {
            var parts = TokenizeNL(message, count);
            foreach (var part in parts)
            {
                if (AtOrigin) Builder.Append(Header());
                Builder.Append(part);

                AtOrigin = part.EndsWith("\n");
            }
        }
    }

    /// <summary>
    /// Appends a new line to this instance.
    /// </summary>
    public void AppendLine() => AppendLine(string.Empty);

    /// <summary>
    /// Appends the given contents to this instance, using the current indentation level, and any
    /// new line separators embedded in the given message, and then adds a new line.
    /// </summary>
    /// <param name="message"></param>
    public void AppendLine(string message)
    {
        Append(message);
        Append("\n");
    }

    /// <summary>
    /// Returns the code that has been emitted into this instance.
    /// </summary>
    /// <returns></returns>
    public string GetCode() => Builder.ToString();

    // ----------------------------------------------------

    /// <summary>
    /// Tokenizes the given message into both regular parts and new line ones, keeping them
    /// both for further processing.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="NLcount"></param>
    /// <returns></returns>
    static string[] TokenizeNL(string message, int NLcount)
    {
        var list = new List<string>((NLcount * 2) + 1);
        var span = message.AsSpan();
        var wind = "\r\n";
        var unix = "\n";
        var term = unix;

        while (span.Length > 0)
        {
            var index = span.IndexOf(wind.AsSpan());
            if (index >= 0)
            {
                list.Add(span.Slice(0, index).ToString());
                list.Add(term);

                span = span.Slice(index + wind.Length);
                continue;
            }

            index = span.IndexOf(unix.AsSpan());
            if (index >= 0)
            {
                list.Add(span.Slice(0, index).ToString());
                list.Add(term);

                span = span.Slice(index + unix.Length);
                continue;
            }

            list.Add(span.ToString());
            break;
        }

        return list.ToArray();
    }

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
    readonly static Dictionary<int, string> _Headers = new();
}