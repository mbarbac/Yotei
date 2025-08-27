namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Used to identify types that shall be treated as nullable ones.
/// <br/> For instance, 'Nullable{T}' or 'T?' cannot be used with attributes.
/// </summary>
/// <typeparam name="T"></typeparam>
public class IsNullable<T> { }