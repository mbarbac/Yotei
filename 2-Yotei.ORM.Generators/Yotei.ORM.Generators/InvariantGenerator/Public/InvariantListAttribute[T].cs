namespace Yotei.ORM.Generators.Invariant;

// ========================================================
/// <summary>
/// Decorates types for which 'InvariantList[T]' will be used as their base one. Unless abstract,
/// decorated hosts must implement a copy constructor.
/// <br/> Clone functionality is automatically handled.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class InvariantListAttribute<T> : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public InvariantListAttribute() { }

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