namespace Runner;

// ========================================================
/// <summary>
/// Represents the optional pre-release portion of a semantic version, composed by its value
/// and metadata parts. See 'https://semver.org' for details.
/// </summary>
public record SemanticRelease : IComparable<SemanticRelease>, IEquatable<SemanticRelease>
{
    /// <summary>
    /// A shared empty instance.
    /// </summary>
    public static SemanticRelease Empty { get; } = new();

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public SemanticRelease() { }

    /// <summary>
    /// Initializes a new instance with the given source value, which may contain both its
    /// version value and its build metadata, separated by a '+' symbol.
    /// </summary>
    /// <param name="source"></param>
    public SemanticRelease(string source)
    {
        source = source.ThrowWhenNull();
        if (source.Length > 0) Value = source;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var value = Value.Length == 0 ? string.Empty : $"{Value}";
        var meta = Metadata.Length == 0 ? string.Empty : $"+{Metadata}";
        return value + meta;
    }

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator string(SemanticRelease value)
    {
        value = value.ThrowWhenNull();
        return value.ToString();
    }

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator SemanticRelease(string value)
    {
        value = value.ThrowWhenNull();
        return new(value);
    }

    /// <summary>
    /// The value of the pre-release version, without its preceding hyphen character, or an empty
    /// string if any. The init setter removes the hyphen found as its first character, if any,
    /// and accepts an optional metadata part separated by the first plus character.
    /// </summary>
    public string Value
    {
        get => _Value;
        init
        {
            _Value = ValidateValue(value, out _, out var metadata);
            if (metadata.Length > 0) Metadata = metadata;
        }
    }
    string _Value = string.Empty;

    // Validates the given value portion...
    static string ValidateValue(string value, out string[] parts, out string metadata)
    {
        value = value.ThrowWhenNull();
        parts = [];
        metadata = string.Empty;

        if (value.Length > 0 && value[0] == '-') value = value[1..];
        if (value.Length > 0)
        {
            var index = value.IndexOf('+');
            if (index >= 0)
            {
                metadata = value[index..];
                value = value[..index];
            }
        }
        if (value.Length > 0)
        {
            parts = value.Split('.');
            foreach (var part in parts)
            {
                ValidatePart(part);
                NoLeadingZeroes(part);
            }
        }
        return value;
    }

    // Validates the part contains no leading zeroes...
    static void NoLeadingZeroes(string part)
    {
        if (part.Length > 1 &&
            part.All(char.IsDigit) &&
            part[0] == '0')
            throw new ArgumentException(
                "Leading zeroes are not allowed.").WithData(part);
    }

    // Validates the chars of the given part...
    static void ValidatePart(string part)
    {
        part = part.ThrowWhenNull();
        if (part.Length == 0) throw new EmptyException("Part is empty.");

        if (part[0] == '-') throw new ArgumentException(
            "Leading '-' characters are not allowed in parts.")
            .WithData(part);

        foreach (var c in part)
            if (!ValidateChar(c))
                throw new ArgumentException("Part carries invalid character(s).")
                .WithData(c)
                .WithData(part);
    }

#pragma warning disable IDE0078
    // Validates the given char...
    static bool ValidateChar(char c) =>
        c == '-' ||
        (c >= '0' && c <= '9') ||
        (c >= 'A' && c <= 'Z') ||
        (c >= 'a' && c <= 'z');
#pragma warning restore

    /// <summary>
    /// The value of the build metadata, without its preceding plus character, or an empty string
    /// if any. The init setter removes the plus character found as its first one, if any.
    /// </summary>
    public string Metadata
    {
        get => _Metadata;
        init
        {
            value = value.ThrowWhenNull();

            if (value.Length > 0 && value[0] == '+') value = value[1..];
            if (value.Length > 0)
            {
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

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance with the original value increased and its metadata cleared. If the
    /// original value was empty, then the given template one is used as the value to increase.
    /// </summary>
    /// <param name="templateIfEmpty"></param>
    /// <returns></returns>
    public SemanticRelease Increase(string templateIfEmpty = "")
    {
        var value = Value; if (value.Length == 0)
        {
            var template = new SemanticRelease(templateIfEmpty);
            value = template.Value;
        }
        var parts = value.Split('.');
        var temp = parts[^1];

        // We may have a last numeric chunk we can increase...
        var index = FirstDigit(temp);
        if (index >= 0)
        {
            var num = temp[index..];
            temp = temp[..index];

            var item = int.TryParse(num, out var r) ? r : 0;
            item++;

            var len = num.Length;
            num = item.ToString($"D{len}");
            temp += num;
        }

        // No last numeric chunk...
        else temp += "1";

        // Finishing...
        parts[^1] = temp;
        return string.Join('.', parts);
    }

    // The index of the first digit in the last numeric part...
    static int FirstDigit(string value)
    {
        var r = -1; for (int i = value.Length - 1; i >= 0; i--)
        {
            if (char.IsDigit(value[i])) r = i;
            else break;
        }
        return r;
    }

    // ------------------------------------------------

    /// <summary>
    /// Compares the two given instances and returns an integer that indicates whether the first
    /// one precedes, follows, or occurs in the same position in sort order as the second one.
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
    }

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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(SemanticRelease? other) => Compare(this, other);

    public static bool operator >(SemanticRelease? x, SemanticRelease? y) => Compare(x, y) > 0;
    public static bool operator <(SemanticRelease? x, SemanticRelease? y) => Compare(x, y) < 0;
    public static bool operator >=(SemanticRelease? x, SemanticRelease? y) => Compare(x, y) >= 0;
    public static bool operator <=(SemanticRelease? x, SemanticRelease? y) => Compare(x, y) <= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(SemanticRelease? other) => Compare(this, other) == 0;

    /// <summary>
    /// <inheritdoc/> This method only takes into consideration the version value carried by
    /// this instance, and not its metadata one.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => Value.GetHashCode();
}