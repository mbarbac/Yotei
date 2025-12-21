namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static class AttributeExtensions
{
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
}