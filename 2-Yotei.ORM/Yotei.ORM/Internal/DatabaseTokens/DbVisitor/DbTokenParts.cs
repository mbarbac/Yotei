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
        source.ThrowWhenNull();

        var arg = source.GetArgument()
            ?? throw new InvalidOperationException(
                "Cannot obtain dynamic argument from the given source-tree").WithData(source);

        if (source is not DbTokenHosted) return new(null, source, null);

        DbToken body = source.Clone(); // To prevent modifications in the original source...
        DbTokenInvoke? head = null;
        DbTokenInvoke? tail = null;

        ExtractHead();
        ExtractTail();
        var same = head is null && tail is null;
        return new(head, same ? source : body, tail);

        /// <summary>
        /// Extracts the head parts, if possible.
        /// </summary>
        void ExtractHead()
        {
            DbToken item = body;
            DbToken? prev = null;

            while (item is DbTokenHosted hosted)
            {
                if (hosted.Host is DbTokenArgument && hosted is DbTokenInvoke invoke) // Found...
                {
                    if (head is null)
                    {
                        head = invoke.Clone();
                        head.ChangeHost(arg);
                    }
                    else
                    {
                        var args = head.Arguments.ToList();
                        args.AddRange(invoke.Arguments);
                        head = new DbTokenInvoke(arg, args);
                    }

                    if (prev is null) body = arg;
                    else ((DbTokenHosted)prev).ChangeHost(arg);

                    ExtractHead(); // Finding the next one...
                    break;
                }

                prev = item;
                item = hosted.Host;
            }
        }

        /// <summary>
        /// Extracts the tail parts, if possible.
        /// </summary>
        void ExtractTail()
        {
            DbToken item = body;

            while (item is DbTokenInvoke invoke)
            {
                if (tail is null)
                {
                    tail = invoke.Clone();
                    tail.ChangeHost(arg);
                }
                else
                {
                    var args = invoke.Arguments.ToList();
                    args.AddRange(tail.Arguments);
                    tail = new DbTokenInvoke(arg, args);
                }

                body = invoke.Host;
                item = body;
            }
        }
    }
}