namespace Yotei.ORM.Internals;

// ========================================================
public static class DbTokenExtensions
{
    /// <summary>
    /// Removes from the given source tree the first ocurrence of a token that matches the given
    /// predicate. Returns the tree without the removed token or, if not found, the original one.
    /// If removed, it is placed in the out argument.
    /// </summary>
    /// <remarks>
    /// If the source token is not a <see cref="DbTokenHosted"/> one, then the actual tree to
    /// remove the matching token from is taken to be the left-most one of the given instance,
    /// if possible.
    /// </remarks>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static IDbToken RemoveFirst(
        this IDbToken source, Predicate<IDbToken> predicate, out IDbToken? removed)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();

        removed = null;
        IDbToken item = default!;

        // Intercepting special cases...
        switch (source)
        {
            case DbTokenBinary temp:
                item = RemoveFirst(temp.Left, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenBinary(item, temp.Operation, temp.Right);

            case DbTokenConvert.ToType temp:
                item = RemoveFirst(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenConvert.ToType(temp.Type, item);

            case DbTokenConvert.ToSpec temp:
                item = RemoveFirst(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenConvert.ToSpec(temp.Type, item);

            case DbTokenSetter temp:
                item = RemoveFirst(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenSetter(item, temp.Value);

            case DbTokenUnary temp:
                item = RemoveFirst(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenUnary(temp.Operation, item);
        }

        // Source must be a hosted one...
        if (source is DbTokenHosted)
        {
            var arg = source.GetArgument()
                ?? throw new InvalidOperationException(
                    "Cannot obtain the dynamic argument of the given source tree.").WithData(source);

            IDbToken cloned = source.Clone(); // Prevents modifications of original source...
            IDbToken? found = null;
            IDbToken? parent = null;
            IDbToken? prev = null;

            // Finding a candidate to remove...
            removed = null;
            item = cloned;
            while (item is DbTokenHosted hosted)
            {
                if (predicate(item))
                {
                    found = item;
                    parent = prev;
                }

                // We need to iterate further till the start of the tree...
                prev = item;
                item = hosted.Host;
            }

            // Found a token to remove...
            if (found is not null)
            {
                removed = found.Clone();
                ((DbTokenHosted)removed).Host = arg;

                if (parent is null) // Found last one in the tree...
                {
                    var host = ((DbTokenHosted)found).Host;
                    return host;
                }
                else // Found an intermediate one...
                {
                    var host = ((DbTokenHosted)found).Host;
                    ((DbTokenHosted)parent).Host = host;
                    return cloned;
                }
            }
        }

        // Finishing...
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source tree the last ocurrence of a token that matches the given
    /// predicate. Returns the tree without the removed token or, if not found, the original one.
    /// If removed, it is placed in the out argument.
    /// </summary>
    /// <remarks>
    /// If the source token is not a <see cref="DbTokenHosted"/> one, then the actual tree to
    /// remove the matching token from is taken to be the left-most one of the given instance,
    /// if possible.
    /// </remarks>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static IDbToken RemoveLast(
        this IDbToken source, Predicate<IDbToken> predicate, out IDbToken? removed)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();

        removed = null;
        IDbToken item = default!;

        // Intercepting special cases...
        switch (source)
        {
            case DbTokenBinary temp:
                item = RemoveLast(temp.Left, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenBinary(item, temp.Operation, temp.Right);

            case DbTokenConvert.ToType temp:
                item = RemoveLast(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenConvert.ToType(temp.Type, item);

            case DbTokenConvert.ToSpec temp:
                item = RemoveLast(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenConvert.ToSpec(temp.Type, item);

            case DbTokenSetter temp:
                item = RemoveLast(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenSetter(item, temp.Value);

            case DbTokenUnary temp:
                item = RemoveLast(temp.Target, predicate, out removed);
                return removed is null
                    ? source
                    : new DbTokenUnary(temp.Operation, item);
        }

        // Source must be a hosted one...
        if (source is DbTokenHosted)
        {
            var arg = source.GetArgument()
                ?? throw new InvalidOperationException(
                    "Cannot obtain the dynamic argument of the given source tree.").WithData(source);

            IDbToken cloned = source.Clone(); // Prevents modifications of original source...
            IDbToken? prev = null;

            // Finding a candidate to remove...
            removed = null;
            item = cloned;
            while (item is DbTokenHosted hosted)
            {
                if (predicate(item)) // Found a token to remove...
                {
                    removed = item.Clone();
                    ((DbTokenHosted)removed).Host = arg;

                    if (prev is null) // Found last one in the tree...
                    {
                        var host = hosted.Host;
                        return host;
                    }
                    else // Found an intermediate one...
                    {
                        var host = hosted.Host;
                        ((DbTokenHosted)prev).Host = host;
                        return cloned;
                    }
                }

                else // Not found yet...
                {
                    prev = item;
                    item = hosted.Host;
                }
            }
        }

        // Finishing...
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source tree all the ocurrences of a tokens that match the given
    /// predicate. Returns the tree without the removed token or, if not found, the original one.
    /// The removed tokens are placed in the out list.
    /// </summary>
    /// <remarks>
    /// If the source token is not a <see cref="DbTokenHosted"/> one, then the actual tree to
    /// remove the matching token from is taken to be the left-most one of the given instance,
    /// if possible.
    /// </remarks>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static IDbToken RemoveAll(
        this IDbToken source, Predicate<IDbToken> predicate, out List<IDbToken> removed)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();

        removed = [];
        IDbToken temp = source;

        while (true)
        {
            temp = temp.RemoveLast(predicate, out var item);

            if (item is null) break;
            else removed.Insert(0, item);
        }

        return temp;
    }
}