namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents the head, body and tail parts of a given database token.
/// </summary>
public class DbTokenParts
{
    /// <summary>
    /// Initializes a new instance with the given head, body and tail parts.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="body"></param>
    /// <param name="tail"></param>
    public DbTokenParts(DbTokenInvoke? head, DbToken body, DbTokenInvoke? tail)
    {
        Head = head;
        Body = body.ThrowWhenNull();
        Tail = tail;
    }

    /// <summary>
    /// Deconstructor.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="body"></param>
    /// <param name="tail"></param>
    public void Deconstruct(out DbTokenInvoke? head, out DbToken body, out DbTokenInvoke? tail)
    {
        head = Head;
        body = Body;
        tail = Tail;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder();

        if (Head is not null) sb.Append(Head);
        sb.Append(Body);
        if (Tail is not null) sb.Append(Tail);

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// The head part carried by this instance, or null if any.
    /// </summary>
    public DbTokenInvoke? Head { get; }

    /// <summary>
    /// The body part carried by this instance, which may be the given source one if it didn't
    /// has any head or tail part.
    /// </summary>
    public DbToken Body
    {
        get => _Body;
        init => _Body = value.ThrowWhenNull();
    }
    DbToken _Body = default!;

    /// <summary>
    /// The head part carried by this instance, or null if any.
    /// </summary>
    public DbTokenInvoke? Tail { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance built from the given source token.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static DbTokenParts Create(DbToken source)
    {
        source.ThrowWhenNull();

        // Easy case...
        if (source is not DbTokenHosted) return new(null, source, null);

        // Extracting parts if possible...
        DbToken body = source;
        DbTokenInvoke? head = null;
        DbTokenInvoke? tail = null;

        var arg = body.GetArgument()
            ?? throw new UnExpectedException("Cannot obtain dynamic argument.").WithData(source);
        
        ExtractTail();
        ExtractHead();

        // Adjusting and finishing...
        return new(head, body, tail);

        /// <summary>
        /// Extracts the tail part, if possible.
        /// </summary>
        void ExtractTail()
        {
            if (body is not DbTokenInvoke) return;

            var temp = body; while (temp is DbTokenInvoke invoke)
            {
                if (tail is null)
                {
                    tail = invoke;
                    tail = tail.ChangeHost(arg);
                }
                else
                {
                    var args = new List<DbToken>(invoke.Arguments);
                    args.AddRange(tail.Arguments);
                    tail = new DbTokenInvoke(arg, args);
                }

                body = invoke.Host;
                temp = body;
            }
        }

        /// <summary>
        /// Extracts the head part, if possible.
        /// </summary>
        void ExtractHead()
        {
            // TODO: ExtractHead is not working...
            throw null;

            var prev = body;
            var temp = body; while (temp is not DbTokenArgument)
            {
                var host = ((DbTokenHosted)temp).Host;

                if (host is DbTokenArgument && temp is DbTokenInvoke invoke)
                {
                    if (head is null)
                    {
                        head = invoke;
                        head = head.ChangeHost(arg);
                    }
                    else
                    {
                        var args = new List<DbToken>(head.Arguments);
                        args.Add(invoke.Arguments);
                        head = new DbTokenInvoke(arg, args);
                    }
                }

                prev = temp;
                temp = host;
            }
        }
    }
}