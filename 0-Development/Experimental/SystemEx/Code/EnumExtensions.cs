namespace Experimental.SystemEx;

// ========================================================
public static class EnumExtensions
{
    /// <summary>
    /// Returns a new value with the given flags added to the original one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static T AddFlags<T>(this T value, T flags) where T : Enum
    {
        var type = value.GetType();
        var holder = new ValueHolder(value.GetType(), value);

        if (holder.IsSigned)
        {
            holder.Signed |= Convert.ToInt64(flags);
            return (T)Enum.Parse(type, holder.Signed.ToString());
        }
        else
        {
            holder.USigned |= Convert.ToUInt64(flags);
            return (T)Enum.Parse(type, holder.USigned.ToString());
        }
    }

    /// <summary>
    /// Returns a new value with the given flags removed from the original one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static T RemoveFlags<T>(this T value, T flags) where T : Enum
    {
        var type = value.GetType();
        var holder = new ValueHolder(value.GetType(), value);

        if (holder.IsSigned)
        {
            holder.Signed &= ~Convert.ToInt64(flags);
            return (T)Enum.Parse(type, holder.Signed.ToString());
        }
        else
        {
            holder.USigned &= ~Convert.ToUInt64(flags);
            return (T)Enum.Parse(type, holder.USigned.ToString());
        }
    }

    // ----------------------------------------------------

    internal struct ValueHolder
    {
        internal ulong USigned;
        internal long Signed;
        internal bool IsSigned;

        internal ValueHolder(Type type, object value)
        {
            type.ThrowWhenNull();
            if (!type.IsEnum) throw new ArgumentException(
                "Type should be an Enum one.")
                .WithData(type);

            IsSigned = IsSignedType(type.GetEnumUnderlyingType());
            USigned = IsSigned ? 0 : Convert.ToUInt64(value);
            Signed = IsSigned ? Convert.ToInt64(value) : 0;
        }

        private static readonly Type LongType = typeof(long);
        private static readonly Type IntType = typeof(int);
        private static readonly Type ShortType = typeof(short);
        private static readonly Type SByteType = typeof(sbyte);

        internal static bool IsSignedType(Type type) =>
            type == LongType ||
            type == IntType ||
            type == ShortType ||
            type == SByteType;
    }
}