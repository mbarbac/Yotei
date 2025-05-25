namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <inheritdoc cref="ICommandEnumerator"/>
public class CommandEnumerator : ORM.Code.CommandEnumerator, ICommandEnumerator
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    public CommandEnumerator(IEnumerableCommand command, CancellationToken token = default)
        : base(command, token)
    {
        if (command.Connection is not IConnection)
            throw new ArgumentException(
                "Command's connection is not a relational one.")
                .WithData(command)
                .WithData(command.Connection);
    }

    /// <inheritdoc/>
    public override string ToString() => $"Relational.Enumerator({Command})";

    // ----------------------------------------------------

    DbCommand? DbCommand;
    DbDataReader? DbReader;

    /// <summary>
    /// Returns an appropriate schema using the contents of the given metadata table, or null
    /// if the schema cannot be generated.
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    protected virtual ISchema? GenerateSchema(DataTable? table)
    {
        if (table == null) return null;

        var connection = (IConnection)Command.Connection;
        var engine = connection.Engine;

        var hiddentags = engine.KnownTags.IsHidden;
        var idtags = engine.KnownTags.IdentifierTags;
        if (idtags.Count == 0) throw new InvalidOperationException(
            "No well-known identifier tags for the associated engine.")
            .WithData(engine);

        // Iterating through the rows...
        var entries = new List<ISchemaEntry>();

        for (int r = 0; r < table.Rows.Count; r++)
        {
            var row = table.Rows[r];

            // Preventing hidden rows...
            if (hiddentags is not null)
            {
                var ishidden = false;
                foreach (var hiddentag in hiddentags)
                {
                    var column = table.Columns[hiddentag];
                    var value = row[column!];
                    
                    if (value is bool temp && temp)
                    {
                        ishidden = true;
                        break;
                    }
                }
                if (ishidden) continue;
            }

            // Iterating through the columns...
            var metas = new List<IMetadataEntry>();

            for (int c = 0; c < table.Columns.Count; c++)
            {
                var name = table.Columns[c].ColumnName;
                var value = row[c] is DBNull ? null : row[c];

                if (value is string str) value = str.NullWhenEmpty();
                if (value is null) continue;
                
                var tag =
                    engine.KnownTags.Find(name) ??
                    new MetadataTag(engine.KnownTags.CaseSensitiveTags, name);

                var temp = metas.Find(x => x.Tag.Contains(tag));
                if (temp != null)
                {
                    var other = temp.Value;
                    if (!value.EqualsEx(other)) throw new InvalidOperationException(
                        "New value for existing metadata entry does not match.")
                        .WithData(temp, "Existing Entry")
                        .WithData(value, "Existing Value")
                        .WithData(other, "New Value");
                }
                else
                {
                    metas.Add(new MetadataEntry(tag, value));
                }
            }

            // Creating a new schema entry...
            var entry = new SchemaEntry(engine, metas);

            if (entry.Identifier.Value == null)
                throw new InvalidOperationException(
                    "Entry identifier is null.")
                    .WithData(entry);

            if (entry.Identifier is IIdentifierChain chain && chain[^1].Value is null)
                throw new InvalidOperationException(
                    "Entry identifier's last part is null or empty.")
                    .WithData(entry);

            // Adding the schema entry...
            entries.Add(entry);
        }

        // Finishing...
        var schema = new Schema(engine, entries);
        return schema;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override ISchema? OnInitialize()
    {
        var connection = (IConnection)Command.Connection;

        DbCommand = connection.Records.CreateDbCommand(Command, iterable: true);
        connection.Records.AddTransaction(DbCommand);

        var behavior = CommandBehavior.KeyInfo;
        DbReader = DbCommand.ExecuteReader(behavior);

        var table = DbReader.GetSchemaTable();
        return GenerateSchema(table);
    }

    /// <inheritdoc/>
    protected override async ValueTask<ISchema?> OnInitializeAsync()
    {
        var connection = (IConnection)Command.Connection;

        DbCommand = connection.Records.CreateDbCommand(Command, iterable: true);
        connection.Records.AddTransaction(DbCommand);

        var behavior = CommandBehavior.KeyInfo;
        DbReader = await DbCommand.ExecuteReaderAsync(behavior, CancellationToken).ConfigureAwait(false);

        var table = await DbReader.GetSchemaTableAsync(CancellationToken).ConfigureAwait(false);
        return GenerateSchema(table);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override IRecord? OnNextResult()
    {
        if (DbReader != null && DbReader.Read())
        {
            var values = new List<object?>();

            for (int i = 0; i < DbReader.FieldCount; i++)
            {
                var value = DbReader.IsDBNull(i)
                    ? null
                    : DbReader.GetValue(i);

                values.Add(value);
            }

            var record = new Record(values) { Schema = this.Schema };
            return record;
        }

        return null;
    }

    /// <inheritdoc/>
    protected override async ValueTask<IRecord?> OnNextResultAsync()
    {
        if (DbReader != null && await DbReader.ReadAsync(CancellationToken).ConfigureAwait(false))
        {
            var values = new List<object?>();

            for (int i = 0; i < DbReader.FieldCount; i++)
            {
                var value = await DbReader.IsDBNullAsync(i, CancellationToken).ConfigureAwait(false)
                    ? null
                    : DbReader.GetValue(i);

                values.Add(value);
            }

            var record = new Record(values) { Schema = this.Schema };
            return record;
        }

        return null;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void OnTerminate()
    {
        if (DbReader != null)
        {
            if (!DbReader.IsClosed) DbReader.Close();
            DbReader.Dispose();
            DbReader = null;
        }

        if (DbCommand != null)
        {
            DbCommand.Dispose();
            DbCommand = null;
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask OnTerminateAsync()
    {
        if (DbReader != null)
        {
            if (!DbReader.IsClosed) await DbReader.CloseAsync().ConfigureAwait(false);
            await DbReader.DisposeAsync().ConfigureAwait(false);
            DbReader = null;
        }

        if (DbCommand != null)
        {
            await DbCommand.DisposeAsync().ConfigureAwait(false);
            DbCommand = null;
        }
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void OnAbort()
    {
        DbCommand?.Cancel();
        OnTerminate();
    }

    /// <inheritdoc/>
    protected override async ValueTask OnAbortAsync()
    {
        DbCommand?.Cancel();
        await OnTerminateAsync().ConfigureAwait(false);
    }
}