#pragma warning disable CS8618

namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates classes for which 'IInvariantList{T}' is to be used as their base one.
/// <br/> Decorated types need to provide an appropriate clone capability.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class InvariantListAttribute<T> : Attribute
{
    /// <summary>
    /// Initializes a new instance to implement the 'IInvariantList{K,T}' interface.
    /// </summary>
    public InvariantListAttribute() { }

    // ----------------------------------------------------

    /// <summary>
    /// If used then its value specifies the return type of the generated methods. If not used,
    /// then the return type is the host type itself.
    /// </summary>
    public Type ReturnType { get; set; }

    /// <summary>
    /// The type of the elements of the implemented invariant base class.
    /// </summary>
    public Type TType => typeof(T);
}