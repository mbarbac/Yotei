namespace Yotei.Tools.FrozenGenerator;

// =========================================================
/// <summary>
/// Decorates interfaces for which the 'IFrozenList[T]' one will be implemented and its
/// related elements upcasted to the decorated type.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IFrozenListAttribute<T> : Attribute { }