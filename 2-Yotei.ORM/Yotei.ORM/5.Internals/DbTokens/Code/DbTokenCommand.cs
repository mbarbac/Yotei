﻿namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an embedded command in a database expression.
/// </summary>
[Cloneable]
public partial class DbTokenCommand : IDbToken
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="command"></param>
    public DbTokenCommand(ICommand command) => Command = command.ThrowWhenNull().Clone();

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected DbTokenCommand(DbTokenCommand source) : this(source.Command) { }

    /// <inheritdoc/>
    public override string ToString()
    {
        var info = Command.GetCommandInfo(iterable: false);
        var str = info.Text.UnWrap('(', ')', trim: true, recursive: true);
        return $"({str})";
    }

    /// <inheritdoc/>
    public virtual DbTokenArgument? GetArgument() => null;
    
    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IDbToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not DbTokenCommand valid) return false;

        var tinfo = Command.GetCommandInfo(iterable: false);
        var xinfo = valid.Command.GetCommandInfo(iterable: false);

        if (!tinfo.Engine.Equals(xinfo.Engine)) return false;
        if (string.Compare(tinfo.Text, xinfo.Text, !tinfo.Engine.CaseSensitiveNames) != 0) return false;
        if (!tinfo.Parameters.Equals(xinfo.Parameters)) return false;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IDbToken);

    public static bool operator ==(DbTokenCommand? host, IDbToken? other) => host?.Equals(other) ?? false;

    public static bool operator !=(DbTokenCommand? host, IDbToken? other) => !(host == other);

    /// <inheritdoc/>
    public override int GetHashCode() => Command.GetHashCode();

    // ----------------------------------------------------

    /// <summary>
    /// The embedded command carried by this instance.
    /// <para>This property captures a clone of the original command given at creation time
    /// and, in principle, it is NOT INTENDED for any further modifications.</para>
    /// </summary>
    public ICommand Command { get; }
}