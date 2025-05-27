#pragma warning disable IDE0038

namespace Yotei.ORM.Internal;

// ========================================================
public static class DbTokenExtensions
{
    /// <summary>
    /// Removes the last occurrence of a token that matches the given predicate, from the tree
    /// represented by the given source token. If so, returns the new tree with that item
    /// removed, and the item itself in the out parameter. Otherwise, returns the original
    /// tree and the out parameter is set to <c>null</c>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static DbToken RemoveFirst(
        this DbToken source, Predicate<DbToken> predicate, out DbToken? removed)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();

        removed = null;

        if (source is DbTokenArgument) return source;
        if (source is not DbTokenHosted) return source;

        var cloned = source.Clone();
        var item = cloned;
        var prev = (DbToken?)null;
        var found = (DbToken?)null;
        var first = true;

        while (item is DbTokenHosted)
        {
            if (predicate(item))
            {
                found = item;
                if (!first) prev = item;
            }

            first = false;
            item = ((DbTokenHosted)item).Host;
        }

        if (found != null)
        {
            if (prev == null) // Found the last one in the tree...
            {
                var host = ((DbTokenHosted)found).Host;

                removed = found;
                return host;
            }
            else // Found an intermediate one...
            {
                var host = ((DbTokenHosted)found).Host;

                removed = found;
                ((DbTokenHosted)prev).ChangeHost(host);
                return cloned;
            }
        }

        return source;
    }

    /// <summary>
    /// Removes the last occurrence of a token that matches the given predicate, from the tree
    /// represented by the given source token. If so, returns the new tree with that item
    /// removed, and the item itself in the out parameter. Otherwise, returns the original
    /// tree and the out parameter is set to <c>null</c>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static DbToken RemoveLast(
        this DbToken source, Predicate<DbToken> predicate, out DbToken? removed)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();

        removed = null;

        if (source is DbTokenArgument) return source;
        if (source is not DbTokenHosted) return source;

        var cloned = source.Clone();
        var item = cloned;
        var prev = (DbToken?)null;

        while (item is DbTokenHosted)
        {
            if (predicate(item))
            {
                if (prev == null) // Found the last one in the tree...
                {
                    var host = ((DbTokenHosted)item).Host;

                    removed = item;
                    return host;
                }
                else // Found an intermediate one...
                {
                    var host = ((DbTokenHosted)item).Host;

                    removed = item;
                    ((DbTokenHosted)prev).ChangeHost(host);
                    return cloned;
                }
            }

            prev = item;
            item = ((DbTokenHosted)item).Host;
        }

        return source;
    }

    /// <summary>
    /// Removes all the occurrences of tokens that match the given predicate, from the tree
    /// represented by the given source token. If so, returns the new tree with that items
    /// removed, and the items themselves in the out parameter. Otherwise, returns the original
    /// tree and the out parameter is set to an empty one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public static DbToken RemoveAll(
        this DbToken source, Predicate<DbToken> predicate, out List<DbToken> list)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();

        list = new List<DbToken>();

        while (true)
        {
            source = source.RemoveLast(predicate, out var removed);

            if (removed == null) break;
            list.Insert(0, removed);
        }

        return source;
    }
}