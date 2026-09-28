namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the ability of converting instances from their source to a target one.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public interface IValueConverter
{
    /// <summary>
    /// The source type to convert values from.
    /// </summary>
    Type SourceType { get; }

    /// <summary>
    /// The target type to convert values to.
    /// </summary>
    Type TargetType { get; }

    /// <summary>
    /// Tries to convert the given value from the source type to the target one, using the given
    /// locale if provided and needed.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="locale"></param>
    /// <returns></returns>
    object? Convert(object? value, Locale? locale = null);
}