namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates interfaces for which 'IInvariantBag[T]' will be used as a base one, and its methods
/// (including clone) redeclared.
/// <br/> The type of the generic argument becomes the return type of the generated methods.
/// Derived types must maintain base compatibility.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IInvariantBagAttribute<T> : Attribute
{
    /// <summary>
    /// The type of the elements of the decorated collection.
    /// </summary>
    public Type TType => typeof(T);
}