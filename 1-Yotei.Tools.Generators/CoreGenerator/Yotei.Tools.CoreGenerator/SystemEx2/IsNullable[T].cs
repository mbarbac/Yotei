namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Used to specify that the wrapped type shall be treated as a nullable one when either
/// the compiler prevents nullable annotations, or when these annotations are not persisted
/// in metadata (for instance, when used with reference types).
/// </summary>
public partial class IsNullable<T> { }