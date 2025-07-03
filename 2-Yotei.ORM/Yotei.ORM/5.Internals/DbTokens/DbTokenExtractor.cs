namespace Yotei.ORM.Internals;

/// <br/>- If the source token is a chain instance, then this method behaves as the standard
/// removal one.

// ========================================================
public static class DbTokenExtractor
{
    /// <summary>
    /// Extracts from the given tree-like source the first ocurrence of a token that matches the
    /// given predicate. If so, returns the resulting tree without that token, which is placed in
    /// the out argument. Otherwise, returns the original source and the out argument is set to
    /// <c>null</c>.
    /// <br/>- If the source token is not a hosted one, then the actual tree to remove from will
    /// be the left-most part of that source, if possible, and the result is a token of the same
    /// type but with its left part with the matching token removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static IDbToken ExtractFirst(
        this IDbToken source, Predicate<IDbToken> predicate, out IDbToken? removed)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();
        removed = null;

        // Case: source is a hosted instance...
        if (source is DbTokenHosted)
        {
            var arg = source.GetArgument() ?? throw new InvalidOperationException(
                "Cannot obtain the dynamic argument of the given source tree.")
                .WithData(source);

            IDbToken cloned = source.Clone();
            IDbToken? prev = null;

            IDbToken? item = cloned;
            while (item is not null)
            {
                if (item is not DbTokenArgument && predicate(item))
                {
                    removed = item.Clone();
                    if (removed is DbTokenHosted xremoved) xremoved.Host = arg;

                    if (prev is null) // Found last one in tree...
                    {
                        var host = (item as DbTokenHosted)!.Host;
                        return host ?? arg;
                    }
                    else // Found an intermediate one...
                    {
                        var host = (item as DbTokenHosted)?.Host;
                        ((DbTokenHosted)prev).Host = host ?? arg;
                        return cloned;
                    }
                }

                prev = item;
                item = (item as DbTokenHosted)?.Host;
            }
        }

        // Othwerwise...
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Extracts from the given tree-like source the last ocurrence of a token that matches the
    /// given predicate. If so, returns the resulting tree without that token, which is placed in
    /// the out argument. Otherwise, returns the original source and the out argument is set to
    /// <c>null</c>.
    /// <br/>- If the source token is not a hosted one, then the actual tree to remove from will
    /// be the left-most part of that source, if possible, and the result is a token of the same
    /// type but with its left part with the matching token removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static IDbToken ExtractLast(
        this IDbToken source, Predicate<IDbToken> predicate, out IDbToken? removed)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Extracts from the given tree-like source all the ocurrences of tokens that match the
    /// given predicate. If so, returns the resulting tree without that tokens, which are placed
    /// in the out argument. Otherwise, returns the original source and the out argument is set
    /// to <c>null</c>.
    /// <br/>- If the source token is not a hosted one, then the actual tree to remove from will
    /// be the left-most part of that source, if possible, and the result is a token of the same
    /// type but with its left part with the matching tokens removed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static IDbToken ExtractAll(
        this IDbToken source, Predicate<IDbToken> predicate, out List<IDbToken> removed)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Extracts from the given tree-like source the combined chain of head invoke operations.
    /// If any is found, then returns the resulting tree without that head tokens, and sets the
    /// out argument to an invoke one with the combined arguments, in order. Otherwise, returns
    /// the original instance and sets the out argument to <c>null</c>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static IDbToken ExtractHeadInvokes(IDbToken source, out DbTokenInvoke? removed)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Extracts from the given tree-like source the combined chain of tail invoke operations.
    /// If any is found, then returns the resulting tree without that tail tokens, and sets the
    /// out argument to an invoke one with the combined arguments, in order. Otherwise, returns
    /// the original instance and sets the out argument to <c>null</c>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static IDbToken ExtractTailInvokes(IDbToken source, out DbTokenInvoke? removed)
    {
        throw null;
    }
}