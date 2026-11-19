#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Used to decorate types for which nullability information shall be persisted.
/// <br/> C# treats nullability annotations on reference types just as syntantic sugar used by the
/// compiler, not persisted in metadata or custom attributes. In addition, the compiler prevents
/// them to be used in some constructions. By contrast, nullable value types are just instances of
/// the <see cref="Nullable{T}"/> type (which cannot be used with reference ones).
/// <br/> The <see cref="IsNullable{T}"/> and <see cref="IsNullableAttribute"/> types are used as
/// workarounds to persist nullability information when then C# syntax doesn't allow such, and when
/// the drawbacks from using them are acceptable. Generic <c>T</c>-alike types are treated. for the
/// purposes of nullability persistence, as if the were reference types.
/// </summary>
#if NETSTANDARD2_0
internal
#else
public
#endif
sealed class IsNullable<T>
{ }