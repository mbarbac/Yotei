namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents the head, body and tail parts extracted from a given source token, where the head
/// and tail ones are the combined chain of invoke operations at the head or tail or that source
/// token tree.
/// </summary>
/// <param name="Head"></param>
/// <param name="Body"></param>
/// <param name="Tail"></param>
public record DbTokenParts(DbTokenInvoke? Head, DbToken Body, DbTokenInvoke? Tail);

// ========================================================
public static partial class DbTokenExtensions
{
    /// <summary>
    /// Extracts the head, body and tail parts from a given source token, where the head and tail
    /// ones are the combined chain of invoke operations at the head or tail or that source token
    /// tree.
    /// <br/> If not head and tail are detected, then the body is the same as the given source.
    /// <br/> In case of ambiguous head or tail, head ones take precedence.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static DbTokenParts ExtractParts(this DbToken source)
    {
        throw null;
    }
}