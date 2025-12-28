namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class AttributeExtensions
{
    /// <summary>
    /// Returns a flattened enumeration of the attributes that decorates each of the syntax
    /// elements of the given list syntax one. No attempts are made to prevent duplication of
    /// attributes.
    /// </summary>
    /// <param name="sources"></param>
    /// <returns></returns>
    public static IEnumerable<AttributeSyntax> GetAttributes(
        this SyntaxList<AttributeListSyntax> sources)
    {
        return sources.SelectMany(static x => x.Attributes);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the collection of attributes that decorates the given symbol, provided that their
    /// class type match the given one.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<AttributeData> GetAttributes(this ISymbol symbol, Type type)
    {
        foreach (var at in symbol.GetAttributes())
            if (at.AttributeClass is not null && at.AttributeClass.Match(type)) yield return at;
    }

    /// <summary>
    /// Determines if the given symbol is decorated with any attribute whose class type match the
    /// given one.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool HasAttributes(
        this ISymbol symbol, Type type) => symbol.GetAttributes(type).Any();

    // ----------------------------------------------------

    /// <summary>
    /// Returns the attributes that decorates the given symbol, obtaining through the attribute
    /// syntax references found decorating the given syntax, which must be the one associated to
    /// that symbol.
    /// <para>
    /// For whatever reasons, <see cref="ISymbol.GetAttributes()"/> happens not to return all the
    /// attributes when they are declared in different places, for instance with partial types.
    /// So, this method finds the attribute syntax elements applied to the given syntax finding
    /// the corresponding references.
    /// </para>
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="syntax"></param>
    /// <returns></returns>
    public static IEnumerable<AttributeData> GetAttributes(
        this ISymbol symbol,
        MemberDeclarationSyntax syntax)
    {
        var atsyntaxes = syntax.AttributeLists.SelectMany(static x => x.Attributes);
        foreach (var atsyntax in atsyntaxes)
        {
            var atd = symbol.GetAttributes().FirstOrDefault(
                x => x.ApplicationSyntaxReference?.GetSyntax() == atsyntax);

            if (atd is not null) yield return atd;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this attribute data instance is equal to the other given one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool EqualTo(this AttributeData source, AttributeData target)
    {
        source.ThrowWhenNull();
        target.ThrowWhenNull();

        // Compare attribute class...
        if (!SymbolEqualityComparer.Default.Equals(source.AttributeClass, target.AttributeClass))
            return false;

        // Compare constructor arguments...
        if (source.ConstructorArguments.Length != target.ConstructorArguments.Length)
            return false;

        for (int i = 0; i < source.ConstructorArguments.Length; i++)
            if (!AreEqual(source.ConstructorArguments[i], source.ConstructorArguments[i]))
                return false;

        // Compare named arguments, order doesn't matter...
        if (source.NamedArguments.Length != source.NamedArguments.Length) return false;

        foreach (var sarg in source.NamedArguments)
        {
            var targ = target.NamedArguments.FirstOrDefault(x => x.Key == sarg.Key);
            if (targ.Key is null || !AreEqual(sarg.Value, targ.Value)) return false;
        }

        // Finishing...
        return true;

        /// <summary>
        /// Invoked to determine if the two given typed constants are the same.
        /// </summary>
        static bool AreEqual(TypedConstant source, TypedConstant target)
        {
            if (source.Kind != target.Kind) return false;

            if (source.Kind == TypedConstantKind.Array)
                return source.Values.SequenceEqual(target.Values, (x, y) => AreEqual(x, y));

            return Equals(source.Value, target.Value);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to obtain the typed constant carried by the named argument whose name is given,
    /// from the given attribute. Returns '<c>true</c> if the named argument is found, and the
    /// type constant in the out argument, or '<c>false</c>' otherwise.
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="name"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static bool GetNamedArgument(
        this AttributeData attribute, string name, [NotNullWhen(true)] out TypedConstant? item)
    {
        attribute.ThrowWhenNull();
        name = name.NotNullNotEmpty(true);

        foreach (var temp in attribute.NamedArguments)
        {
            if (temp.Key == name)
            {
                item = temp.Value;
                return true;
            }
        }

        item = null;
        return false;
    }
}