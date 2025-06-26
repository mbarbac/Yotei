using System.ComponentModel.DataAnnotations;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IQueryCommand"/>
[Cloneable]
[InheritWiths]
public partial class QueryCommand : EnumerableCommand, IQueryCommand
{
    FragmentTerminal.Master _HeadFragment = default!;
    FragmentTerminal.Master _TailFragment = default!;
    FragmentFrom.Master _FromFragment = default!;
    FragmentWhere.Master _WhereFragment = default!;
    FragmentOrderBy.Master _OrderByFragment = default!;
    FragmentSelect.Master _SelectFragment = default!;
    FragmentJoin.Master _JoinFragment = default!;
    FragmentGroupBy.Master _GroupByFragment = default!;
    FragmentWhere.Master _HavingFragment = default!;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="connection"></param>
    public QueryCommand(IConnection connection) : base(connection)
    {
        _HeadFragment = new(this);
        _TailFragment = new(this);
        _FromFragment = new(this);
        _WhereFragment = new(this);
        _OrderByFragment = new(this);
        _SelectFragment = new(this);
        _JoinFragment = new(this);
        _GroupByFragment = new(this);
        _HavingFragment = new(this);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected QueryCommand(QueryCommand source) : base(source)
    {
        _HeadFragment = source._HeadFragment.Clone();
        _TailFragment = source._TailFragment.Clone();
        _FromFragment = source._FromFragment.Clone();
        _WhereFragment = source._WhereFragment.Clone();
        _OrderByFragment = source._OrderByFragment.Clone();
        _SelectFragment = source._SelectFragment.Clone();
        _JoinFragment = source._JoinFragment.Clone();
        _GroupByFragment = source._GroupByFragment.Clone();
        _HavingFragment = source._HavingFragment.Clone();
        _Top = source._Top;
        Distinct = source.Distinct;
    }

    /// <inheritdoc/>
    public override string ToString() => base.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override CommandInfo GetCommandInfo() => GetCommandInfo(true);

    /// <inheritdoc/>
    public override CommandInfo GetCommandInfo(bool _)
    {
        var heads = _HeadFragment.Visit();
        var tails = _TailFragment.Visit();
        var froms = _FromFragment.Visit();
        var wheres = _WhereFragment.Visit();
        var orders = _OrderByFragment.Visit();
        var selects = _SelectFragment.Visit();
        var joins = SupportsJoin ? _JoinFragment.Visit() : new CommandInfo.Builder(Engine);
        var groups = SupportsGroupBy ? _GroupByFragment.Visit() : new CommandInfo.Builder(Engine);
        var havings = SupportsHaving ? _HavingFragment.Visit() : new CommandInfo.Builder(Engine);

        if (heads.IsEmpty && tails.IsEmpty && froms.IsEmpty) return new CommandInfo(Engine);

        var builder = new CommandInfo.Builder(Engine);
        if (!heads.IsEmpty) builder.Add(heads);

        builder.Add("SELECT");
        if (Top > 0) builder.Add($" TOP {Top}");
        if (Distinct) builder.Add(" DISTINCT");
        if (selects.IsEmpty) builder.Add(" *"); else builder.Add(selects);

        if (!froms.IsEmpty) { builder.Add(" FROM "); builder.Add(froms); }
        if (!joins.IsEmpty) { builder.Add(" "); builder.Add(joins); }
        if (!wheres.IsEmpty) { builder.Add(" WHERE "); builder.Add(wheres); }
        if (!orders.IsEmpty) { builder.Add(" ORDER BY "); builder.Add(orders); }
        if (!groups.IsEmpty) { builder.Add(" GROUP BY "); builder.Add(groups); }
        if (!havings.IsEmpty) { builder.Add(" HAVING "); builder.Add(havings); }

        if (Skip > 0 && Take > 0)
        {
            builder.Add($" OFFSET {Skip} ROWS");
            builder.Add($" FETCH NEXT {Take} ROWS ONLY");
        }
        else if (Skip > 0)
        {
            builder.Add($" OFFSET {Skip} ROWS");
        }
        else if (Take > 0)
        {
            //Top(take); Take(0);
            //str = base.GenerateText(parameters, iterable);
            //Top(0); Take(take);
        }

        if (!tails.IsEmpty) builder.Add(tails);
        return builder.CreateInstance();
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override bool SupportsNativePaging
    {
        get => _OrderByFragment.Count > 0 && (Skip > 0 || Take > 0);
        init => base.SupportsNativePaging = value;
    }

    /// <inheritdoc/>
    public virtual QueryCommand WithHead<T>(params Func<dynamic, T>[] specs)
    {
        specs.ThrowWhenNull();
        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i] ?? throw new ArgumentException($"Specification at index #{i} is null.");
            _HeadFragment.Capture(spec);
        }
        return this;
    }
    IQueryCommand IQueryCommand.WithHead<T>(params Func<dynamic, T>[] specs) => WithHead(specs);

