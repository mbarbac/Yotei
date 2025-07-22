namespace Yotei.Tools.WithGenerator;

// ========================================================
internal static class WithDiagnostics
{
    /// <summary>
    /// Cannot use <see cref="WithAttribute.ReturnInterface"/> because the return type of the
    /// base method is not an interface itself.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="severity"></param>
    /// <returns></returns>
    public static Diagnostic InvalidReturnInterface(
        ITypeSymbol type,
        DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var id = "Cloneable01";
        var head = "Cannot use 'ReturnInterface'.";
        var desc = $"Cannot use 'ReturnInterface' on '{type.Name}' because " +
            "the return type of the base method is not an interface itself.";

        var location =
            type.Locations.FirstOrDefault() ??
            type.GetSyntaxNodes().FirstOrDefault()?.GetLocation();

        return Diagnostic.Create(new DiagnosticDescriptor(
            id, head, desc, "Yotei",
            severity, isEnabledByDefault: true),
            location);
    }
}