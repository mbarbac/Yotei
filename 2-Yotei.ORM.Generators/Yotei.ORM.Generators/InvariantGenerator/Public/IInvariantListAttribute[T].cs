namespace Yotei.ORM.Generators.Invariant;

// ========================================================
/// <summary>
/// Decorates interfaces for 'IInvariantList[T]' will be implemented.
/// <br/> Clone functionality is automatically handled.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IInvariantListAttribute<T> : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public IInvariantListAttribute() { }

    // ----------------------------------------------------

    /// <summary>
    /// The type of the elements in the collection.
    /// </summary>
    public Type TType => typeof(T);

    /// <summary>
    /// The return type of the generated methods.
    /// <br/> The default value of this setting is the type of the decorated host.
    /// </summary>
    public Type ReturnType { get; set; } = null!;
}