    /// <inheritdoc/>
    public virtual QueryCommand WithTail<T>(params Func<dynamic, T>[] specs)
    {
        specs.ThrowWhenNull();
        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i] ?? throw new ArgumentException($"Specification at index #{i} is null.");
            _TailFragment.Capture(spec);
        }
        return this;
    }
    IQueryCommand IQueryCommand.WithTail<T>(params Func<dynamic, T>[] specs) => WithTail(specs);

    /// <inheritdoc/>
    public virtual QueryCommand From<T>(params Func<dynamic, T>[] specs)
    {
        specs.ThrowWhenNull();
        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i] ?? throw new ArgumentException($"Specification at index #{i} is null.");
            _FromFragment.Capture(spec);
        }
        return this;
    }
    IQueryCommand IQueryCommand.From<T>(params Func<dynamic, T>[] specs) => From(specs);

    /// <inheritdoc/>
    public virtual QueryCommand Where<T>(params Func<dynamic, T>[] specs)
    {
        specs.ThrowWhenNull();
        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i] ?? throw new ArgumentException($"Specification at index #{i} is null.");
            _WhereFragment.Capture(spec);
        }
        return this;
    }
    IQueryCommand IQueryCommand.Where<T>(params Func<dynamic, T>[] specs) => Where(specs);

    /// <inheritdoc/>
    public virtual QueryCommand OrderBy<T>(params Func<dynamic, T>[] specs)
    {
        specs.ThrowWhenNull();
        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i] ?? throw new ArgumentException($"Specification at index #{i} is null.");
            _OrderByFragment.Capture(spec);
        }
        return this;
    }
    IQueryCommand IQueryCommand.OrderBy<T>(params Func<dynamic, T>[] specs) => OrderBy(specs);

    /// <inheritdoc/>
    public virtual QueryCommand Select<T>(params Func<dynamic, T>[] specs)
    {
        specs.ThrowWhenNull();
        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i] ?? throw new ArgumentException($"Specification at index #{i} is null.");
            _SelectFragment.Capture(spec);
        }
        return this;
    }
    IQueryCommand IQueryCommand.Select<T>(params Func<dynamic, T>[] specs) => Select(specs);

    /// <inheritdoc/>
    public virtual int Top
    {
        get => _Top;
        set
        {
            if (value > 0) { Skip = -1; Take = -1; }
            _Top = value >= 0 ? value : -1;
        }
    }
    int _Top = -1;

    /// <inheritdoc/>
    public override QueryCommand Clear()
    {
        _HeadFragment.Clear();
        _TailFragment.Clear();
        _FromFragment.Clear();
        _WhereFragment.Clear();
        _OrderByFragment.Clear();
        _SelectFragment.Clear();
        _JoinFragment.Clear();
        _GroupByFragment.Clear();
        _HavingFragment.Clear();
        _Top = -1;
        Distinct = false;

        base.Clear();
        return this;
    }
    IQueryCommand IQueryCommand.Clear() => Clear();

    // ----------------------------------------------------

    protected virtual bool SupportsDistinct => true;
    protected virtual bool SupportsJoin => true;
    protected virtual bool SupportsGroupBy => true;
    protected virtual bool SupportsHaving => true;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Distinct { get; set; }

    /// <inheritdoc/>
    public virtual QueryCommand Join<T>(params Func<dynamic, T>[] specs)
    {
        specs.ThrowWhenNull();
        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i] ?? throw new ArgumentException($"Specification at index #{i} is null.");
            _JoinFragment.Capture(spec);
        }
        return this;
    }
    IQueryCommand IQueryCommand.Join<T>(params Func<dynamic, T>[] specs) => Join(specs);

    /// <inheritdoc/>
    public virtual QueryCommand GroupBy<T>(params Func<dynamic, T>[] specs)
    {
        specs.ThrowWhenNull();
        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i] ?? throw new ArgumentException($"Specification at index #{i} is null.");
            _GroupByFragment.Capture(spec);
        }
        return this;
    }
    IQueryCommand IQueryCommand.GroupBy<T>(params Func<dynamic, T>[] specs) => GroupBy(specs);

    /// <inheritdoc/>
    public virtual QueryCommand Having<T>(params Func<dynamic, T>[] specs)
    {
        specs.ThrowWhenNull();
        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i] ?? throw new ArgumentException($"Specification at index #{i} is null.");
            _HavingFragment.Capture(spec);
        }
        return this;
    }
    IQueryCommand IQueryCommand.Having<T>(params Func<dynamic, T>[] specs) => Having(specs);
}