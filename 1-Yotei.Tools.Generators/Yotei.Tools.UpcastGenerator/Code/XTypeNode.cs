using System.Data;
using System.Xml.Linq;

namespace Yotei.Tools.UpcastGenerator;

// ========================================================
/// <inheritdoc cref="TypeNode"/>
internal class XTypeNode(INode parent, TypeCandidate candidate) : TypeNodeEx(parent, candidate)
{
    readonly ChildList<UpcastType> UpcastTypes = [];
    readonly ChildList<IPropertySymbol> UpcastProperties = [];
    readonly ChildList<IMethodSymbol> UpcastMethods = [];

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override bool OnValidate(SourceProductionContext context)
    {
        // No inheritance list in the given syntax...
        if (Syntax.BaseList == null)
        {
            context.NoInheritanceList(Syntax);
            return false;
        }

        // Processing attributes in order...
        var nodes = Syntax.BaseList.ChildNodes().ToList();
        var ats = Syntax.AttributeLists.GetAttributes();

        foreach (var at in ats)
        {
            List<int> indexes = [];
            bool change = false;
            bool prevent = false;

            // Using the attribute arguments, if any...
            if (at.ArgumentList != null)
            {
                for (int i = 0; i < at.ArgumentList.Arguments.Count; i++)
                {
                    var arg = at.ArgumentList.Arguments[i];

                    // Named arguments...
                    if (arg.NameEquals != null)
                    {
                        if (arg.NameEquals.Name.ShortName() == nameof(UpcastAttr.ChangeProperties))
                        {
                            var expr = (LiteralExpressionSyntax)arg.Expression;
                            change = (bool)expr.Token.Value!;
                            continue;
                        }
                        if (arg.NameEquals.Name.ShortName() == nameof(UpcastAttr.PreventVirtual))
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

            // We may have no index specificactions...
            if (indexes.Count == 0)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    var item = Create(nodes, i, change, prevent);
                    Update(item);
                }
            }

            // Or capturing elements from the indexes...
            else
            {
                foreach (var index in indexes)
                {
                    var item = Create(nodes, index, change, prevent);
                    Update(item);
                }
            }
        }

        // Finishing...
        if (UpcastTypes.Count == 0)
        {
            context.NoInheritedTypes(Syntax);
            return false;
        }
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Creates a new upcast type based upon the syntax node in the inheritance list whose
    /// index is given.
    /// </summary>
    UpcastType Create(List<SyntaxNode> nodes, int index, bool change, bool prevent)
    {
        var syntax = (SimpleNameSyntax)((SimpleBaseTypeSyntax)nodes[index]).Type;
        var symbol = (INamedTypeSymbol)SemanticModel.GetSymbolInfo(syntax).Symbol!;
        return new UpcastType(syntax, symbol, change, prevent);
    }

    /// <summary>
    /// Returns the index of the upcast type whose symbol is given.
    /// </summary>
    int IndexOf(INamedTypeSymbol type)
    {
        for (int i = 0; i < UpcastTypes.Count; i++)
            if (SymbolEqualityComparer.Default.Equals(UpcastTypes[i].Symbol, type)) return i;

        return -1;
    }

    /// <summary>
    /// Adds the given element to the collection, or replaces the existing one.
    /// </summary>
    /// <param name="upcast"></param>
    void Update(UpcastType upcast)
    {
        var index = IndexOf(upcast.Symbol);

        if (index < 0) UpcastTypes.Add(upcast);
        else UpcastTypes[index] = upcast;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override void OnEmit(SourceProductionContext context, CodeBuilder cb)
    {
        var hostType = Symbol.EasyName(new EasyNameOptions(useGenerics: true));
        var num = false;

        foreach (var upcast in UpcastTypes)
        {
            DoProperties(upcast);
            DoMethods(upcast);
        }

        /// <summary>
        /// Emits the upcast properties...
        /// </summary>
        void DoProperties(UpcastType upcast)
        {
            if (!upcast.ChangeProperties) return;

            var items = GetUpcastProperties(upcast.Symbol);
            foreach (var item in items)
            {
                if (item.IsGeneratedCode(out var tool, out _) && tool == "Yotei") continue;
                if (Symbol.GetMembers().OfType<IPropertySymbol>().Any(x => Same(item, x))) continue;

                UpcastProperties.Add(item, raiseDuplicates: false);
                if (num) cb.AppendLine();
                num = true;

                var adnul = item.NullableAnnotation == NullableAnnotation.Annotated ? "?" : "";
                var getter = item.GetMethod != null;
                var setter = item.SetMethod != null && !item.SetMethod.IsInitOnly;
                var initer = item.SetMethod != null && item.SetMethod.IsInitOnly;
                var signature = GetSignature(item);
                var invocation = GetInvocation(item);

                // new HostType Name { ... }
                if (Symbol.IsInterface())
                {
                    var name = item.IsIndexer ? "this" : item.Name;

                    cb.Append($"new {hostType}{adnul} {name}{signature} {{");
                    if (getter) cb.Append(" get;");
                    if (setter) cb.Append(" set;");
                    if (setter) cb.Append(" init;");
                    cb.AppendLine(" }");

                    continue;
                }

                // UpcastType UpcastType.Name { ... }
                if (upcast.Symbol.IsInterface())
                {
                    var upcastType = upcast.Symbol.EasyName(new EasyNameOptions(useGenerics: true));
                    var name = item.IsIndexer ? "this" : item.Name;

                    cb.AppendLine($"{upcastType}{adnul} {upcastType}.{name}{signature}");
                    cb.AppendLine("{");
                    cb.IndentLevel++;
                    if (getter) cb.AppendLine($"get => {name}{invocation};");
                    if (setter) cb.AppendLine($"set => {name}{invocation} = ({hostType}{adnul})value;");
                    cb.IndentLevel--;
                    cb.AppendLine("}");
                }

                // public new HostType Name { ... }
                else
                {
                    var name = item.IsIndexer ? "this" : item.Name;

                    cb.AppendLine($"public new {hostType}{adnul} {name}{signature}");
                    cb.AppendLine("{");
                    cb.IndentLevel++;

                    name = item.IsIndexer ? invocation : $".{item.Name}";
                    if (getter) cb.AppendLine($"get => ({hostType}{adnul})base{name};");
                    if (setter) cb.AppendLine($"set => base{name} = value;");
                    if (initer) cb.AppendLine($"init => base{name} = value;");
                    
                    cb.IndentLevel--;
                    cb.AppendLine("}");
                }
            }
        }

        /// <summary>
        /// Emits the upcast methods...
        /// </summary>
        void DoMethods(UpcastType upcast)
        {
            var items = GetUpcastMethods(upcast.Symbol);
            foreach (var item in items)
            {
                if (!item.CanBeReferencedByName) continue;
                if (item.IsGeneratedCode(out var tool, out _) && tool == "Yotei") continue;
                if (Symbol.GetMembers().OfType<IMethodSymbol>().Any(x => Same(item, x))) continue;

                UpcastMethods.Add(item, raiseDuplicates: false);
                if (num) cb.AppendLine();
                num = true;

                var adnul = item.ReturnNullableAnnotation == NullableAnnotation.Annotated ? "?" : "";
                var signature = GetSignature(item);
                var invocation = GetInvocation(item);

                // new HostType Name(...);
                if (Symbol.IsInterface())
                {
                    cb.AppendLine($"new {hostType}{adnul} {item.Name}{signature};");

                    continue;
                }

                // UpcastType UpcastType.Name(...) => Name(...);
                if (upcast.Symbol.IsInterface())
                {
                    var upcastType = upcast.Symbol.EasyName(new EasyNameOptions(useGenerics: true));

                    cb.Append($"{upcastType}{adnul} {upcastType}.{item.Name}{signature} ");
                    cb.Append($"=> {item.Name}{invocation};");
                    cb.AppendLine();
                }

                // public override HostType Name(...) => base.Name(...);
                else
                {
                    var modifier = upcast.PreventVirtual || (!item.IsAbstract && !item.IsVirtual)
                        ? " new"
                        : " override";

                    cb.Append($"public{modifier} {hostType}{adnul} ");
                    cb.Append($"{item.Name}{signature} => ");
                    cb.Append($"({hostType}{adnul})base.{item.Name}{invocation};");
                    cb.AppendLine();
                }
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the candidate properties of the given upcast type.
    /// </summary>
    /// <param name="upcast"></param>
    /// <returns></returns>
    List<IPropertySymbol> GetUpcastProperties(INamedTypeSymbol upcast)
    {
        var comparer = SymbolEqualityComparer.Default;
        var items = upcast.GetMembers().OfType<IPropertySymbol>()
            .Where(x => comparer.Equals(upcast, x.Type))
            .ToList();

        if (items.Count != 0) return items;

        // It may happen that the type is not yet compiled, but if it was generated, then we
        // can find it and grab its generated elements...

        var hierarchy = this.GetHierarchy();
        var node = hierarchy.Find(x =>
        {
            if (x is not XTypeNode core) return false;

            var score = core.Symbol.ConstructedFrom;
            var scast = upcast.ConstructedFrom;
            if (!comparer.Equals(score, scast)) return false;

            return true;
        });
        return node != null
            ? ((XTypeNode)node).UpcastProperties.AsList()
            : items;
    }

    /// <summary>
    /// Gets the candidate methods of the given upcast type.
    /// </summary>
    /// <param name="upcast"></param>
    /// <returns></returns>
    List<IMethodSymbol> GetUpcastMethods(INamedTypeSymbol upcast)
    {
        var comparer = SymbolEqualityComparer.Default;
        var items = upcast.GetMembers().OfType<IMethodSymbol>()
            .Where(x => comparer.Equals(upcast, x.ReturnType))
            .ToList();

        if (items.Count != 0) return items;

        // It may happen that the type is not yet compiled, but if it was generated, then we
        // can find it and grab its generated elements...

        var hierarchy = this.GetHierarchy();
        var node = hierarchy.Find(x =>
        {
            if (x is not XTypeNode core) return false;

            var score = core.Symbol.ConstructedFrom;
            var scast = upcast.ConstructedFrom;
            if (!comparer.Equals(score, scast)) return false;

            return true;
        });

        return node != null
            ? ((XTypeNode)node).UpcastMethods.AsList()
            : items;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the two given properties shall be considered the same, by comparing the
    /// name and the type of the arguments. We assume that the member type is already validated.
    /// </summary>
    bool Same(IPropertySymbol upcastItem, IPropertySymbol hostItem)
    {
        if (upcastItem.Name != hostItem.Name) return false;

        if (upcastItem.Parameters.Length != hostItem.Parameters.Length) return false;
        for (int i = 0; i < upcastItem.Parameters.Length; i++)
        {
            var upcastType = upcastItem.Parameters[i].Type;
            var hostType = hostItem.Parameters[i].Type;
            if (!hostType.IsAssignableTo(upcastType)) return false;
        }

        return true;
    }

    /// <summary>
    /// Determines if the two given methods shall be considered the same, by comparing the
    /// name and the type of the arguments. We assume that the return type is already validated.
    /// </summary>
    bool Same(IMethodSymbol upcastItem, IMethodSymbol hostItem)
    {
        if (upcastItem.Name != hostItem.Name) return false;

        if (upcastItem.TypeParameters.Length != hostItem.TypeParameters.Length) return false;
        for (int i = 0; i < upcastItem.TypeParameters.Length; i++)
        {
            var upcastType = upcastItem.TypeParameters[i];
            var hostType = hostItem.TypeParameters[i];
            if (!hostType.IsAssignableTo(upcastType)) return false;
        }

        if (upcastItem.Parameters.Length != hostItem.Parameters.Length) return false;
        for (int i = 0; i < upcastItem.Parameters.Length; i++)
        {
            var upcastType = upcastItem.Parameters[i].Type;
            var hostType = hostItem.Parameters[i].Type;
            if (!hostType.IsAssignableTo(upcastType)) return false;
        }

        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the parameters' signature.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    string GetSignature(IPropertySymbol item)
    {
        if (!item.IsIndexer) return "";

        var sb = new StringBuilder();
        sb.Append('[');
        for (int i = 0; i < item.Parameters.Length; i++)
        {
            if (i != 0) sb.Append(", ");

            var arg = item.Parameters[i];
            sb.Append(arg.Type.EasyName(new EasyNameOptions(addNullable: true, useGenerics: true)));
            sb.Append(' ');
            sb.Append(arg.Name);
        }
        sb.Append(']');
        return sb.ToString();
    }

    /// <summary>
    /// Gets the parameters' signature.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    string GetSignature(IMethodSymbol item)
    {
        var sb = new StringBuilder();
        sb.Append('(');
        for (int i = 0; i < item.Parameters.Length; i++)
        {
            if (i != 0) sb.Append(", ");

            var arg = item.Parameters[i];
            sb.Append(arg.Type.EasyName(new EasyNameOptions(addNullable: true, useGenerics: true)));
            sb.Append(' ');
            sb.Append(arg.Name);
        }
        sb.Append(')');
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the parameters' invocation.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    string GetInvocation(IPropertySymbol item)
    {
        if (!item.IsIndexer) return $"";

        var sb = new StringBuilder();
        sb.Append('[');
        for (int i = 0; i < item.Parameters.Length; i++)
        {
            if (i != 0) sb.Append(", ");

            var arg = item.Parameters[i];
            sb.Append(arg.Name);
        }
        sb.Append(']');
        return sb.ToString();
    }

    /// <summary>
    /// Gets the parameters' invocation.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    string GetInvocation(IMethodSymbol item)
    {
        var sb = new StringBuilder();
        sb.Append('(');
        for (int i = 0; i < item.Parameters.Length; i++)
        {
            if (i != 0) sb.Append(", ");

            var arg = item.Parameters[i];
            sb.Append(arg.Name);
        }
        sb.Append(')');
        return sb.ToString();
    }
}