namespace Runner;

// ========================================================
/// <summary>
/// Represents the pre-release portion of a semantic specification, as defined at
/// 'https://semver.org'.
/// <br/> A pre-release version is denoted by appending a hypen and a series of dot separated
/// identifiers, that use ASCII alphanumeric characters [0 - 9, A - Z, a - z] only, and that MUST
/// NOT be empty. Hyphens are only allowed once as the separator. Numeric identifiers MUST not
/// include leading zeroes.
/// <br/> Metadata is denoted by appending a plus sign and a series of dot-separated identifiers
/// immediately following the patch or pre-release version. Identifiers must comprise ASCII
/// alphanumerics [0 - 9, A - Z, a - z] characters only, and MUST NOT be empty.
/// <br/> Precedence is calculated by comparing each dot-separated identifier from left to right
/// until a difference is found either numerically or is ASCII sort order. Numeric identifiers
/// always have lower precendence than alphanumeric ones. When comparing precedence build metadata
/// is ignored, but used for equality purposes.
/// </summary>
public record SemanticRelease : IComparable<SemanticRelease>, IEquatable<SemanticRelease>
{
    public static SemanticRelease Empty { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public SemanticRelease() { }

    /// <summary>
    /// Initializes a new instance with the given source value, which must contain a pre-release
    /// value, with or without leading hypens, and optional metadata separated by the first plus
    /// character.
    /// </summary>
    /// <param name="source"></param>
    public SemanticRelease(string source) => Value = source;

    /// <summary>
    /// Returns a string representation of this instance, with or without a leading hypen as
    /// requested if needed.
    /// </summary>
    /// <param name="hyphen"></param>
    /// <returns></returns>
    public string ToString(bool hyphen)
    {
        if (Value.Length == 0 && Metadata.Length == 0) return string.Empty;
        else
        {
            var sb = new StringBuilder();

            if (Value.Length > 0)
            {
                if (hyphen) sb.Append('-');
                sb.Append(Value);
            }
            if (Metadata.Length > 0) sb.Append($"+{Metadata}");
            return sb.ToString();
        }
    }

    /// <inheritdoc/>
    public override string ToString() => ToString(false);

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator string(SemanticRelease value) => value.ThrowWhenNull().ToString();

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator SemanticRelease(string value) => new(value);

    // ----------------------------------------------------

    /// <summary>
    /// Represents the actual value of the pre-release part of a semantic version specification,
    /// which is separated from the normal one by a hyphen (not included in this property), or an
    /// empty string. The value contains a series of dot-separated identifiers that use ASCII
    /// alphanumeric characters [0 - 9, A - Z, a - z] only, that MUST NOT be empty (or consisting
    /// in hyphens only), and must not contain leading zeroes.
    /// </summary>
    public string Value
    {
        get => _Value;
        init
        {
            value.ThrowWhenNull();

            if (value.Length > 0)
            {
                value.NotNullNotEmpty();

                var index = value.IndexOf('+');
                if (index >= 0)
                {
                    var meta = value[index..];
                    Metadata = meta;

                    value = value[..index];
                    if (value.Length == 0)
                    {
                        _Value = string.Empty;
                        return;
                    }
                }

                if (value[0] == '-') value = value[1..];

                var parts = value.Split('.');
                foreach (var part in parts) ValidatePart(part);
            }

            _Value = value;
        }
    }
    string _Value = string.Empty;

    /// <summary>
    /// Represents the build metadata portion of a semantic specification, which is separated the
    /// other ones by a plus sign (not included in this property), or an empty string. It contains
    /// a series of dot separated identifiers using ASCII alphanumeric characters [0 - 9, A - Z,
    /// a - z] only, that MUST NOT be empty.
    /// </summary>
    public string Metadata
    {
        get => _Metadata;
        init
        {
            value.ThrowWhenNull();

            if (value.Length > 0)
            {
                value.NotNullNotEmpty();

                if (value[0] == '+') value = value[1..];

                var parts = value.Split('.');
                foreach (var part in parts) ValidatePart(part);
            }

            _Metadata = value;
        }
    }
    string _Metadata = string.Empty;

    /// <summary>
    /// Determines if this instance is an empty one, or not.
    /// </summary>
    public bool IsEmpty => Value.Length == 0 && Metadata.Length == 0;

    /// <summary>
    /// Invoked to validate a given identifier or part, which must use ASCII alphanumeric characters
    /// [0 - 9, A - Z, a - z] only, and must not be empty.
    /// </summary>
    /// <param name="part"></param>
    /// <param name="noLeadingZeroes"></param>
    static void ValidatePart(string part, bool noLeadingZeroes = false)
    {
        part.NotNullNotEmpty();

        foreach (var c in part)
            if (!char.IsAsciiLetter(c) && !char.IsAsciiDigit(c))
                throw new ArgumentException(
                    "Identifier part carries invalid characters.")
                    .WithData(part);

        if (noLeadingZeroes &&
            part[0] == '0')
            throw new ArgumentException(
                "Identifier carries leading zeroes.")
                    .WithData(part);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Compares the two given instances and returns an integer that indicates whether the first
    /// one precedes, follows, or occurs in the same position as the second one. Comparison is
    /// performed by comparing the parts in the regular value, but does not take into consideration
    /// the build metadata.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int Compare(SemanticRelease? x, SemanticRelease? y)
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return +1;

        var xparts = x.Value.Split('.'); var xlen = xparts.Length;
        var yparts = y.Value.Split('.'); var ylen = yparts.Length;

        int i = 0; while (true)
        {
            if (i == xlen && i == ylen) return 0;
            if (i == xlen) return -1;
            if (i == ylen) return +1;

            var r = CompareParts(xparts[i], yparts[i]);
            if (r != 0) return r;
            i++;
        }

        // Compares the two given parts using the precedence rules...
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

            if (nx) return -1;
            if (ny) return +1;

            return string.Compare(xpart, ypart);
        }
    }

