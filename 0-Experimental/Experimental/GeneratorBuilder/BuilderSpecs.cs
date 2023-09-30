namespace Experimental;

// ========================================================
/// <summary>
/// Represents a set of builder specifications with the format '[name][(...)][[+|-]optionals]',
/// where:
/// <br/>- [name] is the name of the builder method(s) to consider. If it is null, then only the
/// type constructors will be considered. Otherwise, the name of the method(s) that returns an
/// instance compatible with the value of the associated type.
/// <br/>- [(...)] is the comma-separated list of builder arguments. If not used, or if its value
/// is '(*)', then all possible argumens will be taken into consideration. If it is '()', then
/// only parameterless builders will be considered. Otherwise, the list of specs with the format
/// '[name][=[@|member][!]]', where:
/// <br/>.[name] is the name of the builder parameter,
/// <br/>.[=...] specifies the source of the value to use for that parameter, if null then a
/// member with a corresponding name will be found, if [member] a member with a matching name
/// will be found, and if [@] then the value is obtained from an external enforced variable. In
/// any case, if '!' is used then the value to use will be a clone of the given one.
/// <br/>- [+|-[*|spec]] is comma-separated chain of optional init/set arguments. If not used or
/// if '+*', erases previous specs and then all possible ones are considered. If '-*', erases all
/// previous specs. Otherwise the chain of specifications that follows the same rules as the ones
/// for the arguments.
/// </summary>
internal class BuilderSpecs
{
    /// <summary>
    /// Contains a given set of builder specifications.
    /// </summary>
    /// <param name="specs"></param>
    public BuilderSpecs(string? specs)
    {
        specs = specs.NullWhenEmpty();
        if (specs == null) return;

        int n = 0;
        int m = 0;
        string s = default!;

        // Case: we have an argument spec...
        n = specs.IndexOf('('); if (n >= 0)
        {
            Name = specs.Substring(0, n).Trim();
            TheArguments.Clear();

            m = specs.IndexOf(')', n); if (m >= 0)
            {
                s = specs.Substring(n + 1, m - n - 1).Trim();
                if (s.Length > 0)
                {
                    var parts = s.Split(',');
                    foreach (var part in parts) AddArgument(new(part));
                }

                specs = specs.Substring(m + 1);
            }
            else throw new ArgumentException(
                "Not matching ')' specification.")
                .WithData(specs, nameof(specs));
        }

        // Case: no arguments spec...
        else
        {
            n = specs.IndexOfAny(new[] { '+', '-' });
            if (n < 0)
            {
                Name = specs;
                return;
            }
            Name = specs.Substring(0, n).Trim();
            specs = specs.Substring(n);
        }

        // Parsing optionals...
        if (specs.Contains("+")) TheOptionals.Clear();

        n = 0; while (n < specs.Length)
        {
            n = specs.IndexOfAny(new[] { '+', '-' }, n);
            if (n < 0) break;

            m = specs.IndexOfAny(new[] { '+', '-' }, n + 1);
            if (m < 0)
            {
                s = specs.Substring(n);
                if (s.Length <= 1) throw new ArgumentException(
                    "Empty optional specification found.")
                    .WithData(specs, nameof(specs));

                AddOptional(new(s));
                return;
            }
            else
            {
                s = specs.Substring(n, m - n);
                if (s.Length <= 1) throw new ArgumentException(
                    "Empty optional specification found.")
                    .WithData(specs, nameof(specs));

                AddOptional(new(s));
                n = m;
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(Name);

        sb.Append("(");
        if (TheArguments.Count > 0) sb.Append(string.Join(",", Arguments));
        sb.Append(")");

        sb.Append(string.Concat(Optionals));

        return sb.ToString();
    }

    /// <summary>
    /// The name of the builder method(s) to use. If it is empty, then only the type constructors
    /// will be taken into consideration.
    /// </summary>
    public string Name { get; } = string.Empty;

    /// <summary>
    /// The collection of builder argument specifications. If it is an empty one, then only the
    /// parameterless builders will be considered. If the value of its single element is "*",
    /// then all builders will be considered despite their actual arguments.
    /// </summary>
    public IEnumerable<BuilderArgument> Arguments => TheArguments;
    List<BuilderArgument> TheArguments = new() { new("*") };

    /// <summary>
    /// The collection of optional init/set argument specifications. By default this collection
    /// contains a single element that indicates that all possible ones will be considered.
    /// </summary>
    public IEnumerable<BuilderOptional> Optionals => TheOptionals;
    List<BuilderOptional> TheOptionals = new() { new("+*") };

    // ----------------------------------------------------

    /// <summary>
    /// Adds the given argument specification to this instance. If it is a '*', then it must be
    /// an unique one in the collection of arguments.
    /// </summary>
    /// <param name="item"></param>
    public void AddArgument(BuilderArgument item)
    {
        item = item.ThrowWhenNull(nameof(item));

        if (item.Name == "*")
        {
            if (TheArguments.Count > 0)
                throw new ArgumentException("The '*' argument specification must be unique.");

            TheArguments.Add(item);
        }
        else
        {
            if (TheArguments.Any(x => x.Name == "*"))
                throw new ArgumentException("The '*' argument specification must be unique.");

            TheArguments.Add(item);
        }
    }

    /// <summary>
    /// Clears the collection of argument specifications.
    /// </summary>
    public void ClearArguments() => TheArguments.Clear();

    /// <summary>
    /// Adds the given optional init/set specification to this instance. If it is '-*' or '+*',
    /// then the original collection is cleared before adding the given one (only if '+*').
    /// </summary>
    /// <param name="item"></param>
    public void AddOptional(BuilderOptional item)
    {
        item = item.ThrowWhenNull(nameof(item));

        if (item.Name == "*")
        {
            TheOptionals.Clear();
            if (item.IsInclude) TheOptionals.Add(item);
        }
        else
        {
            TheOptionals.Add(item);
        }
    }

    /// <summary>
    /// Clears the collection of optional init/set specifications.
    /// </summary>
    public void ClearOptionals() => TheOptionals.Clear();
}