namespace Yotei.ORM.Internals;

// ========================================================
public static class DbTokenExtensions
{
    /// <summary>
    /// Returns a flat list from the given source source, from first to last, provided it is
    /// a hosted or chained one.
    /// </summary>
    static bool ToFlat(IDbToken source, out List<IDbToken> items, out bool isChain)
    {
        if (source is DbTokenChain chain)
        {
            items = chain.ToList();
            isChain = true;
            return true;
        }
        if (source is DbTokenHosted)
        {
            items = []; while (true)
            {
                items.Add(source);

                if (source is DbTokenHosted hosted) source = hosted.Host;
                else break;
            }

            items.Reverse();
            isChain = false;
            return true;
        }

        items = null!;
        isChain = default;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns either a chained instance or a tree-alike one from the given list.
    /// </summary>
    /// <param name="items"></param>
    /// <param name="isChain"></param>
    /// <returns></returns>
    static IDbToken ToTree(List<IDbToken> items, bool isChain)
    {
        if (isChain) // Chains are easy...
        {
            return new DbTokenChain(items);
        }
        else // We can assume at least one element because arguments are not removed...
        {
            items.Reverse();
            IDbToken token = items[0];

            for (int i = 0; i < (items.Count - 1); i++)
            {
                if (items[i] is DbTokenHosted hosted) hosted.Host = items[i + 1];
                else break;
            }

            return token;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Extracts from the given tree-like source the first ocurrence in that tree of a token that
    /// matches the given predicate. If so, returns the resulting tree without that token, that is
    /// placed in the out argument. Otherwise, returns the original source and the out argument is
    /// set to <c>null</c>.
    /// <br/>
    /// If the source token is not a hosted one, then the actual tree to remove the matching token
    /// from is the left-most part of that source, if such is possible. If the removal succeeded,
    /// then the returned token is a new instance of the same original type, where that matching
    /// token has been removed from that left-most part.
    /// <para>
    /// Note that the returned token may be an empty chain or a plain argument one.
    /// </para>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static IDbToken ExtractFirst(
        this IDbToken source,
        Predicate<IDbToken> predicate, out IDbToken? removed)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Extracts from the given tree-like source the last ocurrence in that tree of a token that
    /// matches the given predicate. If so, returns the resulting tree without that token, that is
    /// placed in the out argument. Otherwise, returns the original source and the out argument is
    /// set to <c>null</c>.
    /// <br/>
    /// If the source token is not a hosted one, then the actual tree to remove the matching token
    /// from is the left-most part of that source, if such is possible. If the removal succeeded,
    /// then the returned token is a new instance of the same original type, where that matching
    /// token has been removed from that left-most part.
    /// <para>
    /// Note that the returned token may be an empty chain or a plain argument one.
    /// </para>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static IDbToken ExtractLast(
        this IDbToken source,
        Predicate<IDbToken> predicate, out IDbToken? removed)
    {
        throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Extracts from the given tree-like source all the ocurrences in that tree of tokens that
    /// match the given predicate, from first to last. If so, returns the resulting tree without
    /// that tokens, that are placed in the out argument. Otherwise, returns the original source
    /// and the out argument is set to an empty one.
    /// <br/>
    /// If the source token is not a hosted one, then the actual tree to remove the matching token
    /// from is the left-most part of that source, if such is possible. If the removal succeeded,
    /// then the returned token is a new instance of the same original type, where that matching
    /// token has been removed from that left-most part.
    /// <para>
    /// Note that the returned token may be an empty chain or a plain argument one.
    /// </para>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static IDbToken ExtractAll(
        this IDbToken source,
        Predicate<IDbToken> predicate, out List<IDbToken> removed)
    {
        throw null;
    }
}