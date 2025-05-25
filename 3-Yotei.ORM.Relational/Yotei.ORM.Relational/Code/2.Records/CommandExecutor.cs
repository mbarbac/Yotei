namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <inheritdoc cref="ICommandExecutor"/>
public class CommandExecutor : ORM.Code.CommandExecutor, ICommandExecutor
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    public CommandExecutor(IExecutableCommand command) : base(command)
    {
        if (command.Connection is not IConnection)
            throw new ArgumentException(
                "Command's connection is not a relational one.")
                .WithData(command)
                .WithData(command.Connection);
    }

    /// <inheritdoc/>
    public override string ToString() => $"Relational.Executor({Command})";

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override int OnExecute()
    {
        var connection = (IConnection)Command.Connection;
        var dbcmd = (DbCommand?)null;

        try
        {
            dbcmd = connection.Records.CreateDbCommand(Command, iterable: false);

            var r = dbcmd.ExecuteNonQuery();
            return r;
        }
        catch
        {
            if (dbcmd != null)
            {
                try { dbcmd.Cancel(); }
                catch { }
            }

            throw;
        }
        finally
        {
            if (dbcmd != null) dbcmd.Dispose();
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask<int> OnExecuteAsync(CancellationToken token)
    {
        var connection = (IConnection)Command.Connection;
        var dbcmd = (DbCommand?)null;

        try
        {
            dbcmd = connection.Records.CreateDbCommand(Command, iterable: false);

            var r = await dbcmd.ExecuteNonQueryAsync(token).ConfigureAwait(false);
            return r;
        }
        catch
        {
            if (dbcmd != null)
            {
                try { dbcmd.Cancel(); }
                catch { }
            }

            throw;
        }
        finally
        {
            if (dbcmd != null) await dbcmd.DisposeAsync().ConfigureAwait(false);
        }
    }
}