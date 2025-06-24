namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IUpdateCommand"/>
[Cloneable]
[InheritWiths]
public partial class UpdateCommand : EnumerableCommand, IUpdateCommand
{
    FragmentTerminal.Master _HeadFragment = default!;
    FragmentTerminal.Master _TailFragment = default!;
    FragmentWhere.Master _WhereFragment = default!;
    FragmentSetter.Master _ColumnsFragment = default!;

    void OnInitialize(IDbToken token)
    {
        var visitor = new DbTokenVisitor(Connection).ToRawVisitor();
        var name = visitor.TokenToLiteral(token).NotNullNotEmpty();

        PrimarySource = Identifier.Create(Engine, name);

        _HeadFragment = new(this);
        _TailFragment = new(this);
        _WhereFragment = new(this);
        _ColumnsFragment = new(this);
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public UpdateCommand(IConnection connection, Func<dynamic, object> table) : base(connection)
    {
        var token = DbLambdaParser.Parse(Engine, table);
        OnInitialize(token);
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public UpdateCommand(IConnection connection, Func<dynamic, string> table) : base(connection)
    {
        var token = DbLambdaParser.Parse(Engine, table);
        OnInitialize(token);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected UpdateCommand(UpdateCommand source) : base(source)
    {
        PrimarySource = source.PrimarySource;
        _HeadFragment = source._HeadFragment.Clone();
        _TailFragment = source._TailFragment.Clone();
        _WhereFragment = source._WhereFragment.Clone();
        _ColumnsFragment = source._ColumnsFragment.Clone();
    }

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
        var columns = _ColumnsFragment.Visit();
        var where = _WhereFragment.Visit();

        var builder = new CommandInfo.Builder(Engine);
        if (!heads.IsEmpty) builder.Add(heads);

        builder.Add($"UPDATE {PrimarySource.Value}");
        if (!columns.IsEmpty) { builder.Add(" SET "); builder.Add(columns); }
        if (iterable) builder.Add(" OUTPUT INSERTED.*");
        if (!where.IsEmpty) { builder.Add(" WHERE "); builder.Add(where); }

        if (!tails.IsEmpty) builder.Add(tails);
        return builder.CreateInstance();
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual UpdateCommand WithHeads<T>(params Func<dynamic, T>[] specs)
    {
        specs.ThrowWhenNull();
        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i] ?? throw new ArgumentException($"Specification at index #{i} is null.");
            _HeadFragment.Capture(spec);
        }
        return this;
    }
    IUpdateCommand IUpdateCommand.WithHeads<T>(params Func<dynamic, T>[] specs) => WithHeads(specs);

    /// <inheritdoc/>
    public virtual UpdateCommand WithTails<T>(params Func<dynamic, T>[] specs)
    {
        specs.ThrowWhenNull();
        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i] ?? throw new ArgumentException($"Specification at index #{i} is null.");
            _TailFragment.Capture(spec);
        }
        return this;
    }
    IUpdateCommand IUpdateCommand.WithTails<T>(params Func<dynamic, T>[] specs) => WithTails(specs);

    /// <inheritdoc/>
    public virtual UpdateCommand Where<T>(params Func<dynamic, T>[] specs)
    {
        specs.ThrowWhenNull();
        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i] ?? throw new ArgumentException($"Specification at index #{i} is null.");
            _WhereFragment.Capture(spec);
        }
        return this;
    }
    IUpdateCommand IUpdateCommand.Where<T>(params Func<dynamic, T>[] specs) => Where(specs);

    /// <inheritdoc/>
    public virtual UpdateCommand Columns<T>(params Func<dynamic, T>[] specs)
    {
        specs.ThrowWhenNull();
        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i] ?? throw new ArgumentException($"Specification at index #{i} is null.");
            _ColumnsFragment.Capture(spec);
        }
        return this;
    }
    IUpdateCommand IUpdateCommand.Columns<T>(params Func<dynamic, T>[] specs) => Columns(specs);

    /// <inheritdoc/>
    public override UpdateCommand Clear()
    {
        _HeadFragment.Clear();
        _TailFragment.Clear();
        _ColumnsFragment.Clear();
        _WhereFragment.Clear();

        return this;
    }
    IUpdateCommand IUpdateCommand.Clear() => Clear();
}