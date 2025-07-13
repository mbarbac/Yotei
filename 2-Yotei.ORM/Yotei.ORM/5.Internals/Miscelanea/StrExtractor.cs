FALTA ISOLATED


#pragma warning disable IDE0042
#pragma warning disable IDE0057

namespace Yotei.ORM.Internals;

// ========================================================
public static class StrExtractor
{
    /// <summary>
    /// Extracts from the given source the first ocurrence of the given separator. If found,
    /// returns the resulting left and right parts and sets the out argumento to <c>true</c>.
    /// Otherwise, sets it to false, and returns the original source in the left part, setting
    /// the right one to null.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <param name="sensitive"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public static (string Left, string? Right) ExtractFirst(
        this string source,
        string separator, bool sensitive, out bool found)
    {
        source.ThrowWhenNull();
        separator.NotNullNotEmpty(trim: false);

        var index = source.IndexOf(separator, sensitive);
        if (index >= 0)
        {
            var left = source.Substring(0, index);
            var right = source.Substring(index + separator.Length);
            found = true;
            return (left, right);
        }

        found = false;
        return (source, null);
    }

    /// <summary>
    /// Extracts from the given source the first ocurrence of the given separator, provided it
    /// happens in a first-level text token. If found, returns the resulting left and right parts
    /// and sets the out argumento to <c>true</c>. Otherwise, sets it to false, and returns the
    /// original source in the left part, setting the right one to null.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <param name="sensitive"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public static (IStrToken Left, IStrToken? Right) ExtractFirst(
        this IStrToken source,
        string separator, bool sensitive, out bool found)
    {
        source.ThrowWhenNull();
        separator.NotNullNotEmpty(trim: false);

        var items = source is StrTokenChain temp ? temp.ToList() : [source];
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            if (item is not StrTokenText text) continue;

            var parts = text.Payload.ExtractFirst(separator, sensitive, out found);
            if (found)
            {
                var left = new StrTokenChain.Builder();
                for (int k = 0; k < i; k++) left.Add(items[k]);
                left.Add(new StrTokenText(parts.Left));

                var right = new StrTokenChain.Builder();
                right.Add(new StrTokenText(parts.Right!));
                for (int k = i + 1; k < items.Count; k++) right.Add(items[k]);

                var xleft = left.CreateInstance().ReduceText();
                var xright = right.CreateInstance().ReduceText();
                return (xleft, xright);
            }
        }

        found = false;
        return (source, null);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Extracts from the given source the last ocurrence of the given separator. If found,
    /// returns the resulting left and right parts and sets the out argumento to <c>true</c>.
    /// Otherwise, sets it to false, and returns the original source in the left part, setting
    /// the right one to null.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <param name="sensitive"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public static (string Left, string? Right) ExtractLast(
        this string source,
        string separator, bool sensitive, out bool found)
    {
        source.ThrowWhenNull();
        separator.NotNullNotEmpty(trim: false);

        var index = source.LastIndexOf(separator, sensitive);
        if (index >= 0)
        {
            var left = source.Substring(0, index);
            var right = source.Substring(index + separator.Length);
            found = true;
            return (left, right);
        }

        found = false;
        return (source, null);
    }

    /// <summary>
    /// Extracts from the given source the last ocurrence of the given separator, provided it
    /// happens in a first-level text token. If found, returns the resulting left and right parts
    /// and sets the out argumento to <c>true</c>. Otherwise, sets it to false, and returns the
    /// original source in the left part, setting the right one to null.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <param name="sensitive"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public static (IStrToken Left, IStrToken? Right) ExtractLast(
        this IStrToken source,
        string separator, bool sensitive, out bool found)
    {
        source.ThrowWhenNull();
        separator.NotNullNotEmpty(trim: false);

        var items = source is StrTokenChain temp ? temp.ToList() : [source];
        for (int i = items.Count - 1; i >= 0; i--)
        {
            var item = items[i];
            if (item is not StrTokenText text) continue;

            var parts = text.Payload.ExtractLast(separator, sensitive, out found);
            if (found)
            {
                var left = new StrTokenChain.Builder();
                for (int k = 0; k < i; k++) left.Add(items[k]);
                left.Add(new StrTokenText(parts.Left));

                var right = new StrTokenChain.Builder();
                right.Add(new StrTokenText(parts.Right!));
                for (int k = i + 1; k < items.Count; k++) right.Add(items[k]);

                var xleft = left.CreateInstance().ReduceText();
                var xright = right.CreateInstance().ReduceText();
                return (xleft, xright);
            }
        }

        found = false;
        return (source, null);
    }
}