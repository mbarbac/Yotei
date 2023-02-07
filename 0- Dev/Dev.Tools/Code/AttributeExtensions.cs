namespace Dev.Tools;

// ========================================================
public static class AttributeExtensions
{
    /// <summary>
    /// Returns an array with the custom attributes applied to this method, whose names macth
    /// the given one.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="name"></param>
    /// <param name="inherit"></param>
    /// <returns></returns>
    public static object[] GetAttributes(this MethodInfo method, string name, bool inherit)
    {
        method = method.ThrowIfNull();
        name = name.NotNullNotEmpty();

        var items = method.GetCustomAttributes(inherit)
            .Where(x => x is Attribute attr && attr.GetType().Name == name)
            .ToArray();

        return items;
    }

    /// <summary>
    /// Returns an array with the custom attributes applied to this type, whose names macth
    /// the given one.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="inherit"></param>
    /// <returns></returns>
    public static object[] GetAttributes(this Type type, string name, bool inherit)
    {
        type = type.ThrowIfNull();
        name = name.NotNullNotEmpty();

        var items = type.GetCustomAttributes(inherit)
            .Where(x => x is Attribute attr && attr.GetType().Name == name)
            .ToArray();

        return items;
    }
}