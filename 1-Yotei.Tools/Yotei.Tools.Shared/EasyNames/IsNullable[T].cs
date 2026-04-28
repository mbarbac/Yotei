namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Used to decorate types for which nullability information shall be persisted, typically used
/// with reference or generic types. The <see cref="IsNullable{T}"/> and the
/// <see cref="IsNullableAttribute"/> types can be used as workarounds when there is the need to
/// persist nullability information on an arbitrary type, and the drawbacks are acceptable.
/// <para>
/// Nullable annotations on reference types are just syntactic sugar, used by the compiler but not
/// persisted in metadata or custom attributes. In addition, the compiler prevents using them in
/// some constructions.
/// <br/> Generic 'T'-alike types are treated, for the purposes of nullability persistance, as if
/// they were reference types.
/// <br/> By contrast, the compiler translates annotated value types into instances of the 
/// <see cref="Nullable{T}"/> type.
/// </para>
/// <para>
/// Note that you are responsible to use these types in allowed contexts only. For instance, the
/// 'EasyName' family of functions will not intercept usages not allowed by the compiler (for
/// instance, some usages with reference types).
/// </para>
/// </summary>
/// <typeparam name="T"></typeparam>
public class IsNullable<T>
{ }