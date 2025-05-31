#pragma warning disable IDE0008
#pragma warning disable IDE0019

namespace Yotei.ORM.Internal;

// ========================================================
public static class DbTokenExtensions
{
    /// <summary>
    /// Extracts the head, body and tail parts from the given source tree of tokens, where the
    /// head and tail ones are the combined chain of invoke operations at the head or tail of
    /// that tree.
    /// </summary>
    /// <remarks>
    /// If no head and tail are detected, then body is the same as the given source.
    /// <br/> In case a part is an ambiguous head or tail, tail ones take precedence.
    /// </remarks>
    /// <param name="source"></param>
    /// <returns></returns>
    public static (DbTokenInvoke? Head, DbToken Body, DbTokenInvoke? Tail) ExtractParts(
        this DbToken source)
    {
        source.ThrowWhenNull();

        if (source is DbTokenArgument) return (null, source, null);
        if (source is not DbTokenHosted) return (null, source, null);

        var arg = source.GetArgument()
            ?? throw new InvalidOperationException(
                "Cannot obtain dynamic argument of given source tree.").WithData(source);

        DbTokenInvoke? head = null;
        DbTokenInvoke? tail = null;
        DbToken body = source.Clone(); // To prevent source modifications...
        bool changed = false;

        ExtractTail();
        ExtractHead();
        return (head, changed ? body : source, tail);

        /// <summary>
        /// Extracts the tail part, if possible.
        /// </summary>
        void ExtractTail()
        {
            var item = body; while (item is DbTokenInvoke invoke)
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
                changed = true;
            }
        }

        /// <summary>
        /// Extracts the head part, if possible.
        /// </summary>
        void ExtractHead()
        {
            DbToken item = body;
            DbToken? prev = null;

            while (item is DbTokenHosted hosted)
            {
                if (hosted.Host is DbTokenArgument) // First possible token...
                {
                    if (hosted is not DbTokenInvoke invoke) break;

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

                    if (prev is not null) ((DbTokenHosted)prev).ChangeHost(arg);
                    changed = true;

                    ExtractHead(); // Finding the next first one...
                    break;
                }

                prev = item;
                item = hosted.Host;
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes from the given source tree of tokens the last ocurrence of the ones that match
    /// the given predicate. If found, returns the modified tree of tokens and the removed one
    /// in the out parameter. Otherwise, returns the original tree and sets the out parameter
    /// to <c>null</c>.
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

        removed = null;
        if (source is DbTokenArgument) return source;
        if (source is not DbTokenHosted) return source;

        var arg = source.GetArgument()
            ?? throw new InvalidOperationException(
                "Cannot obtain dynamic argument of given source tree.").WithData(source);

        DbToken cloned = source.Clone(); // To prevent source modifications...
        DbToken item = cloned;
        DbToken? prev = null;

        while (item is DbTokenHosted hosted)
        {
            if (predicate(item))
            {
                if (prev is null) // Found last one in tree...
                {
                    removed = item.Clone();
                    if (removed is DbTokenHosted rtemp) rtemp.ChangeHost(arg);

                    var host = hosted.Host;
                    return host;
                }
                else // Found intermediate one...
                {
                    removed = item.Clone();
                    if (removed is DbTokenHosted rtemp) rtemp.ChangeHost(arg);

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
    /// Removes from the given source tree of tokens the first ocurrence of the ones that match
    /// the given predicate. If found, returns the modified tree of tokens and the removed one
    /// in the out parameter. Otherwise, returns the original tree and sets the out parameter
    /// to <c>null</c>.
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

        removed = null;
        if (source is DbTokenArgument) return source;
        if (source is not DbTokenHosted) return source;

        var arg = source.GetArgument()
            ?? throw new InvalidOperationException(
                "Cannot obtain dynamic argument of given source tree.").WithData(source);

        DbToken cloned = source.Clone(); // To prevent source modifications...
        DbToken item = cloned;
        DbToken? found = null;
        DbToken? parent = null;
        DbToken? prev = null;

        while (item is DbTokenHosted hosted)
        {
            if (predicate(item)) // Found a candidate to remove...
            {
                found = item;
                parent = prev;
            }

            prev = item;
            item = hosted.Host;
        }

        if (found != null) // Found an item to remove...
        {
            if (parent is null) // Found last one in tree...
            {
                removed = found.Clone();
                if (removed is DbTokenHosted rtemp) rtemp.ChangeHost(arg);

                var host = ((DbTokenHosted)found).Host;
                return host;
            }
            else // Found intermediate one...
            {
                removed = found.Clone();
                if (removed is DbTokenHosted rtemp) rtemp.ChangeHost(arg);

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
    /// Removes from the given source tree of tokens all the ocurrences of the ones that match
    /// the given predicate. If found, returns the modified tree of tokens and the collection of
    /// removed ones in the out parameter. Otherwise, returns the original tree and sets the out
    /// parameter to an empty collection.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public static DbToken RemoveAll(
        this DbToken source, Predicate<DbToken> predicate, out List<DbToken> list)
    {
        source.ThrowWhenNull();
        predicate.ThrowWhenNull();

        DbToken temp = source;
        list = [];

        while (true)
        {
            var hosted = temp as DbTokenHosted;

            if (hosted is null) break;
            else
            {
                temp = hosted.RemoveLast(predicate, out var removed);
                if (removed == null) break;
                list.Insert(0, removed);
            }
        }

        return temp;
    }
}