namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Used to identify the wrapped type as a nullable ones for contexts where this is not allowed
/// by the compiler or where this information is not persisted as metadata.
/// <para>
/// Nullable annotations for reference types are just syntactic sugar used by the compiler, but
/// not persisted as metadata. In addition, nullable annotations are not allowed by the compiler
/// in some constructions. This '<see cref="IsNullable{T}"/>' type can be used as a workaround
/// when using it has not harmful side effects.
/// </para>
/// </summary>
/// <typeparam name="T"></typeparam>
public class IsNullable<T> { }