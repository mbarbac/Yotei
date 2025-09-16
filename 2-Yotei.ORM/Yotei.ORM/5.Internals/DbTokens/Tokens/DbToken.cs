namespace Yotei.ORM.Internals;

// ========================================================
public static class DbToken
{
    /// <summary>
    /// Returns a validated token name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string ValidateTokenName(string name)
    {
        name = name.NotNullNotEmpty();

        if (!ValidFirstChar(name[0])) throw new ArgumentException(
            "Name contains invalid first character.")
            .WithData(name);

        if (name.Any(x => !ValidOtherChar(x))) throw new ArgumentException(
            "Name contains invalid character(s).")
            .WithData(name);

        return name;

        static bool ValidFirstChar(char c) =>
            VALID_FIRST.Contains(c) ||
            (c >= '0' && c <= '9') ||
            (c >= 'A' && c <= 'Z') ||
            (c >= 'a' && c <= 'z');

        static bool ValidOtherChar(char c) => ValidFirstChar(c);
    }

    static readonly string VALID_FIRST = "_$@#";

    // ----------------------------------------------------

    /// <summary>
    /// Returns an immutable collection based upon the given arbitrary one.
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="allowEmpty"></param>
    /// <returns></returns>
    public static DbTokenChain ToArguments(IEnumerable<IDbToken> tokens, bool allowEmpty)
    {
        tokens.ThrowWhenNull();

        if (tokens is not DbTokenChain chain) chain = new DbTokenChain(tokens);

        for (int i = 0; i < chain.Count; i++)
            if (chain[i] is null) throw new ArgumentException(
                "Collection of tokens carries null elements.").WithData(chain);

        if (!allowEmpty && chain.Count == 0) throw new ArgumentException(
            "Collection of tokens cannot be an empty one.");

        return chain;
    }

    /// <summary>
    /// Returns an immutable collection based upon the given arbitrary one.
    /// </summary>
    /// <param name="types"></param>
    /// <param name="allowEmpty"></param>
    /// <returns></returns>
    public static ImmutableArray<Type> ToTypeArguments(IEnumerable<Type> types, bool allowEmpty)
    {
        var list = types.ThrowWhenNull().ToImmutableArray();

        for (int i = 0; i < list.Length; i++)
            if (list[i] is null) throw new ArgumentException(
                "Collection of type arguments carries null elements.").WithData(list);

        if (!allowEmpty && list.Length == 0) throw new EmptyException(
            "Collection of type arguments cannot be empty.");

        return list;
    }
}