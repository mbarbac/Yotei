namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Used to identify types that shall be treated as nullable ones by generators, because neither
/// 'T?' nor 'Nullable{T}' are accepted by the compiler.
/// <br/> If several generators are imported, each need to define this class in its own namespace.
/// </summary>
/// <typeparam name="T"></typeparam>
public class IsNullable<T> { }