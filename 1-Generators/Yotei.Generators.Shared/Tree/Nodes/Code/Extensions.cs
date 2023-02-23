namespace Yotei.Generators.Tree;

// ========================================================
internal static class Extensions
{
    /// <summary>
    /// Obtains the captured type from the given captured element.
    /// </summary>
    /// <param name="captured"></param>
    /// <returns></returns>
    public static ICapturedType AsCapturedType(this ICaptured captured)
    {
        captured = captured.ThrowIfNull(nameof(captured));

        return captured switch {
            ICapturedType item => item,
            ICapturedProperty item => item.Parent,
            ICapturedField item => item.Parent,

            _ => throw new UnreachableException($"Unknown element: {captured}")
        };
    }
}