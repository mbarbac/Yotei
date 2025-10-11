using System.Xml.Schema;

namespace Yotei.Tools;

// ========================================================
public static partial class ObjectExtensions
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> when the given value is null. Otherwise,
    /// returns the given value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    /// Need to place outside extension block for CallerArgumentExpression to work.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowWhenNull<T>(
        [AllowNull] this T value,
        [CallerArgumentExpression(nameof(value))] string? description = null)
    {
        description = description.NullWhenEmpty(true) ?? nameof(description);

        if (value is null) throw new ArgumentNullException(description);
        return value;
    }
}

// ========================================================
public static partial class ObjectExtensions
{
    extension<T>([AllowNull] T value)
    {
        /// <summary>
        /// Determines if this object can be considered equal to the other given one. If both are
        /// null, then this method returns '<c>true</c>'. If only one of them is null, then this
        /// method returns '<c>false</c>'. Otherwise, this method returns the result of invoking
        /// the 'Equals' method on the this instance, with the other one as its parameter.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EqualsEx([AllowNull] T other)
        {
            if (value is null && other is null) return true;
            if (value is null || other is null) return false;
            return value.Equals(other);
        }
    }
}