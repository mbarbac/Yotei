namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates interfaces for which 'IInvariantList[T]' will be used as a base one, and its
/// methods (including clone) redeclared.
/// <br/> The type of the generic argument becomes the type of the elements of the collection.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IInvariantListAttribute<T> : Attribute
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