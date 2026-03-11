namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates classes for which 'InvariantBag[T]' will be used as their base one, if not yet
/// specified, and its methods re-implemented.
/// <br/> Regular types (not interface or abstract ones) must implement a copy constructor.
/// <br/> The type of the generic argument becomes the return type of the generated methods.
/// Derived types must maintain base compatibility.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class InvariantBagAttribute<T> : Attribute
{
    /// <summary>
    /// The type of the elements of the decorated collection.
    /// </summary>
    public Type TType => typeof(T);
}