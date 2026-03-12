namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates classes for which either 'InvariantList[T]' or 'InvariantList[K, T]' will be used
/// as their base one, and its methods (including clone) reimplemented.
/// <br/> The type of the generic arguments becomes the type of the keys and elements of the
/// collection.
/// <br/> Regular types (not interface or abstract ones) must implement a copy constructor.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class InvariantListAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance to implement 'InvariantList[T]'.
    /// </summary>
    /// <param name="ttype"></param>
    public InvariantListAttribute(Type ttype) => TType = ttype.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance to implement 'InvariantList[K, T]'.
    /// </summary>
    /// <param name="ktype"></param>
    /// <param name="ttype"></param>
    public InvariantListAttribute(Type ktype, Type ttype)
    {
        KType = ktype.ThrowWhenNull();
        TType = ttype.ThrowWhenNull();
    }

    /// <summary>
    /// If not <see langword="null"/>, then the type of the keys of the 'InvariantList[K, T]'
    /// decorated collection. If null, not needed for the 'InvariantList[T]' decorated one.
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
}