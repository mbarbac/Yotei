namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IInsertCommand"/>
[Cloneable]
[InheritWiths]
public partial class InsertCommand : EnumerableCommand, IInsertCommand
{
    FragmentTerminal.Master _HeadFragment = default!;
    FragmentTerminal.Master _TailFragment = default!;
    FragmentSetter.Master _ColumnsFragment = default!;

    void OnInitialize(IDbToken token)
    {
        var visitor = new DbTokenVisitor(Connection).ToRawVisitor();
        var name = visitor.TokenToLiteral(token).NotNullNotEmpty();

        PrimarySource = Identifier.Create(Engine, name);

        _HeadFragment = new(this);
        _TailFragment = new(this);
        _ColumnsFragment = new(this);
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public InsertCommand(IConnection connection, Func<dynamic, object> table) : base(connection)
    {
        var token = DbLambdaParser.Parse(Engine, table);
        OnInitialize(token);
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public InsertCommand(IConnection connection, Func<dynamic, string> table) : base(connection)
    {
        var token = DbLambdaParser.Parse(Engine, table);
        OnInitialize(token);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected InsertCommand(InsertCommand source) : base(source)
    {
        PrimarySource = source.PrimarySource;
        _HeadFragment = source._HeadFragment.Clone();
        _TailFragment = source._TailFragment.Clone();
        _ColumnsFragment = source._ColumnsFragment.Clone();
    }

    /// <inheritdoc/>
    public override string ToString() => base.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IIdentifier PrimarySource { get; private set; } = default!;

    /// <inheritdoc/>
    public ICommandExecutor GetExecutor() => Connection.Records.CreateCommandExecutor(this);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override CommandInfo GetCommandInfo() => GetCommandInfo(iterable: false);

    /// <inheritdoc/>
    public override CommandInfo GetCommandInfo(bool iterable)
    {
        var heads = _HeadFragment.Visit();
        var tails = _TailFragment.Visit();
        var names = _ColumnsFragment.VisitNames();
        var values = _ColumnsFragment.VisitValues();

        if (heads.IsEmpty && tails.IsEmpty &&
            names.IsEmpty && values.IsEmpty) return new CommandInfo(Engine);

        var builder = new CommandInfo.Builder(Engine);
        if (!heads.IsEmpty) builder.Add(heads);

        builder.Add($"INSERT INTO {PrimarySource.Value}");
        if (!names.IsEmpty) { builder.Add(" "); builder.Add(names); }
        if (iterable) builder.Add(" OUTPUT INSERTED.*");
        if (!values.IsEmpty) { builder.Add(" VALUES "); builder.Add(values); }

        if (!tails.IsEmpty) builder.Add(tails);
        return builder.CreateInstance();
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual InsertCommand WithHeads<T>(params Func<dynamic, T>[] specs)
    {
        specs.ThrowWhenNull();
        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i] ?? throw new ArgumentException($"Specification at index #{i} is null.");
            _HeadFragment.Capture(spec);
        }
        return this;
    }
    IInsertCommand IInsertCommand.WithHead<T>(params Func<dynamic, T>[] specs) => WithHeads(specs);

    /// <inheritdoc/>
    public virtual InsertCommand WithTails<T>(params Func<dynamic, T>[] specs)
    {
        specs.ThrowWhenNull();
        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i] ?? throw new ArgumentException($"Specification at index #{i} is null.");
            _TailFragment.Capture(spec);
        }
        return this;
    }
    IInsertCommand IInsertCommand.WithTail<T>(params Func<dynamic, T>[] specs) => WithTails(specs);

    /// <inheritdoc/>
    public virtual InsertCommand Columns<T>(params Func<dynamic, T>[] specs)
    {
        specs.ThrowWhenNull();
        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i] ?? throw new ArgumentException($"Specification at index #{i} is null.");
            _ColumnsFragment.Capture(spec);
        }
        return this;
    }
    IInsertCommand IInsertCommand.Columns<T>(params Func<dynamic, T>[] specs) => Columns(specs);

    /// <inheritdoc/>
    public override InsertCommand Clear()
    {
        _HeadFragment.Clear();
        _TailFragment.Clear();
        _ColumnsFragment.Clear();

        base.Clear();
        return this;
    }
    IInsertCommand IInsertCommand.Clear() => Clear();
}