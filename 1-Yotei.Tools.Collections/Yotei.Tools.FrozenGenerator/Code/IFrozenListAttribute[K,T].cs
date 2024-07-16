namespace Yotei.Tools.FrozenGenerator;

// =========================================================
/// <summary>
/// Decorates interfaces for which the 'IFrozenList[K, T]' one will be implemented and its
/// related elements upcasted to the decorated type.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IFrozenListAttribute<K, T> : Attribute { }