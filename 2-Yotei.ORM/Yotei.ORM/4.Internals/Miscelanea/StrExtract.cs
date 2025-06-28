#pragma warning disable IDE0057

namespace Yotei.ORM.Internals;

// ========================================================
public static class StrExtract
{
    /// <summary>
    /// Extracts the first ocurrence of the given separator from the given source text. If found,
    /// sets the out argument to <c>true</c> and returns the resulting parts. Otherwise, returns
    /// <c>false</c> and returns the original source as the left part, and <c>null</c> as the
    /// right one.
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
    /// Extracts the last ocurrence of the given separator from the given source text. If found,
    /// sets the out argument to <c>true</c> and returns the resulting parts. Otherwise, returns
    /// <c>false</c> and returns the original source as the left part, and <c>null</c> as the
    /// right one.
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

    // ----------------------------------------------------

    /// <summary>
    /// Extracts the first ocurrence of the given separator from the elements of the given source
    /// token, provided they are either regular text ones, or first-level text ones in the source
    /// token as a chain. All other individual token types, or other chain elements, are not taken
    /// into consideration. If found, sets the out argument to <c>true</c> and returns the token
    /// that results from the extraction. Otherwise, returns <c>false</c> and returns the original
    /// one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <param name="sensitive"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public static IStrToken ExtractFirst(
        this IStrToken source,
        string separator, bool sensitive, out bool found)
    {
        source.ThrowWhenNull();
        separator.NotNullNotEmpty(trim: false);

        var chain = source is IStrTokenChain xchain ? xchain : new StrTokenChain([source]);

        for (int i = 0; i < chain.Count; i++)
        {
            if (chain[i] is not IStrTokenText item) continue;

            var (left, right) = item.Payload.ExtractFirst(separator, sensitive, out found);
            if (found)
            {
                var builder = new StrTokenChain.Builder();
                for (int k = 0; k < i; k++) builder.Add(chain[k]);
                builder.Add(new StrTokenText(left));
                builder.Add(new StrTokenText(right!));
                for (int k = i + 1; k < chain.Count; k++) builder.Add(chain[k]);

                var temp = builder.CreateInstance().ReduceText();
                return temp;
            }
            else continue;
        }

        found = false;
        return source;
    }

    /// <summary>
    /// Extracts the last ocurrence of the given separator from the elements of the given source
    /// token, provided they are either regular text ones, or first-level text ones in the source
    /// token as a chain. All other individual token types, or other chain elements, are not taken
    /// into consideration. If found, sets the out argument to <c>true</c> and returns the token
    /// that results from the extraction. Otherwise, returns <c>false</c> and returns the original
    /// one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <param name="sensitive"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public static IStrToken ExtractLast(
        this IStrToken source,
        string separator, bool sensitive, out bool found)
    {
        source.ThrowWhenNull();
        separator.NotNullNotEmpty(trim: false);

        var chain = source is IStrTokenChain xchain ? xchain : new StrTokenChain([source]);

        for (int i = chain.Count - 1; i >= 0; i--)
        {
            if (chain[i] is not IStrTokenText item) continue;

            var (left, right) = item.Payload.ExtractLast(separator, sensitive, out found);
            if (found)
            {
                var builder = new StrTokenChain.Builder();
                for (int k = 0; k < i; k++) builder.Add(chain[k]);
                builder.Add(new StrTokenText(left));
                builder.Add(new StrTokenText(right!));
                for (int k = i + 1; k < chain.Count; k++) builder.Add(chain[k]);

                var temp = builder.CreateInstance().ReduceText();
                return temp;
            }
            else continue;
        }

        found = false;
        return source;
    }
}