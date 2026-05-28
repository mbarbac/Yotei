#nullable enable

namespace Yotei.ORM.InvariantGenerator;

// ========================================================
/// <summary>
/// Decorates classes for which 'InvariantBag[T]' will be used as their base one, and its methods
/// (including 'Clone') reimplemented.
/// <br/> Regular types (not interface or abstract ones) must implement a copy constructor.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class InvariantBagAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="type"></param>
    [SuppressMessage("", "IDE0290")]
    public InvariantBagAttribute(Type type) => TType = type.ThrowWhenNull();

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