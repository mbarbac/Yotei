namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IDeleteCommand"/>
[Cloneable]
[InheritWiths]
public partial class DeleteCommand : EnumerableCommand, IDeleteCommand
{
    FragmentTerminal.Master _HeadFragment = default!;
    FragmentTerminal.Master _TailFragment = default!;
    FragmentWhere.Master _WhereFragment = default!;

    void OnInitialize(IDbToken token)
    {
        var visitor = new DbTokenVisitor(Connection).ToRawVisitor();
        var name = visitor.TokenToLiteral(token).NotNullNotEmpty();

        PrimarySource = Identifier.Create(Engine, name);

        _HeadFragment = new(this);
        _TailFragment = new(this);
        _WhereFragment = new(this);
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public DeleteCommand(IConnection connection, Func<dynamic, object> table) : base(connection)
    {
        var token = DbLambdaParser.Parse(Engine, table);
        OnInitialize(token);
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public DeleteCommand(IConnection connection, Func<dynamic, string> table) : base(connection)
    {
        var token = DbLambdaParser.Parse(Engine, table);
        OnInitialize(token);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DeleteCommand(DeleteCommand source) : base(source)
    {
        PrimarySource = source.PrimarySource;
        IsEmptyValid = source.IsEmptyValid;
        _HeadFragment = source._HeadFragment.Clone();
        _TailFragment = source._TailFragment.Clone();
        _WhereFragment = source._WhereFragment.Clone();
    }

    /// <inheritdoc/>
    public override string ToString() => GetCommandInfo(iterable: false, validateEmpty: false).ToString();

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
        var validateEmpty = !IsEmptyValid;
        return GetCommandInfo(iterable, validateEmpty);
    }

    // Note that 'DeleteCommand' is a special one: it can be valid even if empty, if such is
    // explicitly allowed...
    CommandInfo GetCommandInfo(bool iterable, bool validateEmpty)
    {
        var heads = _HeadFragment.Visit();
        var tails = _TailFragment.Visit();
        var where = _WhereFragment.Visit();

        if (validateEmpty && where.IsEmpty && !IsEmptyValid) throw new InvalidOperationException(
            "This DELETE command has not filters, is equivalent to a DELETE ALL one.")
            .WithData(this);

        var builder = new CommandInfo.Builder(Engine);
        if (!heads.IsEmpty) builder.Add(heads);

        builder.Add($"DELETE FROM {PrimarySource.Value}");
        if (iterable) builder.Add(" OUTPUT DELETED.*");
        if (!where.IsEmpty) { builder.Add(" WHERE "); builder.Add(where); }

        if (!tails.IsEmpty) builder.Add(tails);
        return builder.CreateInstance();
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual DeleteCommand WithHeads<T>(params Func<dynamic, T>[] specs)
    {
        specs.ThrowWhenNull();
        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i] ?? throw new ArgumentException($"Specification at index #{i} is null.");
            _HeadFragment.Capture(spec);
        }
        return this;
    }
    IDeleteCommand IDeleteCommand.WithHeads<T>(params Func<dynamic, T>[] specs) => WithHeads(specs);

    /// <inheritdoc/>
    public virtual DeleteCommand WithTails<T>(params Func<dynamic, T>[] specs)
    {
        specs.ThrowWhenNull();
        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i] ?? throw new ArgumentException($"Specification at index #{i} is null.");
            _TailFragment.Capture(spec);
        }
        return this;
    }
    IDeleteCommand IDeleteCommand.WithTails<T>(params Func<dynamic, T>[] specs) => WithTails(specs);

    /// <inheritdoc/>
    public virtual DeleteCommand Where<T>(params Func<dynamic, T>[] specs)
    {
        specs.ThrowWhenNull();
        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i] ?? throw new ArgumentException($"Specification at index #{i} is null.");
            _WhereFragment.Capture(spec);
        }
        return this;
    }
    IDeleteCommand IDeleteCommand.Where<T>(params Func<dynamic, T>[] specs) => Where(specs);

    /// <inheritdoc/>
    public bool IsEmptyValid { get; set; }

    /// <inheritdoc/>
    public override DeleteCommand Clear()
    {
        _HeadFragment.Clear();
        _TailFragment.Clear();
        _WhereFragment.Clear();

        return this;
    }
    IDeleteCommand IDeleteCommand.Clear() => Clear();
}