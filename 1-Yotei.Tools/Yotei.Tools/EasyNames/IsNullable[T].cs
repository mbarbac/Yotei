namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Used to decorate types for which nullability information shall be persisted.
/// <para>
/// Nullable annotations on reference types are just syntactic sugar, used by the compiler but not
/// persisted in metadata or custom attributes. In addition, the compiler prevents using them in
/// some constructions. By contrast, the compiler translates annotated value types into instances
/// of <see cref="Nullable{T}"/>.
/// <br/> The <see cref="IsNullable{T}"/> and <see cref="IsNullableAttribute"/> types can be
/// used as workarounds when there is the need to persist this nullability information on an
/// arbitrary type, and the drawbacks are acceptable.
/// </para>
/// </summary>
/// <typeparam name="T"></typeparam>
public class IsNullable<T> { }