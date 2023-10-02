namespace Yotei.Tools.Generators;

// ========================================================
internal partial class TypeBuilder
{
    SymbolEqualityComparer SymbolComparer = SymbolEqualityComparer.Default;

    string Receiver = default!;
    BuilderSpecs Specs = default!;
    EnforcedMember? EnforcedMember = null;
    bool EnforcedUsed = false;
    ImmutableArray<IMethodSymbol> TypeMethods = default!;
    ImmutableArray<IPropertySymbol> TypeProperties = default!;
    ImmutableArray<IFieldSymbol> TypeFields = default!;

    /// <summary>
    /// Returns the code that assigns to the 'rceiver' variable a new instance of the associated
    /// type, taking into consideration the given specifications if the optional enforced member.
    /// If the code cannot be produced, return null.
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="specs"></param>
    /// <param name="enforced"></param>
    /// <returns></returns>
    public string? GetCode(string receiver, string? specs = null, EnforcedMember? enforced = null)
    {
        Receiver = receiver.NotNullNotEmpty(nameof(receiver));
        Specs = new BuilderSpecs(specs);
        EnforcedMember = enforced;
        

        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Captures the appropriate builders for the given name specifications, null to only use
    /// the type constructors, or the name of a regular method that returns a type compatible
    /// with the type associated with this instance.
    /// </summary>
    /// <returns></returns>
    ImmutableArray<IMethodSymbol> CaptureBuilders()
    {
    }
}