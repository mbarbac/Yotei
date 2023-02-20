namespace Yotei.Generators;

// ========================================================
/// <summary>
/// Represents a <see cref="StringBuilder"/>-alike class used to emit source code.
/// </summary>
public class CodeBuilder
{
    readonly StringBuilder sb = new();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string ToString() => sb.ToString();

    /// <summary>
    /// If this instance is at beginning of a line, or not.
    /// </summary>
    public bool InitLine { get; private set; } = true;

    /// <summary>
    /// The current indentation level on this instance.
    /// </summary>
    public int Tabs
    {
        get => _Tabs;
        set => _Tabs = value >= 0
            ? value
            : throw new ArgumentException(nameof(value), $"Invalid tabs value: {value}");
    }
    int _Tabs = 0;

    /// <summary>
    /// The size in spaces of each tab.
    /// </summary>
    public int TabSize
    {
        get => _TabSize;
        set => _TabSize = value >= 1
            ? value
            : throw new ArgumentException(nameof(value), $"Invalid tab size: {value}");
    }
    int _TabSize = 4;

    /// <summary>
    /// Clears the contents of this instance.
    /// </summary>
    public void Clear()
    {
        sb.Clear();
        InitLine = true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Appends the given message to this instance.
    /// This method does not try to prepend the given message with any header.
    /// </summary>
    /// <param name="message"></param>
    public void NoHeadAppend(string message)
    {
        if (message != null && message.Length > 0)
        {
            sb.Append(message);
            InitLine = false;
        }
    }

    /// <summary>
    /// Appends the given message to this instance, followed by a new line.
    /// This method does not try to prepend the given message with any header.
    /// </summary>
    /// <param name="message"></param>
    public void NoHeadAppendLine(string message)
    {
        if (message != null && message.Length > 0)
        {
            NoHeadAppend(message);
            AppendLine();
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Appends the given message to this instance.
    /// This method tries to add an appropriate header before the message, if this instance
    /// was at the beginning of a line.
    /// </summary>
    /// <param name="message"></param>
    public void Append(string message)
    {
        if (message != null && message.Length > 0)
        {
            TryHead();
            NoHeadAppend(message);
        }
    }

    /// <summary>
    /// Unconditionally appends a new line to this instance.
    /// </summary>
    public void AppendLine()
    {
        sb.AppendLine();
        InitLine = true;
    }

    /// <summary>
    /// Appends the given message to this instance, followed by a new line.
    /// This method tries to add an appropriate header before the message, if this instance
    /// was at the beginning of a line.
    /// </summary>
    /// <param name="message"></param>
    public void AppendLine(string message)
    {
        if (message != null && message.Length > 0)
        {
            TryHead();
            NoHeadAppendLine(message);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Appends a header to this instance, provided it is at the beginning of a line.
    /// </summary>
    void TryHead()
    {
        if (InitLine)
        {
            sb.Append(Head(Tabs, TabSize));
            InitLine = false;
        }
    }

    /// <summary>
    /// Gets a head string with the appropriate number of spaces.
    /// </summary>
    static string Head(int tabs, int size)
    {
        var spaces = tabs * size;
        if (spaces == 0) return string.Empty;

        if (!_Heads.TryGetValue(spaces, out var head))
            _Heads[spaces] = (head = new string(' ', spaces));

        return head;
    }

    static Dictionary<int, string> _Heads = new();
}