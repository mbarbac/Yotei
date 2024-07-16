namespace Yotei.Tools.FrozenGenerator;

// =========================================================
/// <summary>
/// Decorates classes for which the 'FrozenList[T]' one will be used as their base type,
/// and its related elements upcasted to the decorated type.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class FrozenListAttribute<K, T> : Attribute
{
    /// <summary>
    /// If not null, the type that will be used to upcast the inherited members.
    /// </summary>
    public Type? OverrideType { get; init; }
}