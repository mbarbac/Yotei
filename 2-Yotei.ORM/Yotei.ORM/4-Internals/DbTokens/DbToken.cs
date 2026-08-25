using System.ComponentModel.DataAnnotations;

namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Helpers and utilities for <see cref="IDbToken"/> instances.
/// </summary>
public static class DbToken
{
    /// <summary>
    /// Returns a validated token name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string ValidateTokenName(string name)
    {
        name = name.NotNullNotEmpty(trim: true);

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
    /// Returns an immutable collection of tokens from the given arbitrary enumeration.
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="allowEmpty"></param>
    /// <returns></returns>
    public static ImmutableArray<IDbToken> ToArguments(IEnumerable<IDbToken> tokens, bool allowEmpty)
    {
        ArgumentNullException.ThrowIfNull(tokens);

        var chain = tokens is ImmutableArray<IDbToken> temp ? temp : [.. tokens];

        for (int i = 0; i < chain.Length; i++)
            if (chain[i] is null) throw new ArgumentException(
                "Collection of tokens carries null elements.").WithData(chain);

        if (!allowEmpty && chain.Length == 0) throw new ArgumentException(
            "Collection of tokens cannot be an empty one.");

        return chain;
    }

    /// <summary>
    /// Returns an immutable collection of typed from the given arbitrary enumeration.
    /// </summary>
    /// <param name="types"></param>
    /// <param name="allowEmpty"></param>
    /// <returns></returns>
    public static ImmutableArray<Type> ToTypeArguments(IEnumerable<Type> types, bool allowEmpty)
    {
        ArgumentNullException.ThrowIfNull(types);

        var items = types is ImmutableArray<Type> temps ? temps : [.. types];

        for (int i = 0; i < items.Length; i++)
            if (items[i] is null) throw new ArgumentException(
                "Collection of types carries null elements.").WithData(items);

        if (items.Length == 0 &&
            !allowEmpty)
            throw new EmptyException("Collection of type elements cannot be empty.");

        return items;
    }
}