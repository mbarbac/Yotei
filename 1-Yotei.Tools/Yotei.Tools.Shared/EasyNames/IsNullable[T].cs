#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.CoreGenerator;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
///  Used to decorate types for which nullability information shall be persisted.
/// <para>
/// Nullable annotations on reference types are just syntactic sugar, used by the compiler but not
/// persisted in metadata or custom attributes. In addition, the compiler prevents using them in
/// some constructions. By contrast, the compiler translates annotated value types into instances
/// of <see cref="Nullable{T}"/>.
/// </para>
/// <para>
/// The <see cref="IsNullable{T}"/> and <see cref="IsNullableAttribute"/> types can be used as
/// workarounds when there is the need to persist this nullability information on an arbitrary
/// type, and the drawbacks are acceptable.
/// </para>
/// <para>
/// Note that you are responsible to use it in allowed contexts. For instance, the 'EasyName'
/// family of functions will not intercept usages not allowed by the compiler, as for instance
/// some usages with reference types.
/// </para>
/// </summary>
/// <typeparam name="T"></typeparam>
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
class IsNullable<T>
{ }