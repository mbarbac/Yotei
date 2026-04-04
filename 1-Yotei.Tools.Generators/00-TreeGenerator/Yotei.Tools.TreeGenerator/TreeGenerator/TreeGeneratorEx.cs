namespace Yotei.Tools.Generators;

// ========================================================
partial class TreeGenerator
{
    public virtual void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Registering post-initialization actions...
        context.RegisterPostInitializationOutput(DispatchInitialize);

        // TODO - Initialize
        return;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to dispatch post-initialization actions.
    /// </summary>
    /// <param name="context"></param>
    void DispatchInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddEmbeddedAttributeDefinition();

        if (EmitNullabilityHelpers) DoEmitNullabilityHelpers(context);
        OnInitialize(context);
    }

    /// <summary>
    /// Invoked to emit the <see cref="IsNullable{T}"/> and the <see cref="IsNullableAttribute"/>
    /// nullabilily helper types types in the namespace of the current generator.
    /// </summary>
    void DoEmitNullabilityHelpers(IncrementalGeneratorPostInitializationContext context)
    {
        var ns = GetType().Namespace!;
        var str = $$"""
            using System;
            namespace {{ns}};

            /// <summary>
            /// Used to decorate types for which nullability information shall be persisted, typically
            /// used with reference or generic types.
            /// <para>
            /// Nullable annotations on reference types are just syntactic sugar, used by the compiler
            /// but not persisted in metadata or custom attributes. In addition, the compiler prevents
            /// using them in some constructions. By contrast, the compiler translates annotated value
            /// types into instances of <see cref="Nullable{T}"/>.
            /// </para>
            /// <para>
            /// The <see cref="IsNullable{T}"/> and <see cref="IsNullableAttribute"/> types can be used
            /// as workarounds when there is the need to persist this nullability information on an
            /// arbitrary type, provided that the drawbacks of using them are acceptable.
            /// </para>
            /// <para>
            /// You are responsible to use it in allowed contexts. For instance, the 'EasyName' family
            /// of functions will not intercept usages not allowed by the compiler, as for instance
            /// some usages with reference types.
            /// </para>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            [Microsoft.CodeAnalysis.Embedded]
            public class IsNullable<T> { }
                
            /// <summary>
            /// <inheritdoc cref="IsNullable{T}"/>
            /// </summary>
            [Microsoft.CodeAnalysis.Embedded]
            [AttributeUsage(AttributeTargets.All)]
            public class IsNullableAttribute : Attribute { }
            """;

        var name = ns + ".IsNullable[T]";
        var parts = name.Split('.').ToList();
        parts.Reverse();
        parts.Add(".g.cs");
        name = string.Join(".", parts);

        context.AddSource(name, str);
    }
}