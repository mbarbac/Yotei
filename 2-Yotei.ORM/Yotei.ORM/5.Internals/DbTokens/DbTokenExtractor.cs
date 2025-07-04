namespace Yotei.ORM.Internals;

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

        // Special cases...
        IDbToken token = default!;
        switch (source)
        {
            case DbTokenBinary temp:
                token = ExtractFirst(temp.Left, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenBinary(token, temp.Operation, temp.Right);

            case DbTokenConvert.ToType temp:
                token = ExtractFirst(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenConvert.ToType(temp.Type, token);

            case DbTokenConvert.ToSpec temp:
                token = ExtractFirst(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenConvert.ToSpec(temp.Type, token);

            case DbTokenSetter temp:
                token = ExtractFirst(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenSetter(token, temp.Value);

            case DbTokenUnary temp:
                token = ExtractFirst(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenUnary(temp.Operation, token);
        }

        // Case: source is a chained instance...
        if (source is DbTokenChain chain)
        {
            var items = chain.ToList();

            for (int i = 0; i < items.Count; i++)
            {
                var temp = items[i];
                if (predicate(temp))
                {
                    removed = temp;

                    items.RemoveAt(i);
                    temp = new DbTokenChain(items);
                    return temp;
                }
            }
        }

        // Case: source is a hosted instance...
        else if (source is DbTokenHosted)
        {
            var arg = source.GetArgument() ?? throw new InvalidOperationException(
                "Cannot obtain the dynamic argument of the given source tree.")
                .WithData(source);

            IDbToken cloned = source.Clone();
            IDbToken? prev = null;
            IDbToken? found = null;
            IDbToken? parent = null;

            IDbToken? item = cloned;
            while (item is not null)
            {
                if (item is not DbTokenArgument && predicate(item))
                {
                    parent = prev;
                    found = item;
                }

                prev = item;
                item = (item as DbTokenHosted)?.Host;
            }

            if (found is not null)
            {
                removed = found.Clone();
                if (removed is DbTokenHosted xremoved) xremoved.Host = arg;

                if (parent is null) // Found last in tree...
                {
                    var host = (found as DbTokenHosted)?.Host;
                    return host ?? arg;
                }
                else // Found intermediate one...
                {
                    var host = (found as DbTokenHosted)?.Host;
                    ((DbTokenHosted)parent).Host = host ?? arg;
                    return cloned;
                }
            }
        }

        // Otherwise...
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
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();
        removed = null;

        // Special cases...
        IDbToken token = default!;
        switch (source)
        {
            case DbTokenBinary temp:
                token = ExtractLast(temp.Left, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenBinary(token, temp.Operation, temp.Right);

            case DbTokenConvert.ToType temp:
                token = ExtractLast(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenConvert.ToType(temp.Type, token);

            case DbTokenConvert.ToSpec temp:
                token = ExtractLast(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenConvert.ToSpec(temp.Type, token);

            case DbTokenSetter temp:
                token = ExtractLast(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenSetter(token, temp.Value);

            case DbTokenUnary temp:
                token = ExtractLast(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenUnary(temp.Operation, token);
        }

        // Case: source is a chained instance...
        if (source is DbTokenChain chain)
        {
            var items = chain.ToList();

            for (int i = items.Count - 1; i >= 0; i--)
            {
                var temp = items[i];
                if (predicate(temp))
                {
                    removed = temp;

                    items.RemoveAt(i);
                    temp = new DbTokenChain(items);
                    return temp;
                }
            }
        }

        // Case: source is a hosted instance...
        else if (source is DbTokenHosted)
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

        // Otherwise...
        return source;
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
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();
        removed = [];

        // Case: source is a chained instance...
        if (source is DbTokenChain chain)
        {
            var items = chain.ToList();

            for (int i = 0; i < items.Count; i++)
            {
                var temp = items[i];
                if (predicate(temp))
                {
                    items.RemoveAt(i);
                    removed.Add(temp);
                    i--;
                }
            }

            return items.Count == chain.Count ? source : new DbTokenChain(items);
        }

        // Otherwise...
        else
        {
            while (true)
            {
                source = source.ExtractLast(predicate, out var temp);

                if (temp is null) break;
                removed.Add(temp);
            }

            removed.Reverse();
            return source;
        }
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
    public static IDbToken ExtractHeadInvokes(
        this IDbToken source, out DbTokenInvoke? removed, bool recurrent = true)
    {
        source.ThrowWhenNull();
        removed = null;

        if (source is not DbTokenHosted) return source;
        var arg = source.GetArgument() ?? throw new InvalidOperationException(
            "Cannot obtain the dynamic argument of the given source tree.")
            .WithData(source);

        IDbToken cloned = source.Clone();
        List<IDbToken> items = [];
        IDbToken? item = cloned;
        while (item is not null)
        {
            items.Add(item);
            item = (item as DbTokenHosted)?.Host;
        }
        items.Reverse();

        var found = false;
        while (items.Count > 1)
        {
            item = items[1]; if (item is DbTokenInvoke invoke)
            {
                if (removed is null) removed = invoke;
                else
                {
                    var args = removed.Arguments.ToList();
                    args.AddRange(invoke.Arguments);
                    removed = new DbTokenInvoke(arg, args);
                }

                found = true;
                items.RemoveAt(1);
                if (items.Count > 1 && items[1] is DbTokenHosted hosted) hosted.Host = arg;

                if (!recurrent) break;
            }
            else break;
        }

        return found ? items[^1] : source;
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
    public static IDbToken ExtractTailInvokes(
        this IDbToken source, out DbTokenInvoke? removed, bool recurrent = true)
    {
        source.ThrowWhenNull();
        removed = null;

        if (source is not DbTokenHosted) return source;
        var arg = source.GetArgument() ?? throw new InvalidOperationException(
            "Cannot obtain the dynamic argument of the given source tree.")
            .WithData(source);

        IDbToken cloned = source.Clone();
        List<IDbToken> items = [];
        IDbToken? item = cloned;
        while (item is not null)
        {
            items.Add(item);
            item = (item as DbTokenHosted)?.Host;
        }
        items.Reverse();

        int index;
        var found = false;
        while ((index = items.Count -1) >= 1)
        {
            item = items[index]; if (item is DbTokenInvoke invoke)
            {
                if (removed is null)
                {
                    removed = invoke;
                    removed.Host = arg;
                }
                else
                {
                    var args = invoke.Arguments.ToList();
                    args.AddRange(removed.Arguments);
                    removed = new DbTokenInvoke(arg, args);
                }

                found = true;
                items.RemoveAt(index);

                if (!recurrent) break;
            }
            else break;
        }

        return found ? items[^1] : source;
    }
}