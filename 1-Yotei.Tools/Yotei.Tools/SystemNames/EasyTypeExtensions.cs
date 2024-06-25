namespace Yotei.Tools;

// ========================================================
public static class EasyTypeExtensions
{
    /// <summary>
    /// Returns the C#-alike name of the given type, using all options set to false or null.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string EasyName(this Type type)
    {
        return type.EasyName(EasyTypeOptions.False);
    }

    /// <summary>
    /// Returns the C#-alike name of the given type, using the given arguments.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this Type item, EasyTypeOptions options)
    {
        item.ThrowWhenNull();
        options.ThrowWhenNull();

        var genargs = item.GetGenericArguments();
        var genused = 0;
        return item.EasyName(options, genargs, ref genused);
    }
    static string EasyName(this Type item, EasyTypeOptions options, Type[] genargs, ref int genused)
    {
        // Others...
        var sb = new StringBuilder();

        // Finishing...
        return sb.ToString();
    }
}