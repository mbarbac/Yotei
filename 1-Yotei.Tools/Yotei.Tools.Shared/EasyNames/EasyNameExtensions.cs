namespace Yotei.Tools;

// ========================================================
public static partial class EasyNameExtensions
{
    private const string ATTRIBUTE = "Attribute";
    private const string READ_ONLY_ATTRIBUTE = "System.Runtime.CompilerServices.IsReadOnlyAttribute";
    private const string SCOPED_ATTRIBUTE = "System.Runtime.CompilerServices.ScopedRefAttribute";
    private const string REQUIRES_LOCATION = "System.Runtime.CompilerServices.RequiresLocationAttribute";
    private const string IN_ATTRIBUTE = "System.Runtime.InteropServices.InAttribute";

    // ----------------------------------------------------

    /// <summary>
    /// If the type refers to an special one, then returns its special name without taking into
    /// consideration if it is an array, a pointer, a by-ref type. or a nullable wrapper. If it
    /// is not an special one, then returns null.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string? ToSpecialName(this Type source)
    {
        if (source.IsByRef) return Core(source.GetElementType() ?? source);
        if (source.IsArray) return Core(source.GetElementType() ?? source);
        if (source.IsNullableWrapper()) return Core(source.GetGenericArguments()[0]);
        return Core(source);

        // Gets the actual special name for the naked source type...
        static string? Core(Type source) => source switch
        {
            Type t when t == typeof(void) => "void",
            Type t when t == typeof(object) => "object",
            Type t when t == typeof(string) => "string",
            Type t when t == typeof(bool) => "bool",
            Type t when t == typeof(char) => "char",
            Type t when t == typeof(byte) => "byte",
            Type t when t == typeof(sbyte) => "sbyte",
            Type t when t == typeof(short) => "short",
            Type t when t == typeof(ushort) => "ushort",
            Type t when t == typeof(int) => "int",
            Type t when t == typeof(uint) => "uint",
            Type t when t == typeof(long) => "long",
            Type t when t == typeof(ulong) => "ulong",
            Type t when t == typeof(float) => "float",
            Type t when t == typeof(double) => "double",
            Type t when t == typeof(decimal) => "decimal",
            _ => null
        };
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given element is decorated with a readonly attribute that is carried
    /// in its metadata.
    /// </summary>
    public static bool HasReadOnlyAttribute(this ICustomAttributeProvider info)
    {
        return info
            .GetCustomAttributes(false)
            .Any(x => x.GetType().FullName == READ_ONLY_ATTRIBUTE);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the type is a generic 'T' one, for EasyName purposes only.
    /// </summary>
    internal static bool IsGenericAlike(this Type source)
    {
        // Types such 'Predicate<T>' are not considered 'T'-alike ones...
        if (source.GetGenericArguments().Length > 0) return false;

        return source.FullName == null ||
            source.IsGenericType ||
            source.IsGenericParameter ||
            source.IsGenericTypeDefinition ||
            source.IsGenericTypeParameter ||
            source.IsGenericMethodParameter;
    }

#if NETSTANDARD2_0
    extension(Type source)
    {
        /// <summary>
        /// Determines if the type is a generic type parameter, or not.
        /// </summary>
        internal bool IsGenericTypeParameter
            => source.IsGenericParameter && source.DeclaringMethod == null;

        /// <summary>
        /// Determines if the type is a generic method parameter, or not.
        /// </summary>
        internal bool IsGenericMethodParameter
            => source.IsGenericParameter && source.DeclaringMethod != null;
    }
#endif

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the type is a CLR nullable one (<see cref="Nullable{T}"/>), or not.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsCoreNullable(this Type source) =>
        source.GetGenericArguments().Length == 1 &&
        source.Name.StartsWith("Nullable`1");

    /// <summary>
    /// Determines if the given type is a nullable wrapper (either a <see cref="Nullable{T}"/> or
    /// a <see cref="IsNullable{T}"/> one), or not.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullableWrapper(this Type source) =>
        source.GetGenericArguments().Length == 1 && (
        source.Name.StartsWith("Nullable`1") ||
        source.Name.StartsWith("IsNullable`1"));

    /// <summary>
    /// Determines is the given given <see cref="NullableAttribute"/> is enabled.
    /// </summary>
    /// NOTE: The attribute is an 'internal sealed' one (not intended for source code usage), so
    /// we cannot mark this method as public because the parameter would be less accessible than
    /// the method itself.
    internal static bool IsNullableAttributeEnabled(this NullableAttribute at) =>
        at.NullableFlags.Length > 0 &&
        at.NullableFlags[0] == 2;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the given type has been annotated as a nullable one, either because its
    /// metadata carries a <see cref="NullableAttribute"/> or a <see cref="IsNullableAttribute"/>
    /// attribute. This method only cares about these metadata attributes, and does not take in
    /// consideration if the type is a nullable wrapper or not. 
    /// <br/>- Reference types may not carry the <see cref="NullableAttribute"/> even if they have
    /// been annotated (hence why the <see cref="IsNullableAnnotated(Type)"/> one exists, as well
    /// as the <see cref="IsNullable{T}"/> wrapper).
    /// <br/>- For consistency reasons, generic T-alike generic types are treated as conventional
    /// reference ones.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullableAnnotated(this Type source)
    {
        // All but T-alike types...
        if (source.FullName != null)
        {
            if (source.GetCustomAttributes(typeof(NullableAttribute), false)
                .Any(x => IsNullableAttributeEnabled((NullableAttribute)x)))
                return true;
        }

        // Finding custom attributes...
        if (source.GetCustomAttributes(typeof(IsNullableAttribute), false).Length != 0)
            return true;

        // Default...
        return false;
    }

    /// <summary>
    /// Determines if the given element has been annotated as a nullable one, either because its
    /// metadata carries a <see cref="NullableAttribute"/> or a <see cref="IsNullableAttribute"/>
    /// attribute.
    /// <br/> Valid elements are: <see cref="Assembly"/>, <see cref="Module"/>, and the family of
    /// <see cref="MemberInfo"/> ones (including Type, MethodBase, PropertyInfo, FieldInfo and
    /// EventInfo).
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullableAnnotated(this ICustomAttributeProvider source)
    {
        if (source.GetCustomAttributes(typeof(NullableAttribute), false)
            .Any(x => IsNullableAttributeEnabled((NullableAttribute)x)))
            return true;

        if (source.GetCustomAttributes(typeof(IsNullableAttribute), false).Length != 0)
            return true;

        return false;
    }

#if NET6_0_OR_GREATER

    /// <summary>
    /// Determines if the given element has been annotated as a nullable one, either because the
    /// nullability API determines so, or because its metadata either carries a
    /// <see cref="NullableAttribute"/> attribute or a <see cref="IsNullableAttribute"/> one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullableAnnotated(this ParameterInfo source)
    {
        var nic = new NullabilityInfoContext();
        var info = nic.Create(source);
        var done =
            info.ReadState == NullabilityState.Nullable ||
            info.WriteState == NullabilityState.Nullable;

        return done || ((ICustomAttributeProvider)source).IsNullableAnnotated();
    }

    /// <summary>
    /// Determines if the given element has been annotated as a nullable one, either because the
    /// nullability API determines so, or because its metadata either carries a
    /// <see cref="NullableAttribute"/> attribute or a <see cref="IsNullableAttribute"/> one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullableAnnotated(this PropertyInfo source)
    {
        var nic = new NullabilityInfoContext();
        var info = nic.Create(source);
        var done =
            info.ReadState == NullabilityState.Nullable ||
            info.WriteState == NullabilityState.Nullable;

        return done || ((ICustomAttributeProvider)source).IsNullableAnnotated();
    }

    /// <summary>
    /// Determines if the given element has been annotated as a nullable one, either because the
    /// nullability API determines so, or because its metadata either carries a
    /// <see cref="NullableAttribute"/> attribute or a <see cref="IsNullableAttribute"/> one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullableAnnotated(this FieldInfo source)
    {
        var nic = new NullabilityInfoContext();
        var info = nic.Create(source);
        var done =
            info.ReadState == NullabilityState.Nullable ||
            info.WriteState == NullabilityState.Nullable;

        return done || ((ICustomAttributeProvider)source).IsNullableAnnotated();
    }

    /// <summary>
    /// Determines if the given element has been annotated as a nullable one, either because the
    /// nullability API determines so, or because its metadata either carries a
    /// <see cref="NullableAttribute"/> attribute or a <see cref="IsNullableAttribute"/> one.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullableAnnotated(this EventInfo source)
    {
        var nic = new NullabilityInfoContext();
        var info = nic.Create(source);
        var done =
            info.ReadState == NullabilityState.Nullable ||
            info.WriteState == NullabilityState.Nullable;

        return done || ((ICustomAttributeProvider)source).IsNullableAnnotated();
    }

#endif
}