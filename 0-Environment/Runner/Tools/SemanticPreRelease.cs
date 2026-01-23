namespace Runner;

// ========================================================
/// <summary>
/// Represents the pre-release portion of a semantic specification.
/// <br/> See: https://semver.org
/// <para>
/// Pre-release specifications are denoted by appending a hypen and then a series of dot-separated
/// identifiers, using ASCII alphanumeric characters only [0-9, A-Z, a-z], that must not be empty.
/// Hypens are allowed only once. Numeric identifiers must not include leading zeroes.
/// <br/> Metadata is then denoted by appending a plus sign '+' following the patch or pre-release
/// version and a series of dot-separated identifiers with the same syntax.
/// <br/> Precendence is calculated by comparing each dot-separated identifier from left to righ
/// until a difference is found, either numeracally or in ASCII sort-order. Numeric identifiers
/// always have lower precendence than alphanumeric ones. When comparing precedence, metadata is
/// ignored, but used for equality purposes.
/// </para>
/// </summary>
public record SemanticPreRelease : IComparable<SemanticPreRelease>
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
    /// Initializes a new instance with the given source value, which must contain a pre-release
    /// value, with or without leading hypens, and optional metadata separated by the first plus
    /// character.
    /// </summary>
    /// <param name="source"></param>
    public SemanticPreRelease(string source) => Value = source;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    public SemanticPreRelease(SemanticPreRelease source)
    {
        source.ThrowWhenNull();
        _Value = source._Value;
        _Metadata = source._Metadata;
    }

    /// <summary>
    /// Conversion operator.
    /// </summary>
    public static implicit operator string(
        SemanticPreRelease value) => value.ThrowWhenNull().ToString();

    /// <summary>
    /// Conversion operator.
    /// </summary>
    public static implicit operator SemanticPreRelease(string value) => new(value);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
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
    /// Represents the value of this pre-release part, without any metadata.
    /// <br/> Note that this value shall be separated from the main semantic version parts using a
    /// hypen, which is not included in this property.
    /// </summary>
    public string Value
    {
        get => _Value;
        init
        {
            value.ThrowWhenNull();

            if (value.Length > 0)
            {
                value.NotNullNotEmpty(true);

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
    /// Represents the build metadata of this specification, or an empty string.
    /// <br/> Note that this value shall be separated from the pre-release part using a plus '+'
    /// sign, which is not included in this property.
    /// </summary>
    public string Metadata
    {
        get => _Metadata;
        init
        {
            value.ThrowWhenNull();

            if (value.Length > 0)
            {
                value = value.NotNullNotEmpty(true);

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

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the given part, which must use ASCII alphanumeric characters only
    /// [0 - 9, A - Z, a - z], and must not be empty.
    /// </summary>
    /// <param name="part"></param>
    /// <param name="noLeadingZeroes"></param>
    static void ValidatePart(string part, bool noLeadingZeroes = false)
    {
        part.NotNullNotEmpty(true);

        foreach (var c in part)
            if (!char.IsAsciiLetter(c) && !char.IsAsciiDigit(c))
                throw new ArgumentException("Identifier carries invalid characters.").WithData(part);

        if (noLeadingZeroes && part[0] == '0')
            throw new ArgumentException("Identifier carries leading zeroes.").WithData(part);
    }

    // ----------------------------------------------------


    /// <summary>
    /// <inheritdoc cref="IComparable.CompareTo(object?)"/>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns><inheritdoc cref="IComparable.CompareTo(object?)"/></returns>
    public static int Compare(SemanticPreRelease? x, SemanticPreRelease? y)
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
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public int CompareTo(SemanticPreRelease? other) => Compare(this, other);

    public static bool operator >(SemanticPreRelease? x, SemanticPreRelease? y) => Compare(x, y) > 0;
    public static bool operator <(SemanticPreRelease? x, SemanticPreRelease? y) => Compare(x, y) < 0;
    public static bool operator >=(SemanticPreRelease? x, SemanticPreRelease? y) => Compare(x, y) >= 0;
    public static bool operator <=(SemanticPreRelease? x, SemanticPreRelease? y) => Compare(x, y) <= 0;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public virtual bool Equals(SemanticPreRelease? other)
    {
        if (other is null) return false;
        if (CompareTo(other) != 0) return false;
        if (string.Compare(Metadata, other.Metadata, ignoreCase: true) != 0) return false;
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override int GetHashCode() => HashCode.Combine(Value, Metadata);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the original value was increased, provided that it is not an
    /// empty one, and that it can be treated as a trailing numeric one. If this instance was an
    /// empty one, then the optional template (if not null) is used.
    /// <br/> The existing build metadata, if any, is erased. You can add it later if needed.
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public SemanticPreRelease Increase(string? template = null) => Increase(out _, template);

    /// <summary>
    /// Returns a new instance where the original value was increased, provided that it is not an
    /// empty one, and that it can be treated as a trailing numeric one. If this instance was an
    /// empty one, then the optional template (if not null) is used.
    /// <br/> If the value cannot be increased the out argument is set to <c>false</c>.
    /// <br/> The existing build metadata, if any, is erased. You can add it later if needed.
    /// </summary>
    /// <param name="increased"></param>
    /// <param name="template"></param>
    /// <returns></returns>
    public SemanticPreRelease Increase(out bool increased, string? template = null)
    {
        template = template?.NotNullNotEmpty(true);
        if (template is not null)
        {
            var ix = template.IndexOf('+');
            if (ix >= 0)
            {
                template = template[..ix];
                template = template.NotNullNotEmpty(true);
            }
        }

        // No value to increase...
        if (Value.Length == 0)
        {
            increased = template is not null;
            return increased ? new(template!) : Empty;
        }

        // Preparing...
        var parts = Value.Split('.');
        var temp = parts[^1];

        // We might have not a trailing numeric chunk to increase...
        var index = FirstDigit(temp);
        if (index < 0)
        {
            increased = template is not null;
            return new(template is null ? Value : template);
        }

        // Or increasing that numeric trailing chunk...
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

        /// <summary>
        /// The index of the first digit, or -1 if any... 
        /// </summary>
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