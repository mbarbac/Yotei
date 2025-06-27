#pragma warning disable IDE0057
#pragma warning disable IDE0042

namespace Yotei.ORM.Internals;

// ========================================================
public static class StringExtract
{
    /// <summary>
    /// Extracts the left and right parts from the given source using the first ocurrence of the
    /// given separator, if any. Sets the out argument to <c>true</c> if it was found, or to
    /// <c>false</c> otherwise.
    /// <para>If several ocurrences are found then an exception is thrown.</para>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <param name="sensitive"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public static (string Left, string? Right) ExtractLeftRight(
        this string source,
        string separator, bool sensitive, out bool found)
    {
        source.NotNullNotEmpty(trim: false);
        separator.NotNullNotEmpty(trim: false);

        var indexes = source.IndexesOf(separator, sensitive);
        switch (indexes.Count)
        {
            case 0:
                found = false;
                return (source, null);
            
            case 1:
                var index = indexes[0];
                var left = source.Substring(0, index);
                var right = source.Substring(index + separator.Length);
                found = true;
                return (left, right);

            default:
                throw new DuplicateException(
                    "More than one ocurrence of the separator found in the given source.")
                    .WithData(source)
                    .WithData(separator);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Extracts the left and right parts from the given source using the first ocurrence of the
    /// given separator, if any. Ocurrences protected between the engine's terminators, if they
    /// are used, are not taken into consideration. Sets the out argument to <c>true</c> if it
    /// was found, or to <c>false</c> otherwise.
    /// <para>If several ocurrences are found then an exception is thrown.</para>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <param name="sensitive"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public static (string Left, string? Right) ExtractLeftRight(
        this string source,
        string separator, IEngine engine, out bool found)
    {
        source.NotNullNotEmpty(trim: false);
        separator.NotNullNotEmpty(trim: false);
        engine.ThrowWhenNull();

        var sensitive = engine.CaseSensitiveNames;
        if (!engine.UseTerminators) return source.ExtractLeftRight(separator, sensitive, out found);

        var tokenizer = new StrTokenizerWrapped(engine.LeftTerminator, engine.RightTerminator);
        var token = tokenizer.Tokenize(source, reduce: true);
        var chain = token is StrTokenChain xchain ? xchain : new StrTokenChain([token]);

        found = false;
        string left = source;
        string? right = null;

        for (int i = 0; i < chain.Count; i++)
        {
            if (chain[i] is not StrTokenText item) continue;

            var parts = item.Payload.ExtractLeftRight(separator, sensitive, out var temp);
            if (temp)
            {
                if (found) throw new DuplicateException(
                    "Several ocurrences of the given separator are found.")
                    .WithData(source)
                    .WithData(separator);

                found = true;

                var xleft = new StringBuilder();
                for (int k = 0; k < i; k++) xleft.Append(chain[k].ToString());
                xleft.Append(parts.Left);
                left = xleft.ToString();

                var xright = new StringBuilder();
                xright.Append(parts.Right);
                for (int k = i + 1; k < chain.Count; k++) xright.Append(chain[k].ToString());
                right = xright.ToString();
            }
        }

        return (left, right);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Extracts the main an alias parts from a 'main AS alias' source. If found, sets the out
    /// argument to <c>true</c>, or to <c>false</c> otherwise. The default separator can be set
    /// to a different AS-alike value using the last argument.
    /// <para>
    /// If several ocurrences of ' AS ' are found then an exception is thrown
    /// <br/> If not found but source starts with 'AS ' or ends with ' AS' exceptions are thrown.
    /// </para>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sensitive"></param>
    /// <param name="found"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static (string Main, string? Alias) ExtractMainAlias(
        this string source,
        bool sensitive, out bool found, string separator = " AS ")
    {
        if (separator is not " AS " and not " as " and not " As ")
            throw new InvalidOperationException("Invalid separator.").WithData(separator);

        var (main, alias) = source.ExtractLeftRight(separator, sensitive, out found);
        main.NotNullNotEmpty();
        alias?.NotNullNotEmpty();

        separator = separator.Trim();
        var temp = main.Trim();
        if (string.Compare(temp, separator, !sensitive) == 0)
            throw new ArgumentException(
                $"Source resolves into a '{separator}'-alike element.")
                .WithData(source);
        
        return (main, alias);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Extracts the main an alias parts from a 'main AS alias' source. If found, sets the out
    /// argument to <c>true</c>, or to <c>false</c> otherwise. The default separator can be set
    /// to a different AS-alike value using the last argument.
    /// <para>
    /// If several ocurrences of ' AS ' are found then an exception is thrown
    /// <br/> If not found but source starts with 'AS ' or ends with ' AS' exceptions are thrown.
    /// </para>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="engine"></param>
    /// <param name="found"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static (string Main, string? Alias) ExtractMainAlias(
        this string source,
        IEngine engine, out bool found, string separator = " AS ")
    {
        if (separator is not " AS " and not " as " and not " As ")
            throw new InvalidOperationException("Invalid separator.").WithData(separator);

        var (main, alias) = source.ExtractLeftRight(separator, engine, out found);
        main.NotNullNotEmpty();
        alias?.NotNullNotEmpty();

        separator = separator.Trim();
        var temp = main.Trim();
        if (string.Compare(temp, separator, !engine.CaseSensitiveNames) == 0)
            throw new ArgumentException(
                $"Source resolves into a '{separator}'-alike element.")
                .WithData(source);

        return (main, alias);
    }
}