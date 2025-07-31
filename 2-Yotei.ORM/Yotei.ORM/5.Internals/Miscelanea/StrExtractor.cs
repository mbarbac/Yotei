using System.ComponentModel.DataAnnotations;

namespace Yotei.ORM.Internals;

// ========================================================
public static class StrExtractor
{
    /// <summary>
    /// Tries to extract at the head of the given source any of the given specifications. If so,
    /// returns true and sets the out main argument to the remaining source without the found
    /// specification, and the out found argument to that specification. Otherwise returns false
    /// and the out arguments are set to arbitrary values.
    /// </summary>
    public static bool ExtractHead(
        this string source,
        bool sensitive, bool isolated, out string main, out string found,
        params string[] specs)
    {
        specs.ThrowWhenNull();

        found = string.Empty;
        main = source.ThrowWhenNull();

        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i].NotNullNotEmpty(trim: false);
            var index = main.IndexOf(spec, sensitive);
            if (index >= 0)
            {
                var valid = true;
                for (int k = 0; k < index; k++) if (main[k] != ' ') { valid = false; break; }
                if (!valid) continue;

                if (isolated)
                {
                    var temp = main.FindIsolated(spec, 0, sensitive);
                    if (temp != index) continue;
                }

                found = main.Substring(index, spec.Length);
                main = main.Remove(found);
                return true;
            }
        }

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract at the tail of the given source any of the given specifications. If so,
    /// returns true and sets the out main argument to the remaining source without the found
    /// specification, and the out found argument to that specification. Otherwise returns false
    /// and the out arguments are set to arbitrary values.
    /// </summary>
    public static bool ExtractTail(
        this string source,
        bool sensitive, bool isolated, out string main, out string found,
        params string[] specs)
    {
        specs.ThrowWhenNull();

        found = string.Empty;
        main = source.ThrowWhenNull();

        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i].NotNullNotEmpty(trim: false);
            var index = main.LastIndexOf(spec, sensitive);
            if (index >= 0)
            {
                var valid = true;
                for (int k = index + spec.Length; k < main.Length; k++) if (main[k] != ' ') { valid = false; break; }
                if (!valid) continue;

                if (isolated)
                {
                    var ini = index - 1;
                    if (ini < 0) ini = 0;

                    var temp = main.FindIsolated(spec, ini, sensitive);
                    if (temp != index) continue;
                }

                found = main.Substring(index, spec.Length);
                main = main.Remove(found);
                return true;
            }
        }

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract the first ocurrence of any of the given specifications. If so, returns
    /// true and sets the head and tail out arguments to the remainings without the found spec,
    /// and the out found argument to that specification. Otherwise returns false and the out
    /// arguments are set to arbitrary values.
    /// </summary>
    public static bool ExtractFirst(
        this string source,
        bool sensitive, bool isolated, out string head, out string found, out string tail,
        params string[] specs)
    {
        specs.ThrowWhenNull();

        found = string.Empty;
        tail = string.Empty;
        head = source.ThrowWhenNull();

        for (int i = 0; i < specs.Length; i++)
        {
            var spec = specs[i].NotNullNotEmpty(trim: false);
            var index = head.LastIndexOf(spec, sensitive);
            if (index >= 0)
            {
                if (isolated)
                {
                    var temp = head.FindIsolated(spec, 0, sensitive);
                    if (temp != index) continue;
                }

                found = head.Substring(index, spec.Length);
                tail = head[(index + spec.Length)..];
                head = head[..index];
                return true;
            }
        }

        return false;
    }
}