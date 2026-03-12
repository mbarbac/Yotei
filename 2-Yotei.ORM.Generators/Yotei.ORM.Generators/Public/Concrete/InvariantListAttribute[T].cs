namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates classes for which 'InvariantList[T]' will be used as their base one, and its
/// methods (including clone) reimplemented.
/// <br/> Regular types (not interface or abstract ones) must implement a copy constructor.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class InvariantListAttribute<T> : Attribute
{
    /// <summary>
    /// The type of the elements of the decorated collection.
    /// </summary>
    public Type TType => typeof(T);

    /// <summary>
    /// If not <see langword="null"/>, then specifies the return type of the generated methods.
    /// Otherwise, the type of the decorated host will be used by default.
    /// <br/> Derived types must maintain base compatibility.
    /// </summary>
    public Type? ReturnType { get; set; } = null;
}