using System.ComponentModel.DataAnnotations;

namespace Yotei.ORM.Internals;

// ========================================================
public static class StrExtractor
{
    /// <summary>
    /// Tries to extract at the head of the given source any of the given specifications. If so,
    /// returns true and sets the out main argument to the trimmed remaining source without the
    /// found specification, and the out found argument to that specification. Otherwise returns
    /// false and the out arguments are set to arbitrary values.
    /// </summary>
    public static bool ExtractHead(
        this string source,
        bool sensitive, bool isolated, out string main, out string found,
        params string[] specs)
    {
        source.ThrowWhenNull();
        specs.ThrowWhenNull();

        found = string.Empty;
        main = source.NullWhenEmpty(trim: true)!; if (main is null) return false;

        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i].NotNullNotEmpty(trim: true);
            var index = main.IndexOf(spec, sensitive);
            if (index == 0)
            {
                if (isolated)
                {
                    var temp = main.FindIsolated(spec, 0, sensitive);
                    if (temp != index) continue;
                }

                found = main[..spec.Length];
                main = main.Remove(index, spec.Length).Trim();
                return true;
            }
        }

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract at the tail of the given source any of the given specifications. If so,
    /// returns true and sets the out main argument to the trimmed remaining source without the
    /// found specification, and the out found argument to that specification. Otherwise returns
    /// false and the out arguments are set to arbitrary values.
    /// </summary>
    public static bool ExtractTail(
        this string source,
        bool sensitive, bool isolated, out string main, out string found,
        params string[] specs)
    {
        source.ThrowWhenNull();
        specs.ThrowWhenNull();

        found = string.Empty;
        main = source.NullWhenEmpty(trim: true)!; if (main is null) return false;

        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i].NotNullNotEmpty(trim: true);
            var index = main.LastIndexOf(spec, sensitive);
            if (index >= 0 && (index + spec.Length) == main.Length)
            {
                if (isolated)
                {
                    var ini = index - 1;
                    if (ini < 0) ini = 0;

                    var temp = main.FindIsolated(spec, ini, sensitive);
                    if (temp != index) continue;
                }

                found = main[..spec.Length];
                main = main.Remove(index, spec.Length).Trim();
                return true;
            }
        }

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract the first ocurrence of any of the given specifications. If so, returns
    /// true and sets the out head and tail out arguments to the trimmed remainings without the
    /// found specification, and the out found argument to that specification. Otherwise returns
    /// false and the out arguments are set to arbitrary values.
    /// </summary>
    public static bool ExtractFirst(
        this string source,
        bool sensitive, bool isolated, out string head, out string found, out string tail,
        params string[] specs)
    {
        source.ThrowWhenNull();
        specs.ThrowWhenNull();

        found = string.Empty;
        tail = string.Empty;
        head = source.NullWhenEmpty(trim: true)!; if (head is null) return false;

        throw null;

        //for (int i = 0; i < specs.Length; i++)
        //{
        //    var spec = specs[i].NotNullNotEmpty(trim: true);
        //    var index = main.IndexOf(spec, sensitive);
        //    if (index == 0)
        //    {
        //        if (isolated)
        //        {
        //            var temp = main.FindIsolated(spec, 0, sensitive);
        //            if (temp != index) continue;
        //        }

        //        found = main[..spec.Length];
        //        main = main.Remove(index, spec.Length).Trim();
        //        return true;
        //    }
        //}

        //return false;
    }
}