    /// <summary>
    /// <inheritdoc/> Comparison is performed by comparing the parts in the regular value, but
    /// does not take into consideration the build metadata.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(SemanticRelease? other) => Compare(this, other);

    public static bool operator >(SemanticRelease? x, SemanticRelease? y) => Compare(x, y) > 0;
    public static bool operator <(SemanticRelease? x, SemanticRelease? y) => Compare(x, y) < 0;
    public static bool operator >=(SemanticRelease? x, SemanticRelease? y) => Compare(x, y) >= 0;
    public static bool operator <=(SemanticRelease? x, SemanticRelease? y) => Compare(x, y) <= 0;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/> Comparison is performed by comparing the parts in the regular value, and
    /// by comparing the build metadata as well.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(SemanticRelease? other)
    {
        return
            Compare(this, other) == 0 &&
            other is not null &&
            string.Compare(Metadata, other.Metadata) == 0;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the original value increased, provided that such is not
    /// empty, and can be treated as a trailing numeric one. If not, the given template is
    /// used as the new value provided it is not null. Otherswise, the value of the new
    /// instance is not modified.
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public SemanticRelease Increase(string? template = null) => Increase(out _, template);

    /// <summary>
    /// Returns a new instance with the original value increased, provided that such is not
    /// empty, and can be treated as a trailing numeric one. If so, or if the given template
    /// is not null and so used as the new value, the out argument is set to <c>true</c>.
    /// Otherwise, is set to false.
    /// <br/> By default, the existing build metadata is discarded, although it may happen
    /// that the template value contains it.
    /// </summary>
    /// <param name="increased"></param>
    /// <param name="template"></param>
    /// <returns></returns>
    public SemanticRelease Increase(out bool increased, string? template = null)
    {
        template = template?.NotNullNotEmpty();

        // No value to increase...
        if (Value.Length == 0)
        {
            increased = template is not null;
            return new(template is null ? Value : template);
        }

        // Preparing...
        var parts = Value.Split('.');
        var temp = parts[^1];

        // We may have not a trailing numeric chunk to increase...
        var index = FirstDigit(temp);
        if (index < 0)
        {
            increased = template is not null;
            return new(template is null ? Value : template);
        }

        // Or increasing that numeric trailing chunk...
        else
        {
            var num = temp[index..];
            var item = int.TryParse(num, out var r) ? r : 0;
            item++;

            var len = num.Length;
            num = item.ToString($"D{len}");

            temp = temp[..index];
            temp += num;
            parts[^1] = temp;

            increased = true;
            return string.Join('.', parts);
        }

        // The index of the first digit, or -1 if any...
        static int FirstDigit(string value)
        {
            var r = -1;

            for (int i = value.Length - 1; i >= 0; i--)
            {
                if (char.IsDigit(value[i])) r = i;
                else break;
            }
            return r;
        }
    }
}