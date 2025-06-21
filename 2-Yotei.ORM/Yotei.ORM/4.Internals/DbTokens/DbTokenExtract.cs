namespace Yotei.ORM.Internals;

// ========================================================
public static class DbTokenExtract
{
    /// <summary>
    /// Extracts the combined chain of head invoke operations from the given source tree, or just
    /// the first one if no recurrency was requested. If no head operation is found, then the out
    /// '<paramref name="head"/>' parameter is set to null. The '<paramref name="body"/>' one is
    /// set to either the original source, if no head is found, or the remaining tree otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="body"></param>
    /// <param name="head"></param>
    /// <param name="recurrent"></param>
    /// <returns></returns>
    public static bool ExtractHeadInvokes(
        this IDbToken source, out IDbToken body, out DbTokenInvoke? head, bool recurrent = true)
    {
        source.ThrowWhenNull();
        body = source.Clone();
        head = null;

        DbTokenArgument arg = source.GetArgument()!;
        IDbToken item = body;
        bool found = false;
        DbTokenHosted prev = null!;

        ENTRANT:
        while (item is DbTokenHosted hosted)
        {
            if (hosted is DbTokenInvoke invoke && invoke.Host is DbTokenArgument)
            {
                if (prev is not null) prev.Host = arg;

                if (head is null)
                {
                    head = invoke.Clone();
                    found = true;
                }
                else
                {
                    var args = head.Arguments.ToList();
                    args.AddRange(invoke.Arguments);
                    head = new DbTokenInvoke(arg, args);
                }

                if (prev is null) body = arg;

                if (!recurrent) break; // Recurrency not requested...

                item = body;
                prev = null!;
                goto ENTRANT;
            }

            prev = hosted;
            item = hosted.Host;
        }

        if (!found) body = source;
        return found;
    }

    /// <summary>
    /// Extracts the combined chain of tail invoke operations from the given source tree, or just
    /// the first one if no recurrency was requested. If no tail operation is found, then the out
    /// '<paramref name="tail"/>' parameter is set to null. The '<paramref name="body"/>' one is
    /// set to either the original source, if no tail is found, or the remaining tree otherwise.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="body"></param>
    /// <param name="tail"></param>
    /// <param name="recurrent"></param>
    /// <returns></returns>
    public static bool ExtractTailInvokes(
        this IDbToken source, out IDbToken body, out DbTokenInvoke? tail, bool recurrent = true)
    {
        source.ThrowWhenNull();
        body = source.Clone();
        tail = null;

        DbTokenArgument arg = source.GetArgument()!;
        IDbToken item = body;
        bool found = false;

        while (item is DbTokenInvoke invoke)
        {
            if (tail is null)
            {
                tail = invoke.Clone();
                tail.Host = arg;
                found = true;
            }
            else
            {
                var args = invoke.Arguments.ToList();
                args.AddRange(tail.Arguments);
                tail = new DbTokenInvoke(arg, args);
            }

            body = invoke.Host;
            item = body;

            if (!recurrent) break; // Recurrency not requested...
        }

        if (!found) body = source;
        return found;
    }
}