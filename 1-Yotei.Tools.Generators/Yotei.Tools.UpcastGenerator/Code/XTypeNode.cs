using System.ComponentModel;

namespace Yotei.Tools.UpcastGenerator;

// ========================================================
/// <inheritdoc cref="TypeNode"/>
/// <param name="parent"></param>
/// <param name="candidate"></param>
internal class XTypeNode(
    INode parent, TypeCandidate candidate) : TypeNodeEx(parent, candidate)
{
    // The collection of upcasted types from the attributes that decorates this syntax node.
    readonly List<UpcastType> UpcastedTypes = [];

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override bool OnValidate(SourceProductionContext context)
    {
        UpcastedTypes.Clear();

        // We need an inheritance chain to implement...
        if (Syntax.BaseList != null)
        {
            var nodes = Syntax.BaseList!.ChildNodes().ToList();
            var ats = Syntax.AttributeLists.GetAttributes();

            // We process all attributes in order, accepting any to override others...
            foreach (var at in ats)
            {
                List<int> indexes = [];
                bool change = false;
                bool prevent = false;

                // Using the attribute arguments...
                if (at.ArgumentList != null)
                {
                    for (int i = 0; i < at.ArgumentList.Arguments.Count; i++)
                    {
                        var arg = at.ArgumentList.Arguments[i];

                        // Named arguments...
                        if (arg.NameEquals != null)
                        {
                            if (arg.NameEquals.Name.ShortName() == "ChangeProperties")
                            {
                                var expr = (LiteralExpressionSyntax)arg.Expression;
                                change = (bool)expr.Token.Value!;
                                continue;
                            }
                            if (arg.NameEquals.Name.ShortName() == "PreventVirtual")
                            {
                                var expr = (LiteralExpressionSyntax)arg.Expression;
                                prevent = (bool)expr.Token.Value!;
                                continue;
                            }
                        }

                        // First argument...
                        if (i == 0)
                        {
                            if (arg.Expression is LiteralExpressionSyntax unique)
                            {
                                var temp = (int)unique.Token.Value!;
                                if (!indexes.Contains(temp)) indexes.Add(temp);
                                continue;
                            }

                            if (arg.Expression is CollectionExpressionSyntax multiple)
                            {
                                foreach (var entry in multiple.Elements.Cast<ExpressionElementSyntax>())
                                {
                                    var expr = (LiteralExpressionSyntax)entry.Expression;
                                    var temp = (int)expr.Token.Value!;
                                    if (!indexes.Contains(temp)) indexes.Add(temp);
                                    continue;
                                }
                            }
                        }
                    }
                }

                // We may have no index specification in the attribute...
                if (indexes.Count == 0)
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        var item = Create(nodes, i, change, prevent);
                        Update(UpcastedTypes, item);
                    }
                }

                // Or using the captured indexes from the attribute...
                else
                {
                    foreach (var index in indexes)
                    {
                        var item = Create(nodes, index, change, prevent);
                        Update(UpcastedTypes, item);
                    }
                }
            }
        }

        // No inherited types found...
        if (UpcastedTypes.Count == 0)
        {
            context.NoInheritedElements(Syntax);
            return false;
        }
        return true;
    }

    // Creates a new inherited type instance...
    UpcastType Create(List<SyntaxNode> nodes, int index, bool change, bool prevent)
    {
        var syntax = (SimpleNameSyntax)((SimpleBaseTypeSyntax)nodes[index]).Type;
        var symbol = (INamedTypeSymbol)SemanticModel.GetSymbolInfo(syntax).Symbol!;
        return new UpcastType(syntax, symbol, change, prevent);
    }

    // Finds the index of the symbol in the captured inherited types...
    static int IndexOf(List<UpcastType> items, INamedTypeSymbol symbol)
    {
        for (int i = 0; i < items.Count; i++)
            if (SymbolEqualityComparer.Default.Equals(items[i].Symbol, symbol)) return i;

        return -1;
    }

    // Updates the inherited type...
    static void Update(List<UpcastType> items, UpcastType item)
    {
        var index = IndexOf(items, item.Symbol);

        if (index < 0) items.Add(item);
        else items[index] = item;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void OnEmit(SourceProductionContext context, CodeBuilder cb)
    {
        var hostType = Symbol.EasyName(new EasyNameOptions(useGenerics: true));
        var comparer = SymbolEqualityComparer.Default;
        var prev = false;

        foreach (var upcast in UpcastedTypes)
        {
            var upcastType = upcast.Symbol.EasyName(new EasyNameOptions(useGenerics: true));

            if (upcast.ChangeProperties)
            {
                var upcastProperties = upcast.Symbol.GetMembers().OfType<IPropertySymbol>()
                .Where(x => comparer.Equals(upcast.Symbol, x.Type))
                .ToDebugArray();

                foreach (var upcastProperty in upcastProperties) TryProperty(upcastProperty);
            }

            var upcastMethods = upcast.Symbol.GetMembers().OfType<IMethodSymbol>()
                .Where(x => comparer.Equals(upcast.Symbol, x.ReturnType))
                .ToDebugArray();

            foreach (var upcastMethod in upcastMethods) TryMethod(upcastMethod);

            // --------------------------------------------

            // Invoked to emit the given upcasted property...
            void TryProperty(IPropertySymbol upcastProperty)
            {
                var props = Symbol.GetMembers().OfType<IPropertySymbol>()
                    .Where(x => Same(upcastProperty, x))
                    .ToDebugArray();

                if (props.Any()) return; // Already implemented!

                if (prev) cb.AppendLine();
                prev = true;

                var header = upcastProperty.EasyName(new EasyNameOptions(useMemberArguments: true));
                var addnul = upcastProperty.NullableAnnotation == NullableAnnotation.Annotated ? "?" : "";
                var isindexer = upcastProperty.IsIndexer;
                var name = "";
                if (isindexer)
                {
                    var sb = new StringBuilder();
                    sb.Append('['); for (int i = 0; i < upcastProperty.Parameters.Length; i++)
                    {
                        if (i != 0) sb.Append(", ");
                        sb.Append(upcastProperty.Parameters[i].Name);
                    }
                    sb.Append(']');
                    name = sb.ToString();
                }
                else name = upcastProperty.Name;
                var dot = isindexer ? "" : ".";

                var getter = upcastProperty.GetMethod != null;
                var setter = upcastProperty.SetMethod != null && !upcastProperty.SetMethod.IsInitOnly;
                var initter = upcastProperty.SetMethod != null && upcastProperty.SetMethod.IsInitOnly;

                if (upcast.Symbol.IsInterface()) // Inheriting from an interface...
                {
                    if (Symbol.IsInterface())
                    {
                        cb.Append($"new {hostType}{addnul} {header} {{");
                        if (getter) cb.Append(" get;");
                        if (setter) cb.Append(" set;");
                        if (initter) cb.Append(" init;");
                        cb.AppendLine(" }");
                    }
                    else
                    {
                        cb.AppendLine($"{upcastType}{addnul} {upcastType}{dot}{name}");
                        cb.AppendLine("{");
                        cb.IndentLevel++;

                        if (getter) cb.AppendLine($"get => {(isindexer ? "this" : "")}{name};");
                        if (setter) cb.AppendLine($"set => {(isindexer ? "this" : "")} = ({hostType}{addnul})value;");
                        if (initter) cb.AppendLine($"init => {(isindexer ? "this" : "")} = ({hostType}{addnul})value;");

                        cb.IndentLevel--;
                        cb.AppendLine("}");
                    }
                }
                else // Inheriting from a base type...
                {
                    cb.AppendLine($"public new {hostType}{addnul} {header}");
                    cb.AppendLine("{");
                    cb.IndentLevel++;

                    if (getter) cb.AppendLine($"get => ({hostType}{addnul})base{dot}{name};");
                    if (setter) cb.AppendLine($"set => base{dot}{name} = value;");
                    if (initter) cb.AppendLine($"init => base{dot}{name} = value;");

                    cb.IndentLevel--;
                    cb.AppendLine("}");
                }
            }

            // --------------------------------------------

            // Invoked to emit the given upcasted property...
            void TryMethod(IMethodSymbol upcastMethod)
            {
            }
        }
    }

    // Determines if the property in the upcasted type is the same as the one in the host one.
    bool Same(IPropertySymbol upcastProp, IPropertySymbol hostProp)
    {
        if (upcastProp.Name != hostProp.Name) return false;
        if (upcastProp.Parameters.Length != hostProp.Parameters.Length) return false;

        for (int i = 0; i < upcastProp.Parameters.Length; i++)
        {
            var upcastType = upcastProp.Type;
            var hostType = hostProp.Type;
            if (!hostType.IsAssignableTo(upcastType)) return false;
        }
        return true;
    }
}