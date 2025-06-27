#pragma warning disable IDE0042
#pragma warning disable IDE0057

namespace Yotei.ORM.Internals;

// ========================================================
public static class StringExtract
{
    /// <summary>
    /// Extracts the left and right parts of the given source, provided they are separated by the
    /// given separator. If found, sets the out argument to <c>true</c>. Otherwise, it is set to
    /// <c>false</c>, the returned left part is set to the original source, and the right one is
    /// set to <c>null</c>.
    /// <br/> If several ocurrences of the separator are found, then an exception is thrown.
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
                    "More than one ocurrence of the given separator found in the given source.")
                    .WithData(separator)
                    .WithData(source);
        }
    }

    /// <summary>
    /// Extracts the left and right parts of the given source, provided they are separated by the
    /// given separator. If found, sets the out argument to <c>true</c>. Otherwise, it is set to
    /// <c>false</c>, the returned left part is set to the original source, and the right one is
    /// set to <c>null</c>.
    /// <br/> If several ocurrences of the separator are found, then an exception is thrown.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static (string Left, string? Right) ExtractLeftRight(
        this string source,
        string separator, bool sensitive)
    {
        return source.ExtractLeftRight(separator, sensitive, out _);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Extracts the left and right parts of the given source, provided they are separated by the
    /// given separator. If found, sets the out argument to <c>true</c>. Otherwise, it is set to
    /// <c>false</c>, the returned left part is set to the original source, and the right one is
    /// set to <c>null</c>. In any case, source elements protected by the engine terminators, if
    /// used, are not taken into consideration.
    /// <br/> If several ocurrences of the separator are found, then an exception is thrown.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <param name="engine"></param>
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
        var chain = token is StrTokenChain temp ? temp : new StrTokenChain([token]);

        found = false;
        string left = source;
        string? right = null;

        for (int i = 0; i < chain.Count; i++)
        {
            if (chain[i] is not StrTokenText item) continue;

            var parts = item.Payload.ExtractLeftRight(separator, sensitive, out var other);
            if (other)
            {
                if (found) throw new DuplicateException(
                    "Several ocurrences of the given separator found.")
                    .WithData(source)
                    .WithData(separator);

                left = parts.Left;
                right = parts.Right;
                found = true;
            }
        }
        return (left, right);
    }

    /// <summary>
    /// Extracts the left and right parts of the given source, provided they are separated by the
    /// given separator. If found, sets the out argument to <c>true</c>. Otherwise, it is set to
    /// <c>false</c>, the returned left part is set to the original source, and the right one is
    /// set to <c>null</c>. In any case, source elements protected by the engine terminators, if
    /// used, are not taken into consideration.
    /// <br/> If several ocurrences of the separator are found, then an exception is thrown.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <param name="engine"></param>
    /// <returns></returns>
    public static (string Left, string? Right) ExtractLeftRight(
        this string source,
        string separator, IEngine engine)
    {
        return source.ExtractLeftRight(separator, engine, out _);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Extracts the 'main' and 'alias' parts of a source "main AS alias" source string. If found,
    /// sets the out argument to <c>true</c>. Otherwise, it is set to <c>false</c>, the returned
    /// left part is set to the original source, and the right one is set to <c>null</c>.
    /// <br/> If several ocurrences of the separator are found, then an exception is thrown.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sensitive"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public static (string Main, string? Alias) ExtractMainAlias(
        this string source,
        bool sensitive, out bool found)
    {
        source.ThrowWhenNull();

        var comparison = StringComparison.OrdinalIgnoreCase;
        
        if (source.StartsWith("AS ", comparison)) throw new ArgumentException(
            "Source cannot start with 'AS '")
            .WithData(source);

        if (source.EndsWith(" AS", comparison)) throw new ArgumentException(
            "Source cannot end with 'AS '")
            .WithData(source);

        var (main, alias) = source.ExtractLeftRight(" AS ", sensitive, out found);        
        if (main.Trim().Equals("AS", comparison)) throw new ArgumentException(
            "Source cannot just be 'AS'.")
            .WithData(source);

        return (main, alias);
    }

    /// <summary>
    /// Extracts the 'main' and 'alias' parts of a source "main AS alias" source string. If found,
    /// sets the out argument to <c>true</c>. Otherwise, it is set to <c>false</c>, the returned
    /// left part is set to the original source, and the right one is set to <c>null</c>.
    /// <br/> If several ocurrences of the separator are found, then an exception is thrown.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sensitive"></param>
    /// <returns></returns>
    public static (string Main, string? Alias) ExtractMainAlias(
        this string source, bool sensitive)
        => source.ExtractMainAlias(sensitive, out _);

    /// <summary>
    /// Extracts the 'main' and 'alias' parts of a source "main AS alias" source string. If found,
    /// sets the out argument to <c>true</c>. Otherwise, it is set to <c>false</c>, the returned
    /// left part is set to the original source, and the right one is set to <c>null</c>. In any
    /// case, source elements protected by the engine terminators, if used, are not taken into
    /// consideration.
    /// <br/> If several ocurrences of the separator are found, then an exception is thrown.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="engine"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public static (string Main, string? Alias) ExtractMainAlias(
        this string source,
        IEngine engine, out bool found)
    {
        source.ThrowWhenNull();

        var comparison = StringComparison.OrdinalIgnoreCase;

        if (source.StartsWith("AS ", comparison)) throw new ArgumentException(
            "Source cannot start with 'AS '")
            .WithData(source);

        if (source.EndsWith(" AS", comparison)) throw new ArgumentException(
            "Source cannot end with 'AS '")
            .WithData(source);

        var (main, alias) = source.ExtractLeftRight(" AS ", engine, out found);
        if (main.Trim().Equals("AS", comparison)) throw new ArgumentException(
            "Source cannot just be 'AS'.")
            .WithData(source);

        return (main, alias);
    }

    /// <summary>
    /// Extracts the 'main' and 'alias' parts of a source "main AS alias" source string. If found,
    /// sets the out argument to <c>true</c>. Otherwise, it is set to <c>false</c>, the returned
    /// left part is set to the original source, and the right one is set to <c>null</c>. In any
    /// case, source elements protected by the engine terminators, if used, are not taken into
    /// consideration.
    /// <br/> If several ocurrences of the separator are found, then an exception is thrown.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="engine"></param>
    /// <returns></returns>
    public static (string Main, string? Alias) ExtractMainAlias(
        this string source, IEngine engine)
        => source.ExtractMainAlias(engine, out _);
}