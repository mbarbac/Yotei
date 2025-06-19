namespace Yotei.ORM.Internals;

// ========================================================
public static class DbTokenVisitorExtensions
{
    /// <summary>
    /// Invoked to build an alias using the contents of the given chain which, by default, are
    /// joined without using any separators among them.
    /// </summary>
    /// <param name="visitor"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public static string ParseAlias(this DbTokenVisitor visitor, DbTokenChain chain)
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
}