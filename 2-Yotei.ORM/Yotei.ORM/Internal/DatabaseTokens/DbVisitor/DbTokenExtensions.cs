namespace Yotei.ORM.Internal;

// ========================================================
public static partial class DbTokenExtensions
{
    /// <summary>
    /// Removes from the given source the first ocurrence of the token that matches the given
    /// predicate. If found, returns the modified source token and the removed one in the out
    /// parameter. Otherwise, just returns the original source.
    /// <br/> If source is not a tree (hosted) one, then the actual source to remove from will
    /// be the left-most target token of the given instance, if possible.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static DbToken RemoveFirst(
        this DbToken source, Predicate<DbToken> predicate, out DbToken? removed)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();

        if (source is DbTokenChain chain) source = chain.Reduce();

        removed = null;
        DbToken item = default!;

        // Intercepting some special cases where the left-most tree-token may exist...
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

            default:
                return source is DbTokenHosted hosted
                    ? RemoveFirstCore(hosted, predicate, out removed)
                    : source;
        }
    }

    /// <summary>
    /// Invoked to remove the first token that matches the given predicate from the given source
    /// tree of tokens.
    /// </summary>
    static DbToken RemoveFirstCore(
        this DbTokenHosted source, Predicate<DbToken> predicate, out DbToken? removed)
    {
        removed = null;

        var arg = source.GetArgument()
            ?? throw new InvalidOperationException(
                "Cannot obtain dynamic argument from the given source-tree").WithData(source);

        DbToken cloned = source.Clone(); // To prevent modifications in the original source...
        DbToken item = cloned;
        DbToken? found = null;
        DbToken? parent = null;
        DbToken? prev = null;

        // Let's find a candidate to remove...
        while (item is DbTokenHosted hosted)
        {
            if (predicate(item))
            {
                found = item;
                parent = prev;
            }

            // Any case, we need to dig further till the start of the tree...
            prev = item;
            item = hosted.Host;
        }

        // Found a token to remove...
        if (found != null)
        {
            removed = found.Clone();
            ((DbTokenHosted)removed).ChangeHost(arg);

            if (parent is null) // Found last one in the tree...
            {
                var host = ((DbTokenHosted)found).Host;
                return host;
            }
            else // Found an intermediate one...
            {
                var host = ((DbTokenHosted)found).Host;
                ((DbTokenHosted)parent).ChangeHost(host);
                return cloned;
            }
        }

        // Not found...
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source the last ocurrence of the token that matches the given
    /// predicate. If found, returns the modified source token and the removed one in the out
    /// parameter. Otherwise, just returns the original source.
    /// <br/> If source is not a tree (hosted) one, then the actual source to remove from will
    /// be the left-most target token of the given instance, if possible.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static DbToken RemoveLast(
        this DbToken source, Predicate<DbToken> predicate, out DbToken? removed)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();

        if (source is DbTokenChain chain) source = chain.Reduce();

        removed = null;
        DbToken item = default!;

        // Intercepting some special cases where the left-most tree-token may exist...
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

            default:
                return source is DbTokenHosted hosted
                    ? RemoveLastCore(hosted, predicate, out removed)
                    : source;
        }
    }

    /// <summary>
    /// Invoked to remove the first token that matches the given predicate from the given source
    /// tree of tokens.
    /// </summary>
    static DbToken RemoveLastCore(
        this DbTokenHosted source, Predicate<DbToken> predicate, out DbToken? removed)
    {
        removed = null;

        var arg = source.GetArgument()
            ?? throw new InvalidOperationException(
                "Cannot obtain dynamic argument from the given source-tree").WithData(source);

        DbToken cloned = source.Clone(); // To prevent modifications in the original source...
        DbToken item = cloned;
        DbToken? prev = null;

        // Let's find a candidate to remove...
        while (item is DbTokenHosted hosted)
        {
            if (predicate(item)) // Found a token to remove...
            {
                removed = item.Clone();
                ((DbTokenHosted)removed).ChangeHost(arg);

                if (prev is null) // Found last one in the tree...
                {
                    var host = hosted.Host;
                    return host;
                }
                else // Found an intermediate one...
                {
                    var host = hosted.Host;
                    ((DbTokenHosted)prev).ChangeHost(host);
                    return cloned;
                }
            }

            else // Not found yet...
            {
                prev = item;
                item = hosted.Host;
            }
        }

        // Not found...
        return source;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source all the ocurrences of tokens that match the given
    /// predicate. If any is found, returns the modified source token and the collection of
    /// removed ones in the out parameter. Otherwise, just returns the original source.
    /// <br/> If source is not a tree (hosted) one, then the actual source to remove from will
    /// be the left-most target token of the given instance, if possible.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public static DbToken RemoveAll(
        this DbToken source, Predicate<DbToken> predicate, out List<DbToken> removed)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();

        removed = [];
        DbToken temp = source;

        while (true)
        {
            temp = temp.RemoveLast(predicate, out var item);
            if (item is null) break;
            removed.Insert(0, item);
        }

        return temp;
    }
}