#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
static partial class EasyNameExtensions
{
    /// <summary>
    /// Obtains the C#-alike easy name representation of the given language element, using default
    /// options.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string EasyName(this Type source) => source.EasyName(EasyTypeOptions.Default);

    /// <summary>
    /// Obtains the C#-alike easy name representation of the given language element, using the
    /// given options.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EasyName(this Type source, EasyTypeOptions options)
    {
        throw null;
    }
}