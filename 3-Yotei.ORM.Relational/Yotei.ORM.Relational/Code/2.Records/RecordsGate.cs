namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <inheritdoc cref="IRecordsGate"/>
public class RecordsGate : ORM.Code.RecordsGate, IRecordsGate
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public RecordsGate(IConnection connection) : base(connection) { }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public new IConnection Connection => (IConnection)base.Connection;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override ICommandEnumerator CreateCommandEnumerator(
        IEnumerableCommand command, CancellationToken token = default)
    {
        command.ThrowWhenNull();

        if (!ReferenceEquals(Connection, command.Connection))
            throw new InvalidOperationException(
                "Command's connection is not the same as this instance's one.")
                .WithData(command)
                .WithData(this);

        return new CommandEnumerator(command, token);
    }

    /// <inheritdoc/>
    public override ICommandExecutor CreateCommandExecutor(
        IExecutableCommand command)
    {
        command.ThrowWhenNull();

        if (!ReferenceEquals(Connection, command.Connection))
            throw new InvalidOperationException(
                "Command's connection is not the same as this instance's one.")
                .WithData(command)
                .WithData(this);

        return new CommandExecutor(command);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual DbCommand CreateDbCommand(ICommand command, bool iterable)
    {
        command.ThrowWhenNull();

        Connection.ThrowIfDisposed();
        Connection.ThrowIfDisposing();
        if (Connection.DbConnection is null) throw new InvalidOperationException(
            "No physical connection available.")
            .WithData(Connection);

        var info = command.GetCommandInfo(iterable);
        var text = info.Text.NotNullNotEmpty();
        var pars = info.Parameters.ToList();

        var dbcmd = Connection.DbConnection.CreateCommand();
        Remove_Null_Valued_Parameters();
        Add_Parameters();

        dbcmd.CommandText = text;
        return dbcmd;

        /// <summary>
        /// Replaces null-valued parameters with the equivalent NULL strings, if possible.
        /// </summary>
        void Remove_Null_Valued_Parameters()
        {
            var temps = new List<IParameter>();
            foreach (var par in pars)
            {
                if (par.Value is not null) continue;

                text = text.Replace(par.Name, Connection.Engine.NullValueLiteral);
                temps.Add(par);
            }
            foreach (var par in temps)
            {
                pars.Remove(par);
            }
        }

        /// <summary>
        /// Adds the captured parameters to the database command.
        /// </summary>
        void Add_Parameters()
        {
            if (Connection.Engine.PositionalParameters) Add_Parameters_By_Ordinal();
            else Add_Parameters_By_Name();
        }

        /// <summary>
        /// Adds the captured parameters to the database command, by ordinal.
        /// </summary>
        void Add_Parameters_By_Ordinal()
        {
            var sensitive = Connection.Engine.CaseSensitiveNames;
            var comparison = sensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            var temps = new List<(int Index, IParameter Parameter)>();

            for (int i = 0; i < pars.Count; i++)
            {
                var par = pars[i];
                var index = text.IndexOf(par.Name, comparison);

                if (index < 0) continue;
                temps.Add((index, par));
            }

            temps.Sort((a, b) => a.Index.CompareTo(b.Index));

            for (int i = 0; i < temps.Count; i++)
            {
                var par = temps[i].Parameter;
                var item = dbcmd.CreateParameter();
                item.ParameterName = par.Name;
                item.Value = Connection.ToDatabase.TryConvert(par.Value, command.Locale);

                dbcmd.Parameters.Add(item);
            }
        }

        /// <summary>
        /// Adds the captured parameters to the database command, by name.
        /// </summary>
        void Add_Parameters_By_Name()
        {
            for (int i = 0; i < pars.Count; i++)
            {
                var par = pars[i];
                var item = dbcmd.CreateParameter();
                item.ParameterName = par.Name;
                item.Value = Connection.ToDatabase.TryConvert(par.Value, command.Locale);

                dbcmd.Parameters.Add(item);
            }
        }
    }

    // ----------------------------------------------------

    static Dictionary<(Type, string), PropertyInfo> Properties = [];
    static BindingFlags PropertyFlags = BindingFlags.Instance | BindingFlags.NonPublic;

    static PropertyInfo? FindProperty(Type type, string name)
    {
        lock (Properties)
        {
            if (Properties.TryGetValue((type, name), out var prop)) return prop;

            prop = type.GetProperty(name, PropertyFlags);
            if (prop != null)
            {
                Properties.Add((type, name), prop);
            }

            return prop;
        }
    }

    static bool TryGetProperty(object host, string name, out object? value)
    {
        var type = host.GetType();
        var prop = FindProperty(type, name); if (prop != null)
        {
            value = prop.GetValue(host, null);
            return true;
        }

        value = null;
        return false;
    }

    /// <inheritdoc/>
    public virtual void AddTransaction(DbCommand command)
    {
        command.ThrowWhenNull();

        Connection.ThrowIfDisposed();
        Connection.ThrowIfDisposing();
        if (Connection.DbConnection is null) throw new InvalidOperationException(
            "No physical connection available.")
            .WithData(Connection);

        if (command.Transaction != null) return;
        if (System.Transactions.Transaction.Current != null) return;

        if (!TryGetProperty(Connection.DbConnection, "InnerConnection", out var inner)) return;
        if (inner is null || !TryGetProperty(inner, "CurrentTransaction", out var current)) return;
        if (current is null || !TryGetProperty(inner, "Parent", out var parent)) return;
        if (parent is null) return;

        command.Transaction = (DbTransaction)parent;
    }
}