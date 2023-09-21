namespace Yotei.Tools;

// ========================================================
public static class AttributeExtensions
{
    public static Attribute[] GetCustomAttributes(
        this Type type, Type attributeType, bool inherit)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(attributeType);

        var name = attributeType.FullName;
        if (name == null) throw new ArgumentException(
            $"Cannot obtain the full name of the given attribute '{name}'.")
            .WithData(attributeType)
            .WithData(type);

        var items = type.GetCustomAttributes(inherit)
            .OfType<Attribute>()
            .Where(x => x.GetType().FullName == name)
            .ToArray();

        return items;
    }

    /// <summary>
    /// Returns an array with all the custom attributes applied to the given method whose name
    /// is the given one.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="attributeType"></param>
    /// <param name="inherit"></param>
    /// <returns></returns>
    public static Attribute[] GetCustomAttributes(
        this MethodInfo method, Type attributeType, bool inherit)
    {
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(attributeType);

        var name = attributeType.FullName;
        if (name == null) throw new ArgumentException(
            $"Cannot obtain the full name of the given attribute '{name}'.")
            .WithData(attributeType)
            .WithData(method);

        var items = method.GetCustomAttributes(inherit)
            .OfType<Attribute>()
            .Where(x => x.GetType().FullName == name)
            .ToArray();

        return items;
    }
}