namespace Yotei.Tools;

// =============================================================
/// <summary>
/// Represents the options to use to run a console menu.
/// </summary>
public record struct ConsoleMenuOptions
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public ConsoleMenuOptions() { }

    /// <summary>
    /// Whether to replicate the result of the selection to the not console-alike debug outputs.
    /// <br/> The default value of this property is false.
    /// </summary>
    public bool Debug { get; set; }

    /// <summary>
    /// The interval to wait for the user to select an entry.
    /// <br/> The default value of this property is infinite.
    /// </summary>
    public TimeSpan Timeout
    {
        readonly get => TimeSpan.FromMilliseconds(Milliseconds);
        set => Milliseconds = value.ValidatedTimeout;
    }
    long Milliseconds = -1;

    /// <summary>
    /// The color to use for the '[_]' selector.
    /// <br/> The default value of this property is 'Green'.
    /// </summary>
    public ConsoleColor SelectorColor { get; set; } = ConsoleColor.Green;

    /// <summary>
    /// The color to use for entry description after the selector.
    /// <br/> The default value of this property is 'White'.
    /// </summary>
    public ConsoleColor DescriptionColor { get; set; } = ConsoleColor.White;

    /// <summary>
    /// The background color to use.
    /// <br/> The default value of this property is the background color when this instance was
    /// created.
    /// </summary>
    public ConsoleColor BackgroundColor { get; set; } = Console.BackgroundColor;
}