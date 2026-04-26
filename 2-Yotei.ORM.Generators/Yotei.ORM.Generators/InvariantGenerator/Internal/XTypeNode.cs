using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// <inheritdoc/>
/// </summary>
public partial class XTypeNode : TypeNode
{
    const string IINVARIANTBAG = "IInvariantBag", INVARIANTBAG = "InvariantBag";
    const string IINVARIANTLIST = "IInvariantList", INVARIANTLIST = "InvariantList";
    const string NAMESPACEAPI = "Yotei.ORM.Tools", NAMESPACECODE = "Yotei.ORM.Tools";

    AttributeData Attribute;
    bool IsBag;
    int Arity;
    INamedTypeSymbol KType; string KTypeName; bool KTypeNullable;
    INamedTypeSymbol TType; string TTypeName; bool TTypeNullable;
    string Bracket;
    Type Template;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="symbol"></param>
    [SuppressMessage("", "IDE0290")]
    public XTypeNode(INamedTypeSymbol symbol) : base(symbol)
    {
        Attribute = default!;
        IsBag = default!;
        Arity = default!;
        KType = default!; KTypeName = default!; KTypeNullable = default;
        TType = default!; TTypeName = default!; TTypeNullable = default;
        Bracket = default!;
        Template = default!;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override bool OnValidate(SourceProductionContext context)
    {
        var r = base.OnValidate(context);

        // Host type's kind validations...
        if (Symbol.IsRecord) { TreeError.RecordsNotSupported.Report(Symbol, context); r = false; }
        if (Symbol.TypeKind == TypeKind.Struct) { TreeError.KindNotSupported.Report(Symbol, context); r = false; }

        // Just one attribute allowed...
        if (Attributes.Count == 0) { TreeError.NoAttributes.Report(Symbol, context); r = false; }
        else if (Attributes.Count > 1) { TreeError.TooManyAttributes.Report(Symbol, context); r = false; }
        else Attribute = Attributes[0];

        // Capturing whether the attribute is for a bag or a list...
        var atc = Attribute.AttributeClass!;
        IsBag = atc.Name.StartsWith(IINVARIANTBAG) || atc.Name.StartsWith(INVARIANTBAG);

        // Capturing arity and related settings...
        Arity = atc.Arity;

        var options = EasyTypeOptions.Full with
        { UseSpecialNames = true, NullableStyle = EasyNullableStyle.UseAnnotations  };

        // Attribute is a not-generic one...
        if (Arity == 0)
        {
            var args = Attribute.ConstructorArguments
                .Where(static x => !x.IsNull && x.Kind == TypedConstantKind.Type)
                .Select(static x => (INamedTypeSymbol)x.Value!)
                .ToArray();

            if (args.Length == 1) // One [T] argument (can be a bag or a list)...
            {
                Template = IsBag ? typeof(IBagTemplate<>) : typeof(IListTemplate<>);

                TType = args[0].UnwrapNullable(out TTypeNullable);
                TTypeName = TType.EasyName(options);
                if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

                Bracket = $"<{TTypeName}>";
                Arity = 1;
            }

            else if (args.Length == 2) // Two [K,T] arguments (must be a list)...
            {
                Template = typeof(IListTemplate<,>);
                KType = args[0].UnwrapNullable(out KTypeNullable);
                TType = args[1].UnwrapNullable(out TTypeNullable);

                KTypeName = KType.EasyName(options);
                TTypeName = TType.EasyName(options);
                if (KTypeNullable && !KTypeName.EndsWith('?')) KTypeName += '?';
                if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

                Bracket = $"<{KTypeName}, {TTypeName}>";
                Arity = 2;
            }

            else { TreeError.InvalidAttribute.Report(Symbol, context); r = false; } // Invalid
        }

        // Attribute is a '[T]' one (can be a bag or a list)...
        else if (Arity == 1)
        {
            Template = IsBag ? typeof(IBagTemplate<>) : typeof(IListTemplate<>);

            TType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out TTypeNullable);
            TTypeName = TType.EasyName(options);
            if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

            Bracket = $"<{TTypeName}>";
            Arity = 1;
        }

        // Attribute is a '[K,T]' one (must be a list)...
        else if (Arity == 2)
        {
            Template = typeof(IListTemplate<,>);
            KType = ((INamedTypeSymbol)atc.TypeArguments[0]).UnwrapNullable(out KTypeNullable);
            TType = ((INamedTypeSymbol)atc.TypeArguments[1]).UnwrapNullable(out TTypeNullable);

            KTypeName = KType.EasyName(options);
            TTypeName = TType.EasyName(options);
            if (KTypeNullable && !KTypeName.EndsWith('?')) KTypeName += '?';
            if (TTypeNullable && !TTypeName.EndsWith('?')) TTypeName += '?';

            Bracket = $"<{KTypeName}, {TTypeName}>";
            Arity = 2;
        }

        // Otherwise, invalid arity...
        else { TreeError.InvalidAttribute.Report(Symbol, context); r = false; }

        // Finishing...
        return r;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    protected override string? GetBaseList()
    {
        IEnumerable<INamedTypeSymbol>[] chains =
            [Symbol.IsInterface ? [.. Symbol.AllInterfaces] : [.. Symbol.AllBaseTypes]];

        var found = Finder.Find(chains, out INamedTypeSymbol? value, (type, out value) =>
        {
            if (type.MatchAny([
                typeof(IInvariantBagAttribute), typeof(IInvariantBagAttribute<>),
                typeof(IInvariantListAttribute),
                typeof(IInvariantListAttribute<,>),typeof(IInvariantListAttribute<>),]))
            {
                value = type;
                return true;
            }

            value = null;
            return false;
        });

        if (!found) // Lets add to the base list...
        {
            var name = Attribute.AttributeClass!.Name;
            name = name.RemoveLast("Attribute").ToString();
            name += Bracket;
            return name;
        }

        return null; // If found we need not to add...
    }
}