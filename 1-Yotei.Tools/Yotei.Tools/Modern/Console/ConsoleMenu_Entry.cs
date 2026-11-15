namespace Yotei.Tools;

partial record ConsoleMenu
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a console menu.
    /// </summary>
    /// <param name="headline"></param>
    /// <param name="execute"></param>
    [DebuggerDisplay("{Header()}")]
    public class Entry(string? headline = null, Action? execute = null)
    {
        readonly string? _Headline = headline;
        readonly Action? _Execute = execute;

        /// <summary>
        /// The headline of this entry, used in the menu.
        /// </summary>
        /// <returns></returns>
        public virtual string Headline() => _Headline ?? string.Empty;

        /// <summary>
        /// The action invoked when this entry is selected.
        /// </summary>
        public virtual void Execute() => _Execute?.Invoke();
    }
}