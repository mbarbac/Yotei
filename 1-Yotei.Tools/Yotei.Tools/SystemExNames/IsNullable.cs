namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Used to decorate types for which nullability information shall be persisted.
/// <br/> Nullable annotations on reference or on generic types are just syntactic sugar used by
/// the compiler but, in most circumstances, not persisted in metadata or in custom attributes, or
/// just prevented by the compiler. By contrast, the compiler translates annotated value types into
/// <see cref="Nullable{T}"/> instances.
/// <br/> The <see cref="IsNullable{T}"/> and <see cref="IsNullableAttribute"/> types can be used
/// as far-from-perfect workarounds when there is the need to specify this nullability information.
/// </summary>
/// <typeparam name="T"></typeparam>
public class IsNullable<T> { }

// ========================================================
/// <summary>
/// <inheritdoc cref="IsNullable{T}"/>
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class IsNullableAttribute : Attribute { }