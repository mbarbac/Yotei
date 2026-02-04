namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Used to wrap types for which nullability information shall be persisted.
/// <para>
/// Nullable annotations on value types are always translated by the compiler into instances of
/// the <see cref="Nullable{T}"/> struct. By contrast, nullable annotations on reference types are
/// just syntactic sugar used by the compiler but, in general, either they are not persisted in
/// metadata or in custom attributes, or they are not allowed in certain contexts (e.g., generic
/// type arguments).
/// <br/> The <see cref="IsNullable{T}"/> and <see cref="IsNullableAttribute"/> types can be used
/// as workarounds when there is the need to persist nullability information for their associated
/// types, or when there is the need to specify it in those not-allowed contexts.
/// </para>
/// </summary>
/// <typeparam name="T"></typeparam>
internal class IsNullable<T> { }