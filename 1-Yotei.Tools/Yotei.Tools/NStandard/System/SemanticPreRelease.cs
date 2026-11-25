using System.Collections.ObjectModel;

#if NETSTANDARD2_0
namespace Yotei.Tools.NStd20;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Represents the pre-release portion of a semantic versioning specification.
/// See: https://semver.org
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public record SemanticPreRelease : IComparable<SemanticPreRelease>, IEquatable<SemanticPreRelease>
{
    /// <summary>
    /// An empty shared instance.
    /// </summary>
    public static SemanticPreRelease Empty { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public SemanticPreRelease()
    {
        _ValueParts = [];
        _MetaParts = [];
    }

    /// <summary>
    /// Initializes a new instance with the given value.
    /// </summary>
    /// <param name="value"></param>
    public SemanticPreRelease(string value) : this() => Value = value;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    public SemanticPreRelease(SemanticPreRelease other)
    {
        ArgumentNullException.ThrowIfNull(other);

        _ValueParts = (string[])other._ValueParts.Clone();
        _MetaParts = (string[])other._MetaParts.Clone();
    }

    /// <summary>
    /// <inheritdoc/>
    /// <br/> The returned value includes the leading '-' or '+' separators.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        if (IsEmpty) return string.Empty;

        var sb = new StringBuilder();
        if (_ValueParts.Length > 0) sb.Append($"-{Value}");
        if (_MetaParts.Length > 0) sb.Append($"+{Metadata}");
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator string(
        SemanticPreRelease value) => value.ThrowWhenNull().ToString();

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator SemanticPreRelease(string value) => new(value);

    // ----------------------------------------------------

    /// <summary>
    /// The pre-release value carried by this instance (without any metadata), or an empty string
    /// if any. The value of this property DOES NOT include the leading '-' separator.
    /// </summary>
    public string Value
    {
        get
        {
#if NETSTANDARD2_0
            return _Value ??= string.Join(".", _ValueParts);
#else
            return _Value ??= string.Join('.', _ValueParts);
#endif
        }
        init
        {
            _ValueParts = []; // Preventive cleaning...

            // Empty is allowed, but not null or blanks-only...
            ArgumentNullException.ThrowIfNull(value);

            if (value.Length == 0) return;
            value = value.NotNullNotEmpty(trim: true);

            // It can start with '-'...
            if (value[0] == '-') value = value[1..].NotNullNotEmpty(trim: true);

            // Intercepting the metadata part...
            var index = value.IndexOf('+');
            if (index >= 0)
            {
                Metadata = value[index..];
                value = value[..index];
                value = value.NotNullNotEmpty(trim: true);
            }

            // Getting the dot-separated parts...
            _ValueParts = value.Split('.'); for (int i = 0; i < _ValueParts.Length; i++)
            {
                var part = _ValueParts[i]; part = ValidatePart(part, leadingZeroes: false);
                _ValueParts[i] = part;
            }
        }
    }
    string? _Value;

    /// <summary>
    /// Reports what are the actual parts in the value.
    /// </summary>
    public ReadOnlyCollection<string> ValueParts => Array.AsReadOnly(_ValueParts);
    string[] _ValueParts;

    /// <summary>
    /// The pre-release metadata carried by this instance, or an empty string if any. The value of
    /// this property DOES NOT include the leading '+' separator.
    /// </summary>
    public string Metadata
    {
        get
        {
#if NETSTANDARD2_0
            return _Metadata ??= string.Join(".", _MetaParts);
#else
            return _Metadata ??= string.Join('.', _MetaParts);
#endif
        }
        init
        {
            _MetaParts = []; // Preventive cleaning...

            // Empty is allowed, but not null or blanks-only...
            ArgumentNullException.ThrowIfNull(value);

            if (value.Length == 0) return;
            value = value.NotNullNotEmpty(trim: true);

            // It can start with '+'...
            if (value[0] == '+') value = value[1..].NotNullNotEmpty(trim: true);

            // Getting the dot-separated parts...
            _MetaParts = value.Split('.');
            for (int i = 0; i < _MetaParts.Length; i++)
                _MetaParts[i] = ValidatePart(_MetaParts[i], leadingZeroes: true);
        }
    }
    string? _Metadata;

    /// <summary>
    /// Reports what are the actual parts in the metadata.
    /// </summary>
    public ReadOnlyCollection<string> MetadataParts => Array.AsReadOnly(_MetaParts);
    string[] _MetaParts;

    /// <summary>
    /// Determines if this instance is an empty one or not.
    /// </summary>
    public bool IsEmpty => _ValueParts.Length == 0 && _MetaParts.Length == 0;

    // ----------------------------------------------------

    readonly static char[] CHARS = "ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvxyz".ToCharArray();
    readonly static char[] DIGITS = "0123456789".ToCharArray();

    /// <summary>
    /// Returns a validated part: only standard ASCII characters and digits are allowed.
    /// </summary>
    static string ValidatePart(string part, bool leadingZeroes)
    {
        part = part.NotNullNotEmpty(trim: true);

        foreach (var c in part)
        {
            if (CHARS.IndexOf(c) < 0 && DIGITS.IndexOf(c) < 0)
                throw new ArgumentException("Part carries invalid characters.").WithData(part);
        }

        if (!leadingZeroes &&
            part[0] == '0' &&
            part.All(char.IsDigit))
            throw new ArgumentException("Part carries leading zeroes.").WithData(part);

        return part;
    }

    /// <summary>
    /// Splits the given part into its head and tail trunks. The tail one is the trailing digits
    /// only portion of the part, and may be empty if any. The head one is the leading remaining
    /// part, and it may be empty if the part is a digits-only one.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    static (string Head, string Tail) GetTrunks(string part)
    {
        // Getting the first trailing digit...
        var index = -1; for (int i = part.Length - 1; i >= 0; i--)
        {
            if (!char.IsDigit(part[i])) break;
            index = i;
        }

        // Finishing...
        var head = index < 0 ? part : part[..index];
        var tail = index < 0 ? "" : part[index..];
        return (head, tail);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Compares the two given instances and returns a value that determines if the first one
    /// precedes, follows, or is equal to the other given one.
    /// <br/> Note that this method DOES NOT take into consideration the metadata parts.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int Compare(SemanticPreRelease? x, SemanticPreRelease? y)
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return +1;

        var xlen = x._ValueParts.Length;
        var ylen = y._ValueParts.Length;

        int i = 0; while (true)
        {
            if (i == xlen && i == ylen) return 0;
            if (i == xlen) return -1;
            if (i == ylen) return +1;

            var r = CompareParts(x._ValueParts[i], y._ValueParts[i]);
            if (r != 0) return r;
            i++;
        }

        /// <summary>
        /// Compares the two given parts using the precedence rules.
        /// </summary>
        static int CompareParts(string xpart, string ypart)
        {
            // Get the trunks...
            var (xhead, xtail) = GetTrunks(xpart);
            var (yhead, ytail) = GetTrunks(ypart);

            // All numeric
            if (xhead.Length == 0 && yhead.Length == 0)
            {
                var xvalue = ulong.TryParse(xtail, out var rx) ? rx : 0;
                var yvalue = ulong.TryParse(ytail, out var ry) ? ry : 0;
                var result = xvalue.CompareTo(yvalue);
                if (result < 0) return -1;
                if (result > 0) return +1;
                return 0;
            }

            // Numeric parts have lower precedence...
            if (xhead.Length == 0) return -1;
            if (yhead.Length == 0) return +1;

            // Trailing numeric trunks...
            if (xtail.Length > 0 && ytail.Length > 0)
            {
                // The head must be the same to proceed to numeric comparison...
                if (string.Compare(xhead, yhead) == 0)
                {
                    var xvalue = ulong.TryParse(xtail, out var rx) ? rx : 0;
                    var yvalue = ulong.TryParse(ytail, out var ry) ? ry : 0;
                    var result = xvalue.CompareTo(yvalue);
                    if (result < 0) return -1;
                    if (result > 0) return +1;
                }
            }
            else // Trunk numeric parts have lowe precedence...
            {
                if (xtail.Length > 0) return -1;
                if (ytail.Length > 0) return +1;
            }

            // Alphanumeric default case...
            var r = string.Compare(xpart, ypart, StringComparison.Ordinal);
            if (r < 0) return -1;
            if (r > 0) return +1;
            return 0;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// <br/> Note that this method DOES NOT take into consideration the metadata parts.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(SemanticPreRelease? other) => Compare(this, other);

    public static bool operator >(SemanticPreRelease x, SemanticPreRelease y) => Compare(x, y) > 0;

    public static bool operator <(SemanticPreRelease x, SemanticPreRelease y) => Compare(x, y) < 0;

    public static bool operator >=(SemanticPreRelease x, SemanticPreRelease y) => Compare(x, y) >= 0;

    public static bool operator <=(SemanticPreRelease x, SemanticPreRelease y) => Compare(x, y) <= 0;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// <br/> Note that this method DOES NOT take into consideration the metadata part.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(SemanticPreRelease? other) => Compare(this, other) == 0;

    /// <summary>
    /// <inheritdoc/>
    /// <br/> Note that this method DOES NOT take into consideration the metadata part.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => HashCode.Combine(Value);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the release value has been increased, if possible, setting
    /// the value of the out argument accordingly. Only the last numeric part, or the last one
    /// containing a trailing numeric trunk, is increased. Alphanumeric parts are skipped and
    /// not increased.
    /// </summary>
    /// <returns></returns>
    public SemanticPreRelease Increase(out bool increased)
    {
        // No value to increase...
        increased = false;
        if (_ValueParts.Length == 0) return Empty;

        // Preparing...
        var parts = (string[])_ValueParts.Clone();
        for (int i = parts.Length - 1; i >= 0; i--)
        {
            var part = parts[i];
            var (head, tail) = GetTrunks(part);

            // Pure numeric...
            if (head.Length == 0)
            {
                var value = ulong.TryParse(tail, out var r) ? r : 0; value++;
                parts[i] = value.ToString();
                increased = true;
                break;
            }

            // Trailing numeric...
            if (head.Length > 0 && tail.Length > 0)
            {
                var value = ulong.TryParse(tail, out var r) ? r : 0; value++;
                var len = tail.Length;
                var trunk = value.ToString($"D{len}");
                
                head += trunk;
                parts[i] = head;
                increased = true;
                break;
            }
        }

        // Finishing...
        if (!increased) return this;

        for (int i = 0; i < parts.Length; i++)
        {
            var part = parts[i]; part = ValidatePart(part, leadingZeroes: false);
            parts[i] = part;
        }
        return new SemanticPreRelease() { _ValueParts = parts };
    }
}