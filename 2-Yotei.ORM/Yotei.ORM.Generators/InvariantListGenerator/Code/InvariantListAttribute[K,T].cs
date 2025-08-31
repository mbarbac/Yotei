#pragma warning disable CS8618

namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates class for which 'IInvariantList{K,T}' is to be used as their base one.
/// <br/> Clone capability is automatically implemented, hence why the decorated host need a copy
/// constructor.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class InvariantListAttribute<K, T> : Attribute
{
    /// <summary>
    /// Initializes a new instance to implement the 'IInvariantList{K,T}' interface.
    /// </summary>
    public InvariantListAttribute() { }

    // ----------------------------------------------------

    /// <summary>
    /// The type of the keys of the implemented invariant base class.
    /// </summary>
    public Type KType => typeof(K);

    /// <summary>
    /// The type of the elements of the implemented invariant base class.
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