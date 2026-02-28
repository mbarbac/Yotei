namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class EasyNameExtensions
{
    /// <summary>
    /// Ensures that the hide type name setting has the given value.
    /// Returns the original instance if so, or a new one otherwise.
    /// </summary>
    static EasyNameOptions WithHideTypeName(this EasyNameOptions options, bool value) =>
        options.HideTypeName == value
        ? options
        : options with { HideTypeName = true };

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the type is a generic-alike one.
    /// </summary>
    static bool IsGenericAlike(this Type source) =>
        (!source.IsGenericType && source.FullName == null) ||
        source.IsGenericParameter /*||
        source.IsGenericTypeParameter ||
        source.IsGenericMethodParameter*/;

    /// <summary>
    /// Determines if the type is a nullable wrapper, or not.
    /// </summary>
    static bool IsNullableWrapper(this Type source)
    {
        if (source.GetGenericArguments().Length != 1) return false;
        if (source.Name.StartsWith("Nullable`1")) return true;
        if (source.Name.StartsWith("IsNullable`1")) return true;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines nullability by using the nullability API.
    /// </summary>
    static bool ByNullabilityApi(this ParameterInfo source) => false;
    /*{
        var nic = new NullabilityInfoContext();
        var info = nic.Create(source);
        return
            info.ReadState == NullabilityState.Nullable ||
            info.WriteState == NullabilityState.Nullable;
    }*/

    /// <summary>
    /// Determines nullability by using the nullability API.
    /// </summary>
    static bool ByNullabilityApi(this PropertyInfo source) => false;
    /*{
        var nic = new NullabilityInfoContext();
        var info = nic.Create(source);
        return
            info.ReadState == NullabilityState.Nullable ||
            info.WriteState == NullabilityState.Nullable;
    }*/

    /// <summary>
    /// Determines nullability by using the nullability API.
    /// </summary>
    static bool ByNullabilityApi(this FieldInfo source) => false;
    /*{
        var nic = new NullabilityInfoContext();
        var info = nic.Create(source);
        return
            info.ReadState == NullabilityState.Nullable ||
            info.WriteState == NullabilityState.Nullable;
    }*/

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given <see cref="NullableAttribute"/> attribute represents a nullable
    /// element, or not.
    /// </summary>
    static bool ByNullableAttribute(this NullableAttribute? at) =>
        at is not null &&
        at.NullableFlags.Length > 0 &&
        at.NullableFlags[0] == 2;

    /// <summary>
    /// Determines nullability by using the <see cref="NullableAttribute"/> attribute.
    /// </summary>
    static bool ByNullableAttribute(this ICustomAttributeProvider source) =>
        source.GetCustomAttributes(typeof(NullableAttribute), false).
        OfType<NullableAttribute>().FirstOrDefault().
        ByNullableAttribute();

    // ----------------------------------------------------

    /// <summary>
    /// Determines nullability by using the <see cref="IsNullableAttribute"/> attribute.
    /// </summary>
    static bool ByIsNullableAttribute(this Type source)
        => source.GetCustomAttributes<IsNullableAttribute>().Any();

    /// <summary>
    /// Determines nullability by using the <see cref="IsNullableAttribute"/> attribute.
    /// </summary>
    static bool ByIsNullableAttribute(this ParameterInfo source)
        => source.GetCustomAttributes<IsNullableAttribute>().Any();

    /// <summary>
    /// Determines nullability by using the <see cref="IsNullableAttribute"/> attribute.
    /// </summary>
    static bool ByIsNullableAttribute(this MethodInfo source)
        => source.GetCustomAttributes<IsNullableAttribute>().Any();

    /// <summary>
    /// Determines nullability by using the <see cref="IsNullableAttribute"/> attribute.
    /// </summary>
    static bool ByIsNullableAttribute(this PropertyInfo source)
        => source.GetCustomAttributes<IsNullableAttribute>().Any();

    /// <summary>
    /// Determines nullability by using the <see cref="IsNullableAttribute"/> attribute.
    /// </summary>
    static bool ByIsNullableAttribute(this FieldInfo source)
        => source.GetCustomAttributes<IsNullableAttribute>().Any();
}