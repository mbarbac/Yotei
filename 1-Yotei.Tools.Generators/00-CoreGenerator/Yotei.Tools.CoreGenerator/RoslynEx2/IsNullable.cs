namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Used to decorate types for which nullability information shall be persisted.
/// <para>
/// Nullable annotations on reference types is just syntactic sugar used by the compiler for its
/// nullability analysis, but in general either is not persisted in metadata or custom attributes,
/// or the compiler just prevents using them. By contrast, the compiler translates annotated value
/// types into <see cref="Nullable{T}"/> instances.
/// </para><para>
/// The <see cref="IsNullable{T}"/> and <see cref="IsNullableAttribute"/> types are workarounds
/// that can be used when there is the need to specify nullability but the compiler prevents using
/// nullable annotations, or when they are not persisted.
/// </para>
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// Generators that need to use this workarounds shall can make analogous types public in their own
/// namespaces to prevent type collisions. If you're lazy, you can inherit this documentation.
/// </remarks>
internal class IsNullable<T> { }

// ========================================================
/// <summary>
/// <inheritdoc cref="IsNullable{T}"/>
/// </summary>
[AttributeUsage(AttributeTargets.All)]
internal class IsNullableAttribute : Attribute { }