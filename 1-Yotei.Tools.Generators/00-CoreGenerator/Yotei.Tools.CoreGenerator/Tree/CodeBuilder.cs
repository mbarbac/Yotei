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
            : throw new ArgumentException($"Invalid Indent Level: {value}");
    } = 2;

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
        // HIGH: remove comment after span is included
        /*
        message ??= string.Empty;
        args ??= [null];
        if (message.Length > 0 && args.Length > 0) message = string.Format(message, args);

        var numNL = message.Count(static x => x == '\n');
        if (numNL == 0)
        {
            if (AtOrigin) Builder.Append(GetHeader());
            Builder.Append(message);
            AtOrigin = message.EndsWith('\n');
        }
        else
        {
            var items = TokenizeNL(message, numNL);
            foreach (var item in items)
            {
                if (AtOrigin) Builder.Append(GetHeader());
                Builder.Append(item);
                AtOrigin = item.EndsWith('\n');
            }
        }
        */
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
    /// <param name="message"></param>
    /// <param name="numNL"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Returns a string header of the appropriate size using the current indentation size and
    /// level.
    /// </summary>
    /// <returns></returns>
    string GetHeader()
    {
        var num = IndentLevel * IndentSize;

        if (!_Headers.TryGetValue(num, out var header))
        {
            header = new string(' ', num);
            _Headers[num] = header;
        }
        return header;
    }
}