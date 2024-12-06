using System.Runtime.InteropServices.Marshalling;

namespace Runner;

// ========================================================
/// <summary>
/// Represents the optional pre-release portion of a semantic version specification, which is
/// composed by its value and its metadata.
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
    public SemanticRelease(string source)
    {
        source = source.ThrowWhenNull();
        if (source.Length > 0) Value = source;
    }

    /// <inheritdoc/>
    public override string ToString() => ToString(false);

    /// <summary>
    /// Returns a string representation of this instance, with or without a leading hypen as
    /// requested.
    /// </summary>
    /// <param name="hypen"></param>
    /// <returns></returns>
    public string ToString(bool hypen)
    {
        if (Value.Length == 0 && Metadata.Length == 0) return string.Empty;
        else
        {
            var sb = new StringBuilder();
            
            if (Value.Length > 0)
            {
                if (hypen) sb.Append('-');
                sb.Append(Value);
            }
            if (Metadata.Length > 0) sb.Append($"+{Metadata}");
            return sb.ToString();
        }
    }

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
    /// The value of the pre-release version, without its preceding hypen, or an empty string.
    /// The init setter removes the leading hypen characters, if any, and accepts optional
    /// metadata separated by the first plus character.
    /// </summary>
    public string Value
    {
        get => _Value;
        init
        {
            _Value = Validate(value, out _, out var metadata);
            if (metadata.Length > 0) Metadata = metadata;
        }
    }
    string _Value = string.Empty;

    /// <summary>
    /// The build metadata value, without its preceding plus character, or an empty string. The
    /// init setter removes the leading plus characters, if any.
    /// </summary>
    public string Metadata
    {
        get => _Metadata;
        init
        {
            value = value.ThrowWhenNull();

            var plus = false; while (value.Length > 0)
            {
                if (value[0] == '+')
                {
                    plus = true;
                    value = value[1..];
                }
                else
                {
                    plus = false;
                    break;
                }
            }
            if (plus) throw new ArgumentException(
                "Metadata cannot consist on just plus character(s).")
                .WithData(value);

            if (value.Length > 0)
            {
                var parts = value.Split('.');
                foreach (var part in parts) Validate(part);
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
    /// Invoked to obtain the pre-release parts and metadata from the given value.
    /// </summary>
    static string Validate(string value, out string[] parts, out string metadata)
    {
        value = value.ThrowWhenNull();

        parts = [];
        metadata = string.Empty;

        var hypen = false; while (value.Length > 0)
        {
            if (value[0] == '-')
            {
                hypen = true;
                value = value[1..];
            }
            else
            {
                hypen = false;
                break;
            }
        }
        if (hypen) throw new ArgumentException(
            "Parts cannot consist on just hypen character(s).")
            .WithData(value);

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
                Validate(part);
                NoLeadingZeroes(part);
            }
        }

        return value;
    }

    /// <summary>
    /// Invoked to validate the given part has no leasing zeroes.
    /// </summary>
    static void NoLeadingZeroes(string part)
    {
        if (part.Length > 1 &&
            part[0] == '0' &&
            part.All(char.IsDigit))
            throw new ArgumentException(
                "Leading zeroes are not allowed in pre-release parts.")
                .WithData(part);
    }

    /// <summary>
    /// Invoked to validate the given part.
    /// </summary>
    static void Validate(string part)
    {
        part.NotNullNotEmpty();

        if (part[0] == '-') throw new ArgumentException(
            "Leading '-' character is not allowed in pre-release parts.")
            .WithData(part);

        if (part[0] == '+') throw new ArgumentException(
            "Leading '+' character is not allowed in pre-release parts.")
            .WithData(part);

        foreach (var c in part)
            if (!Validate(c)) throw new ArgumentException(
                "Pre-release part carries invalid characters.")
                .WithData(c)
                .WithData(part);
    }

    /// <summary>
    /// Invoked to validate that the given char belongs to an acceptable range.
    /// </summary>
    static bool Validate(char c) =>
        c is '-' or '+' or
        >= '0' and <= '9' or
        >= 'A' and <= 'Z' or
        >= 'a' and <= 'z';

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the original value increased, and the original metadata
    /// cleared. Note that the new value might be the same as the original one if it was an empty
    /// one, or not a trailing numeric one. In any case, the build metadata has been cleared.
    /// </summary>
    /// <returns></returns>
    public SemanticRelease Increase() => Increase(out _);

    /// <summary>
    /// Returns a new instance with the original value increased, and the original metadata
    /// cleared. The out argument determines whether the value has been increased, or not, for
    /// instance because it was an empty one or not a trailing numeric one.
    /// </summary>
    /// <param name="done"></param>
    /// <returns></returns>
    public SemanticRelease Increase(out bool done)
    {
        // Returning a new version with the metadata cleared...
        if (Value.Length == 0)
        {
            done = false;
            return new(Value);
        }

        // Preparing...
        var parts = Value.Split('.');
        var temp = parts[^1];

        // We may have a last numeric chunk to increase
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

            parts[^1] = temp;
            done = true;
            return string.Join('.', parts);
        }

        // Or there is not a trailing numeric part...
        else
        {
            done = false;
            return new(Value);
        }
    }

    /// <summary>
    /// Gets the index of the first digit in the last numeric part, or -1 if any.
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

    // ----------------------------------------------------

    /// <summary>
    /// Compares the two given instances and returns an integer that indicates whether the first
    /// one precedes, follows, or occurs in the same sort order position as the second one.
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

        /// <summary>
        /// Invoked to compare the two given parts.
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

            if (nx) return -1;
            if (ny) return +1;

            return string.Compare(xpart, ypart);
        }
    }

    /// <inheritdoc/>
    public int CompareTo(SemanticRelease? other) => Compare(this, other);

    public static bool operator >(SemanticRelease? x, SemanticRelease? y) => Compare(x, y) > 0;
    public static bool operator <(SemanticRelease? x, SemanticRelease? y) => Compare(x, y) < 0;
    public static bool operator >=(SemanticRelease? x, SemanticRelease? y) => Compare(x, y) >= 0;
    public static bool operator <=(SemanticRelease? x, SemanticRelease? y) => Compare(x, y) <= 0;

    /// <inheritdoc/>
    public virtual bool Equals(SemanticRelease? other) => Compare(this, other) == 0;

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode();
}