namespace Yotei.ORM.Internals;

// ========================================================
public static class DbTokenVisitorExtensions
{
    /// <summary>
    /// Returns a clone of this instance with the range separator set to ", ".
    /// </summary>
    /// <param name="visitor"></param>
    /// <returns></returns>
    public static DbTokenVisitor ToCommaVisitor(this DbTokenVisitor visitor)
    {
        return visitor.ThrowWhenNull() with { RangeSeparator = ", " };
    }

    /// <summary>
    /// Returns a clone of this instance with the range separator set null.
    /// </summary>
    /// <param name="visitor"></param>
    /// <returns></returns>
    public static DbTokenVisitor ToNullSeparatorVisitor(this DbTokenVisitor visitor)
    {
        return visitor.ThrowWhenNull() with { RangeSeparator = null };
    }

    /// <summary>
    /// Returns a clone of this instance with all its properties set to <c>false</c> or to
    /// <c>null</c>, but keeping the <see cref="DbTokenVisitor.Locale"/> one.
    /// </summary>
    /// <param name="visitor"></param>
    /// <returns></returns>
    public static DbTokenVisitor ToRawVisitor(this DbTokenVisitor visitor)
    {
        return visitor.ThrowWhenNull() with
        {
            UseNullString = false,
            CaptureValues = false,
            ConvertValues = false,
            UseQuotes = false,
            UseTerminators = false,
            RangeSeparator = null,
        };
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to build an alias using the contents of the given chain which, by default, are
    /// joined without using any separators among them.
    /// </summary>
    /// <param name="visitor"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public static string ChainToAlias(this DbTokenVisitor visitor, DbTokenChain chain)
    {
        visitor.ThrowWhenNull();
        chain.ThrowWhenNull();

        visitor = visitor.ToRawVisitor();

        var engine = visitor.Connection.Engine;
        var builder = visitor.VisitRange(chain);
        var id = new IdentifierPart(engine, builder.Text);

        var name = engine.UseTerminators ? id.Value : id.UnwrappedValue;
        return name
            ?? throw new ArgumentException("Generated alias resolves into null.").WithData(chain);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reduces the given token to a suitable literal.
    /// </summary>
    /// <param name="visitor"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static string TokenToLiteral(this DbTokenVisitor visitor, IDbToken token)
    {
        visitor.ThrowWhenNull();
        token.ThrowWhenNull();

        visitor = visitor.ToRawVisitor();

        var builder = visitor.Visit(token);
        return builder.Text;
    }
}