#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents the pre-release portion of a semantic versioning specification.
/// See: https://semver.org
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
#if NETSTANDARD2_0
internal
#else
public
#endif
record SemanticPreRelease : IComparable<SemanticPreRelease>, IEquatable<SemanticPreRelease>
{
    /// <summary>
    /// An empty shared instance.
    /// </summary>
    public static SemanticPreRelease Empty { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public SemanticPreRelease() { }

    /// <summary>
    /// Initializes a new instance with the given value.
    /// </summary>
    /// <param name="value"></param>
    public SemanticPreRelease(string value) => Value = value;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    public SemanticPreRelease(SemanticPreRelease other)
    {
        other.ThrowWhenNull();
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
        if (Value.Length > 0) sb.Append($"-{Value}");
        if (Metadata.Length > 0) sb.Append($"+{Metadata}");
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator string(
        SemanticPreRelease value)
        => value.ThrowWhenNull().ToString();

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator SemanticPreRelease(
        string value)
        => new(value);

    // ----------------------------------------------------

    /// <summary>
    /// The pre-release value carried by this instance (without any metadata), or an empty string
    /// if any. The value of this property DOES NOT include the leading '-' separator.
    /// </summary>
    public string Value
    {
        get => _Value ?? string.Concat(_ValueParts);
        init
        {
            _ValueParts = [];

            value.ThrowWhenNull();
            value = value.NullWhenEmpty(trim: true)!; if (value is null) return;

            if (value[0] == '-') value = value[1..];
            value = value.NotNullNotEmpty(trim: true);

            var index = value.IndexOf('+');
            if (index >= 0)
            {
                Metadata = value[index..];
                value = value[..index].Trim();
                if (value.Length == 0) return;
            }

            _ValueParts = value.Split('.');
            for (int i = 0; i < _ValueParts.Length; i++)
                _ValueParts[i] = ValidatePart(_ValueParts[i], leadingZeroes: false);
        }
    }
    string? _Value;
    string[] _ValueParts = [];

    /// <summary>
    /// The pre-release metadata carried by this instance, or an empty string if any. The value of
    /// this property DOES NOT include the leading '+' separator.
    /// </summary>
    public string Metadata
    {
        get => _Metadata ?? string.Concat(_MetaParts);
        init
        {
            _MetaParts = [];

            value.ThrowWhenNull();
            value = value.NullWhenEmpty(trim: true)!; if (value is null) return;

            if (value[0] == '+') value = value[1..];
            value = value.NotNullNotEmpty(trim: true);

            _MetaParts = value.Split('.');
            for (int i = 0; i < _MetaParts.Length; i++)
                _MetaParts[i] = ValidatePart(_MetaParts[i], leadingZeroes: true);
        }
    }
    string? _Metadata;
    string[] _MetaParts = [];

    /// <summary>
    /// Determines if this instance is an empty one or not.
    /// </summary>
    public bool IsEmpty => _ValueParts.Length == 0 && _MetaParts.Length == 0;

    // ----------------------------------------------------

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

        if (!leadingZeroes && part[0] == '0')
            throw new ArgumentException("Part carries leading zeroes.").WithData(part);

        return part;
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

        throw null;

        /// <summary>
        /// Compares the two given parts using the precendence rules.
        /// </summary>
        static int CompareParts(string xpart, string ypart)
        {
            var nx = xpart.All(char.IsDigit);
            var ny = ypart.All(char.IsDigit);

            if (nx && ny)
            {
                var vx = int.TryParse(xpart, out var rx) ? rx : 0;
                var vy = int.TryParse(ypart, out var ry) ? ry : 0;
                return vx.CompareTo(vy);
            }
            else
            {
                if (nx) return -1;
                if (ny) return +1;
                return string.Compare(xpart, ypart);
            }
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
    /// Returns a new instance where the original value was increased. If that value was an empty
    /// one, then the given template is returned instead (provided it is not a null one, in which
    /// case an empty instance is returned).
    /// <br/> The <paramref name="increased"/> value is always <c>true</c> unless the source value
    /// was an empty one AND the template was also null or empty. If this happens, then its value
    /// is set to <c>false</c>.
    /// <br/> Any existing metadata is ignored.
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public SemanticPreRelease Increase(out bool increased, string? template = null)
    {
        increased = false;

        // Adjusting the template, if needed...
        template = template?.NotNullNotEmpty(true);
        if (template is not null)
        {
            var index = template.IndexOf('+');
            if (index >= 0)
            {
                template = template[..index];
                template = template.NotNullNotEmpty(trim: true);
            }
        }

        // No value to increase...
        if (_ValueParts.Length == 0)
        {
            increased = template is not null;
            return increased ? new(template!) : Empty;
        }

        // Iterating...
        var parts = (string[])_ValueParts.Clone();
        var overflow = false;

        for (int i = parts.Length - 1; i >= 0; i--)
        {
            var part = parts[i];
            if (part.All(char.IsDigit))
            {
                var value = ulong.TryParse(part, out var temp) ? temp : 0;
                value++;
                parts[i] = value.ToString();
                increased = true;
                break;
            }
            else
            {
                var value = Increase(part, out overflow);
                parts[i] = value;
                increased = true;
                if (!overflow) break;

                if (i == 0) // Adjusting first part for overflow...
                {
                    value += CHARS[0];
                    parts[i] = value;
                    break;
                }
            }
        }

        // Finishing...
        var result = new SemanticPreRelease() { _ValueParts = parts };
        return result;
    }

    // ----------------------------------------------------

    static char[] CHARS = "ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvxyz".ToCharArray();
    static char[] DIGITS = "0123456789".ToCharArray();

    /// <summary>
    /// Returns an increased char with an indication if overflow has happened.
    /// NOTE: we DO NOT consider embedded '-', '+', or any other separators.
    /// </summary>
    static char Increase(char value, out bool overflow)
    {
        if (char.IsDigit(value)) // Increasing digit characters...
        {
            var index = DIGITS.IndexOf(value);

            if (index == DIGITS.Length - 1) { overflow = true; return DIGITS[0]; }
            overflow = false;
            return DIGITS[index + 1];
        }
        else // Increasing letter characters...
        {
            var index = CHARS.IndexOf(value);
            if (index < 0) throw new ArgumentException(
                "Cannot increase an invalid character").WithData(value);

            if (index == CHARS.Length - 1) { overflow = true; return CHARS[0]; }
            overflow = false;
            return CHARS[index + 1];
        }
    }

    /// <summary>
    /// Increases an alphanumeric part with an indication if overflow has happened.
    /// </summary>
    static string Increase(string part, out bool overflow)
    {
        var sb = new StringBuilder(part);
        overflow = false;

        for (int i = sb.Length - 1; i >= 0; i--)
        {
            var value = Increase(sb[i], out overflow);
            sb[i] = value;
        }
        return sb.ToString();
    }
}