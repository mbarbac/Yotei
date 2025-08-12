namespace Yotei.ORM.Internals;

// ========================================================
public static partial class StrExtractor
{
    /// <summary>
    /// Tries to remove the left-most and right-most rounded brackets from the given parts,
    /// considering them both as a single unit. Spaces before the first bracket and after the
    /// last one are also removed.
    /// <para>This method is typically invoked when a middle separator is removed from a source
    /// wrapped by rounded brackets, as in '(a=b)', having '(a' and 'b)' as the left and right
    /// parts.</para>
    /// </summary>
    public static bool RemoveBrackets(ref string left, ref string right)
    {
        left.ThrowWhenNull();
        right.ThrowWhenNull();

        var ini = left.IndexOf('('); if (ini < 0) return false;
        var end = right.LastIndexOf(')'); if (end < 0) return false;

        for (int i = 0; i < ini; i++) if (left[i] != ' ') return false;
        for (int i = end + 1; i < right.Length; i++) if (right[i] != ' ') return false;

        left = left[(ini + 1)..];
        right = right[..end];
        return true;
    }

    // ----------------------------------------------------

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
                main = main.RemoveLast(found);
                return true;
            }
        }

        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract the first ocurrence of any of the given specifications. If so, returns
    /// true and sets the head and tail out arguments to the remainings without the found spec,
    /// and the out found argument to that specification. Otherwise, returns false and the out
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
            var index = head.IndexOf(spec, sensitive);
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

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract the last ocurrence of any of the given specifications. If so, returns
    /// true and sets the head and tail out arguments to the remainings without the found spec,
    /// and the out found argument to that specification. Otherwise, returns false and the out
    /// arguments are set to arbitrary values.
    /// </summary>
    public static bool ExtractLast(
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
                    var ini = index - 1;
                    if (ini < 0) ini = 0;

                    var temp = head.FindIsolated(spec, ini, sensitive);
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