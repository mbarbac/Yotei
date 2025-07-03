namespace Yotei.ORM.Internals;

// ========================================================
public static class DbTokenExtractor
{
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
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();
        removed = null;

        // Special cases...
        IDbToken item = default!;
        switch (source)
        {
            case DbTokenBinary temp:
                item = ExtractFirst(temp.Left, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenBinary(item, temp.Operation, temp.Right);

            case DbTokenConvert.ToType temp:
                item = ExtractFirst(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenConvert.ToType(temp.Type, item);

            case DbTokenConvert.ToSpec temp:
                item = ExtractFirst(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenConvert.ToSpec(temp.Type, item);

            case DbTokenSetter temp:
                item = ExtractFirst(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenSetter(item, temp.Value);

            case DbTokenUnary temp:
                item = ExtractFirst(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenUnary(temp.Operation, item);
        }

        // If source is a chained or hosted one...
        if (ToFlat(source, out var items, out var isChain))
        {
            for (int i = 0; i < items.Count; i++)
            {
                var temp = items[i];
                if (temp is DbTokenArgument && !isChain) continue;

                if (predicate(temp))
                {
                    items.RemoveAt(i);
                    removed = temp;
                    var r = ToTree(items, isChain);
                    return r;
                }
            }
        }

        // Otherwise...
        return source;
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
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();
        removed = null;

        // Special cases...
        IDbToken item = default!;
        switch (source)
        {
            case DbTokenBinary temp:
                item = ExtractLast(temp.Left, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenBinary(item, temp.Operation, temp.Right);

            case DbTokenConvert.ToType temp:
                item = ExtractLast(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenConvert.ToType(temp.Type, item);

            case DbTokenConvert.ToSpec temp:
                item = ExtractLast(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenConvert.ToSpec(temp.Type, item);

            case DbTokenSetter temp:
                item = ExtractLast(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenSetter(item, temp.Value);

            case DbTokenUnary temp:
                item = ExtractLast(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenUnary(temp.Operation, item);
        }

        // If source is a chained or hosted one...
        if (ToFlat(source, out var items, out var isChain))
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                var temp = items[i];
                if (temp is DbTokenArgument && !isChain) continue;

                if (predicate(temp))
                {
                    items.RemoveAt(i);
                    removed = temp;
                    var r = ToTree(items, isChain);
                    return r;
                }
            }
        }

        // Otherwise...
        return source;
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
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();
        removed = [];

        // Special cases...
        IDbToken item = default!;
        switch (source)
        {
            case DbTokenBinary temp:
                item = ExtractAll(temp.Left, predicate, out removed);
                return removed.Count == 0
                    ? source
                    : new DbTokenBinary(item, temp.Operation, temp.Right);

            case DbTokenConvert.ToType temp:
                item = ExtractAll(temp.Target, predicate, out removed);
                return removed.Count == 0
                    ? source
                    : new DbTokenConvert.ToType(temp.Type, item);

            case DbTokenConvert.ToSpec temp:
                item = ExtractAll(temp.Target, predicate, out removed);
                return removed.Count == 0
                    ? source
                    : new DbTokenConvert.ToSpec(temp.Type, item);

            case DbTokenSetter temp:
                item = ExtractAll(temp.Target, predicate, out removed);
                return removed.Count == 0
                    ? source
                    : new DbTokenSetter(item, temp.Value);

            case DbTokenUnary temp:
                item = ExtractAll(temp.Target, predicate, out removed);
                return removed.Count == 0
                    ? source
                    : new DbTokenUnary(temp.Operation, item);
        }

        // If source is a chained or hosted one...
        if (ToFlat(source, out var items, out var isChain))
        {
            var done = false;
            for (int i = 0; i < items.Count; i++)
            {
                var temp = items[i];
                if (temp is DbTokenArgument && !isChain) continue;

                if (predicate(temp))
                {
                    done = true;
                    items.RemoveAt(i);
                    removed.Add(temp);
                    i--;
                }
            }
            if (done)
            {
                var r = ToTree(items, isChain);
                return r;
            }
        }

        // Otherwise...
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a flat list from the given source source, from first to last, provided it is
    /// a hosted or chained one.
    /// </summary>
    static bool ToFlat(IDbToken source, out List<IDbToken> items, out bool isChain)
    {
        if (source is DbTokenChain chain) // Chains are easy...
        {
            items = chain.ToList();
            isChain = true;
            return true;
        }
        if (source is DbTokenHosted) // From first to last...
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

            for (int i = 0; i < items.Count; i++)
            {
                items[i] = items[i].Clone();
            }
            for (int i = 0; i < (items.Count - 1); i++)
            {
                if (items[i] is DbTokenHosted hosted) hosted.Host = items[i + 1];
                else break;
            }
            return items[0];
        }
    }
}