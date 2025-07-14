namespace Yotei.ORM.Internals;

// ========================================================
public static partial class Extractor
{
    /// <summary>
    /// Removes the heading and tailing rounded brackets from the given parts, interpreted as a
    /// single unit. Heading spaces before the first '(', and tailing ones after the last ')',
    /// are also removed.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool RoundedBrackets(ref string left, ref string right)
    {
        left.ThrowWhenNull();
        right.ThrowWhenNull();

        var main = $"{left}{right}".Trim();
        var tokenizer = new StrWrappedTokenizer('(', ')');
        var token = tokenizer.Tokenize(main);

        if (token is StrTokenWrapped wrapped)
        {
            var index = left.IndexOf('('); left = left[(index + 1)..];
            index = right.LastIndexOf(')'); right = right[..index];
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
    public static (string Main, string Spec) Head(
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
    public static (string Main, string Spec) Tail(
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
    /// separator that matches any of the given specifications, in order, which must be isolated
    /// or not as requested. If a separator is found, it is trimmed before returning, and all
    /// spaces are kept in both the left and right parts.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sensitive"></param>
    /// <param name="isolated"></param>
    /// <param name="found"></param>
    /// <param name="specs"></param>
    /// <returns></returns>
    public static (string Left, string Spec, string Right) FirstSeparator(
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

    // ----------------------------------------------------

    /// <summary>
    /// Extracts from the given source the left and right parts that are separated by the last
    /// separator that matches any of the given specifications, in order, which must be isolated
    /// or not as requested. If a separator is found, it is trimmed before returning, and all
    /// spaces are kept in both the left and right parts.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sensitive"></param>
    /// <param name="isolated"></param>
    /// <param name="found"></param>
    /// <param name="specs"></param>
    /// <returns></returns>
    public static (string Left, string Spec, string Right) LastSeparator(
        string source, bool sensitive, bool isolated, out bool found, params string[] specs)
    {
        for (int i = 0; i < specs.Length; i++)
        {
            var str = source.AsSpan().Trim();
            var spec = specs[i];
            spec = spec.NotNullNotEmpty(trim: true);

            var index = str.LastIndexOf(spec, sensitive);
            if (index >= 0)
            {
                if (isolated)
                {
                    var temp = source.FindIsolated(spec, 0, sensitive);
                    if (temp < 0) continue;
                }

                var item = str.ToString().Substring(index, spec.Length);
                index = source.LastIndexOf(spec, sensitive);
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