namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a <see cref="StringBuilder"/>-alike class used to emit source code.
/// </summary>
internal class CodeBuilder
{
    readonly StringBuilder Builder = new();

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public CodeBuilder() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Builder.ToString();

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
    /// Determines if the current print position is at the start of a new line, or not.
    /// </summary>
    bool Origin { get; set; } = true;

    /// <summary>
    /// Appends the given contents to this instance, taking care of keeping the indentation when
    /// new line separators are found.
    /// </summary>
    /// <param name="message"></param>
    public void Append(string message)
    {
        if (message == null) return;
        if (message.Length == 0) return;

        var count = message.Count(x => x == '\n');
        if (count == 0)
        {
            if (Origin) Builder.Append(Header());
            Builder.Append(message);

            Origin = message.EndsWith("\n");
        }
        else
        {
            var parts = TokenizeNL(message, count);
            foreach (var part in parts)
            {
                if (Origin) Builder.Append(Header());
                Builder.Append(part);

                Origin = part.EndsWith("\n");
            }
        }
    }

    // Returns an array where the new-line tokens are explicitly identified...
    static string[] TokenizeNL(string message, int size)
    {
        var list = new List<string>((size * 2) + 1);
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
    /// Appends a new line to this instance.
    /// </summary>
    public void AppendLine() => AppendLine("");

    /// <summary>
    /// Appends the given contents to this instance, and then adds a new line, taking care of
    /// keeping the indentation when new line separators are found.
    /// </summary>
    /// <param name="message"></param>
    public void AppendLine(string message)
    {
        Append(message);
        Append("\n");
    }

    /// <summary>
    /// Clears this instance.
    /// </summary>
    public void Clear()
    {
        Builder.Clear();
        Origin = true;
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
    static Dictionary<int, string> _Headers = new();
}