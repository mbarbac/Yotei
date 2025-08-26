namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Used to specify that a given type shall be a nullable one.
/// <br/> This class is typically used with types used in attributes, where the '?' annotation
/// or the <see cref="Nullable{T}"/> one are not accepted by the compiler.
/// </summary>
/// <typeparam name="T"></typeparam>
public class IsNullable<T> { }