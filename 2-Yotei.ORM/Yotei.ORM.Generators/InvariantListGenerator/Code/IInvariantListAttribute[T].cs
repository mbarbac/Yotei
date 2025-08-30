#pragma warning disable CS8618

namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates interfaces for which 'IInvariantList{T}' is to be implemented.
/// <br/> Clone capability is automatically implemented.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IInvariantListAttribute<T> : Attribute
{
    /// <summary>
    /// Initializes a new instance to implement the 'IInvariantList{K,T}' interface.
    /// </summary>
    public IInvariantListAttribute() { }

    // ----------------------------------------------------

    /// <summary>
    /// The type of the elements of the implemented invariant list interface.
    /// </summary>
    public Type TType => typeof(T);

    /// <summary>
    /// If used then its value specifies the return type of the generated methods. If not used,
    /// then the return type is the host type itself.
    /// </summary>
    public Type ReturnType { get; set; }

    /// <summary>
    /// If used specifies whether the generated methods are virtual-alike ones, or not. If not
    /// used, then the generator tries to generate virtual-alike ones.
    /// </summary>
    public bool VirtualMethod { get; set; }
}