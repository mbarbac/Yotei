namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Used to identify types that shall be treated as nullable ones.
/// For instance, 'IsNullable{T}' is used to mimic 'T?'.
/// </summary>
/// <typeparam name="T"></typeparam>
public class IsNullable<T> { }