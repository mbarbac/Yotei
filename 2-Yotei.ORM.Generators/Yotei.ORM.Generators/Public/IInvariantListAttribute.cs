#nullable enable

namespace Yotei.ORM.InvariantGenerator;

// ========================================================
/// <summary>
/// Decorates interfaces for which either 'IInvariantList[T]' or 'IInvariantList[K, T]' will be
/// used as a base one, and its methods (including 'Clone') redeclared.
/// </summary>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IInvariantListAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance to implement 'IInvariantList[T]'.
    /// </summary>
    /// <param name="ttype"></param>
    public IInvariantListAttribute(Type ttype) => TType = ttype.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance to implement 'IInvariantList[K, T]'.
    /// </summary>
    /// <param name="ktype"></param>
    /// <param name="ttype"></param>
    public IInvariantListAttribute(Type ktype, Type ttype)
    {
        KType = ktype.ThrowWhenNull();
        TType = ttype.ThrowWhenNull();
    }

    /// <summary>
    /// If not <see langword="null"/>, then the type of the keys of the 'IInvariantList[K, T]'
    /// decorated collection. If null, not needed for the 'IInvariantList[T]' decorated one.
    /// </summary>
    public Type? KType { get; }

    /// <summary>
    /// The type of the elements of the decorated collection.
    /// </summary>
    public Type TType { get; }

    /// <summary>
    /// If not <see langword="null"/>, then specifies the return type of the generated methods.
    /// Otherwise, the type of the decorated host will be used by default.
    /// <br/> Derived types must maintain base compatibility.
    /// </summary>
    public Type? ReturnType { get; set; } = null;

    /// <summary>
    /// If <see langword="true"/>, emits the appropiate version of the inherited 'Clone' method.
    /// <br/> The default value of this property is <see langword="false"/>, which means that the
    /// application code takes care of this.
    /// </summary>
    public bool EmitClone { get; set; }
}