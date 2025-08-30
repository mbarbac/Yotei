#pragma warning disable CS8618

namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates interfaces for which 'IInvariantList{K,T}' is to be implemented.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IInvariantListAttribute<K, T> : Attribute
{
    /// <summary>
    /// Initializes a new instance to implement the 'IInvariantList{K,T}' interface.
    /// </summary>
    public IInvariantListAttribute() { }

    // ----------------------------------------------------

    /// <summary>
    /// If used then its value specifies the return type of the generated methods. If not used,
    /// then the return type is the host type itself.
    /// </summary>
    public Type ReturnType { get; set; }

    /// <summary>
    /// The type of the keys of the implemented invariant list interface.
    /// </summary>
    public Type KType => typeof(K);

    /// <summary>
    /// The type of the elements of the implemented invariant list interface.
    /// </summary>
    public Type TType => typeof(T);
}