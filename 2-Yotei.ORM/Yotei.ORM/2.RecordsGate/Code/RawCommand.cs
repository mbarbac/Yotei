using System.ComponentModel.DataAnnotations;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRawCommand"/>
public class RawCommand : EnumerableCommand, IRawCommand
{
    readonly FragmentRaw.Master FragmentRaw;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public RawCommand(IConnection connection) : base(connection) => FragmentRaw = new(this);

    /// <summary>
    /// Initializes a new instance with the contents obtained from the given text and optional
    /// collection of values for the command arguments.
    /// <br/> If any values are used, then they must be encoded in the given text using either a
    /// '{n}' positional specification or a '{name}' named one. In the later case, 'name' may or
    /// may not start with the engine's prefix. Unused values or dangling specifications are not
    /// allowed.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="args"></param>
    public RawCommand(IConnection connection, string text, params object?[]? args)
        : this(connection)
        => Append(text, args);

    /// <summary>
    /// Initializes a new instance with the contents obtained from both parsing the given dynamic
    /// lambda expression, and the optional collection of values for the command arguments (which
    /// are used only when the expression resolves into a string).
    /// <br/> If any values are used, then they must be encoded in the given text using either a
    /// '{n}' positional specification or a '{name}' named one. In the later case, 'name' may or
    /// may not start with the engine's prefix. Unused values or dangling specifications are not
    /// allowed.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="spec"></param>
    /// <param name="range"></param>
    public RawCommand(IConnection connection, Func<dynamic, object> spec, params object?[]? range)
        : this(connection)
        => Append(spec, range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected RawCommand(RawCommand source)
        : base(source)
        => FragmentRaw = source.FragmentRaw.Clone();

    /// <inheritdoc/>
    public ICommandExecutor GetExecutor() => Connection.Records.CreateCommandExecutor(this);

    /// <inheritdoc/>
    public override IRawCommand Clone() => new RawCommand(this);
    IEnumerableCommand IEnumerableCommand.Clone() => Clone();
    IExecutableCommand IExecutableCommand.Clone() => Clone();

    /// <inheritdoc/>
    public override IRawCommand WithConnection(IConnection value) => new RawCommand(this) { Connection = value };
    IEnumerableCommand IEnumerableCommand.WithConnection(IConnection value) => WithConnection(value);
    IExecutableCommand IExecutableCommand.WithConnection(IConnection value) => WithConnection(value);

    /// <inheritdoc/>
    public override IRawCommand WithLocale(Locale value) => new RawCommand(this) { Locale = value };
    IEnumerableCommand IEnumerableCommand.WithLocale(Locale value) => WithLocale(value);
    IExecutableCommand IExecutableCommand.WithLocale(Locale value) => WithLocale(value);

    /// <inheritdoc/>
    public override IRawCommand WithSkip(int value) => new RawCommand(this) { Skip = value };

    /// <inheritdoc/>
    public override IRawCommand WithTake(int value) => new RawCommand(this) { Take = value };

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool SupportsNativePaging
    {
        get
        {
            var sensitive = Connection.Engine.CaseSensitiveNames;
            var str = GetCommandInfo().Text;
            return str.StartsWith("SELECT", !sensitive);
        }
    }

    /// <inheritdoc/>
    public override bool IsValid => FragmentRaw.Count > 0;

    /// <inheritdoc/>
    public override ICommandInfo GetCommandInfo() => FragmentRaw.Visit().CreateInstance();

    /// <inheritdoc/>
    public override ICommandInfo GetCommandInfo(bool _) => GetCommandInfo();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override IRawCommand Clear()
    {
        FragmentRaw.Clear();
        return this;
    }
    IExecutableCommand IExecutableCommand.Clear() => Clear();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IRawCommand Append(string text, params object?[]? args)
    {
        text.NotNullNotEmpty(trim: false);
        args ??= [null];

        if (text.Length == 0) throw new ArgumentException(
            "Empty literals are not accepted.")
            .WithData(text);

        var info = new CommandInfo(Connection.Engine, text, args);
        var token = new DbTokenCommandInfo(info);
        var entry = FragmentRaw.CreateEntry(token);

        FragmentRaw.Add(entry);
        return this;
    }

    /// <inheritdoc/>
    public virtual IRawCommand Append(Func<dynamic, object> spec, params object?[]? range)
    {
        FragmentRaw.Capture(spec, range);
        return this;
    }
}