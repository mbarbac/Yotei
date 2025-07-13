namespace Yotei.ORM.Internals;

// ========================================================
public static class Extractor
{
    /// <summary>
    /// Removes the heading and tailing rounded brackets from the given parts, interpreted as a
    /// single unit. Heading spaces before the first '(', and tailing ones after the last ')',
    /// are also removed.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool RemoveRoundedBrackets(ref string left, ref string right)
    {
        var sleft = left.AsSpan().Trim();
        var xleft = sleft.StartsWith('(') ? left.IndexOf('(') : -1;

        var sright = right.AsSpan().Trim();
        var xright = sright.EndsWith(')') ? sright.LastIndexOf(')') : -1;

        if (xleft >= 0 && xright >= 0)
        {
            left = left[(xleft + 1)..];
            right = right[..xright];
            return true;
        }

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Extracts from the head of the given source the first matching specification, which must
    /// be isolated or not as requested. If found, the returned spec is trimmed, and all spaces
    /// are kept in main.
    /// <br/> If '<paramref name="isolated"/>' is <c>true</c>, then only isolated specifications
    /// are candidates for matching.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sensitive"></param>
    /// <param name="isolated"></param>
    /// <param name="found"></param>
    /// <param name="specs"></param>
    /// <returns></returns>
    public static (string Main, string Spec) ExtractHead(
        string source, bool sensitive, bool isolated, out bool found, params string[] specs)
    {
        source.ThrowWhenNull();
        specs.ThrowWhenNull();

        for (int i = 0; i < specs.Length; i++)
        {
            var str = source.AsSpan().Trim();
            var spec = specs[i];
            spec = spec.NotNullNotEmpty(trim: true);

            var index = str.IndexOf(spec, sensitive);
            if (index == 0)
            {
                if (isolated)
                {
                    var temp = source.FindIsolated(spec, 0, sensitive);
                    if (temp < 0) continue;
                }

                var item = str[..spec.Length].ToString();
                index = source.IndexOf(spec, sensitive);
                var main = source.Remove(index, spec.Length);

                found = true;
                return (main, item);
            }
        }

        found = false;
        return (string.Empty, string.Empty);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Extracts from the tail of the given source the first matching specification, which must
    /// be isolated or not as requested. If found, the returned spec is trimmed, and all spaces
    /// are kept in main.
    /// <br/> If '<paramref name="isolated"/>' is <c>true</c>, then only isolated specifications
    /// are candidates for matching.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sensitive"></param>
    /// <param name="isolated"></param>
    /// <param name="found"></param>
    /// <param name="specs"></param>
    /// <returns></returns>
    public static (string Main, string Spec) ExtractTail(
        string source, bool sensitive, bool isolated, out bool found, params string[] specs)
    {
        source.ThrowWhenNull();
        specs.ThrowWhenNull();

        for (int i = 0; i < specs.Length; i++)
        {
            var str = source.AsSpan().Trim();
            var spec = specs[i];
            spec = spec.NotNullNotEmpty(trim: true);

            var index = str.LastIndexOf(spec, sensitive);
            if (index >= 0 && (index + spec.Length) == str.Length)
            {
                if (isolated)
                {
                    var temp = source.FindIsolated(spec, 0, sensitive);
                    if (temp < 0) continue;
                }

                var item = str[index..].ToString();
                index = source.LastIndexOf(spec, sensitive);
                var main = source.Remove(index, spec.Length);

                found = true;
                return (main, item);
            }
        }

        found = false;
        return (string.Empty, string.Empty);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Extracts from the given source the left and right parts that are separated by the first
    /// matching specification, which must be isolated or not as requested. If a separator is
    /// found, it is trimmed before returning, and all spaces are kept in both the left and right
    /// parts.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sensitive"></param>
    /// <param name="isolated"></param>
    /// <param name="found"></param>
    /// <param name="specs"></param>
    /// <returns></returns>
    public static (string Left, string Spec, string Right) ExtractSeparator(
        string source, bool sensitive, bool isolated, out bool found, params string[] specs)
    {
        for (int i = 0; i < specs.Length; i++)
        {
            var str = source.AsSpan().Trim();
            var spec = specs[i];
            spec = spec.NotNullNotEmpty(trim: true);

            var index = str.IndexOf(spec, sensitive);
            if (index >= 0)
            {
                if (isolated)
                {
                    var temp = source.FindIsolated(spec, 0, sensitive);
                    if (temp < 0) continue;
                }

                var item = str.ToString().Substring(index, spec.Length);
                index = source.IndexOf(spec, sensitive);
                var main = source.Remove(index, spec.Length);
                var left = main[..index];
                var right = main[index..];

                found = true;
                return (left, item, right);
            }
        }

        found = false;
        return (string.Empty, string.Empty, string.Empty);
    }
}