#pragma warning disable CS8618

namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates classes for which either 'InvariantList{K,T}' or 'InvariantList{T}' is to be used
/// as their base one.
/// <br/> Decorated types need to provide an appropriate clone capability.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class InvariantListAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance to implement the 'IInvariantList{T}' interface.
    /// <br/> Use 'IsNullable{type}' if the type is a nullable one.
    /// </summary>
    /// <param name="ttype"></param>
    public InvariantListAttribute(Type ttype)
    {
        KType = null;
        TType = ttype.ThrowWhenNull();
    }

    /// <summary>
    /// Initializes a new instance to implement the 'IInvariantList{K,T}' interface.
    /// <br/> Use 'IsNullable{type}' if any type is a nullable one.
    /// </summary>
    /// <param name="ktype"></param>
    /// <param name="ttype"></param>
    public InvariantListAttribute(Type ktype, Type ttype)
    {
        KType = ktype.ThrowWhenNull();
        TType = ttype.ThrowWhenNull();
    }

    // ----------------------------------------------------

    /// <summary>
    /// If used then its value specifies the return type of the generated methods. If not used,
    /// then the return type is the host type itself.
    /// </summary>
    public Type ReturnType { get; set; }

    /// <summary>
    /// If not null the type of the keys of the implemented invariant list interface. If null,
    /// then it is not used.
    /// </summary>
    public Type? KType { get; }

    /// <summary>
    /// The type of the elements of the implemented invariant list interface.
    /// </summary>
    public Type TType { get; }
}