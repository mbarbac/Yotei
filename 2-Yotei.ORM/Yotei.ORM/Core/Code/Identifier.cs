using System.Data;

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IIdentifier"/>
/// </summary>
[Cloneable(ReturnType = typeof(IIdentifier))]
public partial class Identifier : IIdentifier
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public Identifier(IEngine engine) => throw null;

    /// <summary>
    /// Initializes a new instance with the parts obtained from the given range of values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public Identifier(IEngine engine, IEnumerable<string?> range) => throw null;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected Identifier(Identifier other) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(IIdentifier? other) => throw null;
    //{
    //    if (ReferenceEquals(this, other)) return true;
    //    if (other is null) return false;

    //    if (IgnoreCase != other.IgnoreCase) return false;
    //    if (Count != other.Count) return false;

    //    var temps = other.ToList();
    //    foreach (var item in Items)
    //    {
    //        var index = temps.FindIndex(x => string.Compare(x, item, IgnoreCase) == 0);
    //        if (index >= 0) temps.RemoveAt(index);
    //        else break;
    //    }

    //    return temps.Count == 0;
    //}

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as IMetadataTag);

    public static bool operator ==(Identifier? host, IIdentifier? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(Identifier? host, IIdentifier? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => throw null;
    //{
    //    var code = 0;
    //    code = HashCode.Combine(code, IgnoreCase);
    //    code = HashCode.Combine(code, Count);
    //    foreach (var name in Items) code = HashCode.Combine(code, name);
    //    return code;
    //}

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? Value { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public string? this[int index] { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="useTerminators"></param>
    /// <returns></returns>
    public string? this[int index, bool useTerminators] { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="useTerminators"></param>
    /// <returns></returns>
    public IEnumerable<string?> Enumerate(bool useTerminators) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public bool Contains(string? part) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public int IndexOf(string? part) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public int LastIndexOf(string? part) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public List<int> IndexesOf(string? part) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<string?> predicate) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<string?> predicate) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<string?> predicate) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<string?> predicate) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public bool Match(string? specs) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IIdentifier.IBuilder ToBuilder() => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IIdentifier Replace(int index, string? value) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IIdentifier Add(string? value) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IIdentifier AddRange(IEnumerable<string?> range) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IIdentifier Insert(int index, string? value) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IIdentifier InsertRange(int index, IEnumerable<string?> range) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual IIdentifier RemoveAt(int index) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IIdentifier RemoveRange(int index, int count) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public virtual IIdentifier Remove(string? part) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public virtual IIdentifier RemoveLast(string? part) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public virtual IIdentifier RemoveAll(string? part) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IIdentifier Remove(Predicate<string?> predicate) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IIdentifier RemoveLast(Predicate<string?> predicate) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IIdentifier RemoveAll(Predicate<string?> predicate) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IIdentifier Clear() => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the collection of the raw not-terminated dot-separated parts carried by the given
    /// value, provided the dots are not protected by the engine terminators, if any and if used.
    /// Empty parts are transformed into null one. By default the returned collection is reduced
    /// by removing its null heading parts.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public static List<string?> Split(IEngine engine, string? value, bool reduce = true)
    {
        ArgumentNullException.ThrowIfNull(engine);

        value = value.NullWhenEmpty(trim: true);
        if (value == null) return reduce ? [] : [null];

        var items = engine.UseTerminators ? WithTerminators(value, engine) : NoTerminators(value);
        if (reduce)
        {
            while (items.Count > 0)
            {
                if (items[0] == null) items.RemoveAt(0);
                else break;
            }
        }

        return items;

        /// <summary>
        /// Invoked when the engine does not use terminators.
        /// </summary>
        static List<string?> NoTerminators(string value)
        {
            string?[] items = value.Contains('.') ? value.Split('.') : [value];

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i] = items[i].NullWhenEmpty(trim: true);
                if (item != null)
                {
                    if (item.Contains(' ')) throw new ArgumentException(
                        "Not terminated identifier parts cannot contain spaces.")
                        .WithData(item, "part");
                }
            }

            return [.. items];
        }

        /// <summary>
        /// Invoked when the engine uses terminators.
        /// </summary>
        static List<string?> WithTerminators(string value, IEngine engine)
        {
            var items = engine.UseTerminators ? Split(value, engine) : [.. value.Split('.')];
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i] = items[i]
                    .Unwrap(engine.LeftTerminator, engine.RightTerminator, trim: true)
                    .NullWhenEmpty(trim: true);

                if (item is not null)
                {
                    if (item.StartsWith('.') || item.EndsWith('.')) throw new ArgumentException(
                        "Identifier part cannot begin or end with dots.")
                        .WithData(item);
                }
            }

            return items;

            /// <summary>
            /// Invoked to split the value into its first-level dot-separated parts.
            /// </summary>
            static List<string?> Split(string value, IEngine engine)
            {
                var dots = GetDots(value, engine);
                List<string?> parts = [];
                int ini = 0;
                int end, len;
                string str;

                for (int i = 0; i < dots.Count; i++)
                {
                    end = dots[i];
                    len = end - ini;
                    str = value.Substring(ini, len); parts.Add(str);
                    ini = end + 1;
                }
                end = value.Length;
                len = end - ini;
                str = value.Substring(ini, len); parts.Add(str);

                return parts;
            }

            /// <summary>
            /// Obtains the indexes of the unprotected dots.
            /// </summary>
            static List<int> GetDots(string value, IEngine engine)
                => engine.LeftTerminator == engine.RightTerminator
                ? GetDotsSameTerminators(value, engine.LeftTerminator)
                : GetDotsDifferentTerminators(value, engine.LeftTerminator, engine.RightTerminator);

            /// <summary>
            /// Obtains the indexes of the unprotected dots when both terminators are the same.
            /// </summary>
            static List<int> GetDotsSameTerminators(string value, char ch)
            {
                List<int> dots = [];
                bool found = false;

                for (int i = 0; i < value.Length; i++)
                {
                    var c = value[i];
                    if (c == ch) { found = !found; continue; }
                    if (c == '.') { if (!found) dots.Add(i); continue; }
                }
                return dots;
            }

            /// <summary>
            /// Obtains the indexes of the unprotected dots when both terminators are the same.
            /// </summary>
            static List<int> GetDotsDifferentTerminators(string value, char left, char right)
            {
                List<int> temps = [];
                List<int> dots = [];

                for (int i = 0; i < value.Length; i++)
                {
                    var c = value[i];
                    if (c == left) { temps.Add(i); continue; }
                    if (c == right)
                    {
                        if (temps.Count == 0) continue;

                        var last = temps[^1];
                        temps.RemoveAt(temps.Count - 1);
                        dots = [.. dots.Where(x => x < last)];
                    }
                    if (c == '.') { dots.Add(i); continue; }
                }
                return dots;
            }
        }
    }
}