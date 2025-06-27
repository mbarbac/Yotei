namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing SELECT clauses.
/// <br/>- Standard syntax: 'x => x.Element'.
/// <br/>- Alternate syntax: 'x => x.Element.As(...)'.
/// <br/>- Alternate syntax: 'x => x.Source.All()'.
/// </summary>
/// <remarks>
/// SELECT clauses accept complex specifications.
/// <br/>- Example: 'SELECT SUM([Amount]) AS TotalAmount, ...'
/// </remarks>
public static partial class FragmentSelect
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build SELECT-alike clauses.
    /// </summary>
    [Cloneable]
    public partial class Entry : Fragment.Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="master"></param>
        /// <param name="body"></param>
        public Entry(Master master, IDbToken body) : base(master, body)
        {
            if (body is DbTokenLiteral literal)
            {
                var str = literal.Value.Trim();

                // Extracting all columns, if requested...
                var (strAll, other) = str.ExtractLeftRight(".*", Engine, out var foundAll);
                if (foundAll)
                {
                    AllColumns = true;
                    str = strAll + other;
                }

                // Extracting alias, if any...
                var (main, alias) = str.ExtractMainAlias(Engine, out var foundAs);
                if (foundAs)
                {
                    var comparison = Engine.CaseSensitiveNames
                        ? StringComparison.Ordinal
                        : StringComparison.OrdinalIgnoreCase;

                    var wrapped = Engine.UseTerminators &&
                        alias!.StartsWith(Engine.LeftTerminator.ToString(), comparison) &&
                        alias!.EndsWith(Engine.RightTerminator.ToString(), comparison);

                    var id = new IdentifierPart(Engine, alias);

                    Alias = wrapped ? id.Value : id.UnwrappedValue;
                    str = main;
                }

                // Finishing...
                if (foundAll || foundAs) Body = new DbTokenLiteral(str.NotNullNotEmpty());
            }
        }
        /*
            
            {
                // Invalid literals not intercepted afterwards...
                string main = literal.Value;
                if (main.EndsWith(" OR", StringComparison.OrdinalIgnoreCase) ||
                    main.EndsWith(" AND", StringComparison.OrdinalIgnoreCase))
                    throw new EmptyException("Source ends with a connector.").WithData(main);

                // Extracting connector from literal sources, if any...
                if (main.Length > 0 && main[0] == ' ') main = main[1..];

                string connector;
                UseOR = null;
                connector = "OR "; if (ExtractFirst(ref main, ref connector, false)) UseOR = true;
                connector = "AND "; if (ExtractFirst(ref main, ref connector, false)) UseOR = false;
                if (UseOR is not null) main = main.NotNullNotEmpty();

                // Transforming 'target==value' into proper 'target=value' SQL...
                var (target, value) = main.ExtractLeftRight("==", Engine, out var found);
                if (found)
                {
                    target = Adjust(target);
                    value = Adjust(value!);
                    main = $"({target} = {value})";

                    string Adjust(string source)
                    {
                        if (source is not null)
                        {
                            if (source.StartsWith('(') && !source.EndsWith(')')) source = source[1..];
                            if (!source.StartsWith('(') && source.EndsWith(')')) source = source[..^1];
                        }
                        return source.NotNullNotEmpty(trim: true);
                    }
                }

                // Finalizing...
                if (UseOR is not null || found) Body = new DbTokenLiteral(main);
            }

            // Using 'target=value' instead of the right 'target==value' one...
            else if (body is DbTokenSetter setter)
            {
                Body = new DbTokenBinary(setter.Target, ExpressionType.Equal, setter.Value);
            }
        }*/

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source)
        {
            AllColumns = source.AllColumns;
            Alias = source.Alias;
        }

        /// <summary>
        /// Determines if all columns from the given source shall be selected, as in 'Table.*'.
        /// </summary>
        public bool AllColumns
        {
            get => _AllColumns;
            init => _AllColumns = value;
        }
        internal bool _AllColumns;

        /// <summary>
        /// The alias that qualifies the source (Body), or null if any.
        /// </summary>
        public string? Alias
        {
            get => _Alias;
            init => _Alias = value?.NotNullNotEmpty();
        }
        internal string? _Alias;

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string ToString()
        {
            var str = Body.ToString()!;
            if (AllColumns) str += ".*";
            if (Alias is not null) str += $" AS {Alias}";
            return str;
        }

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor)
        {
            var builder = visitor.Visit(Body);

            if (AllColumns) builder.ReplaceText($"{builder.Text}.*");
            if (Alias is not null) builder.ReplaceText($"{builder.Text} AS {Alias}");
            return builder;
        }
    }

    // ====================================================
    /// <summary>
    /// Represents a collection of fragments used to build SELECT-alike clauses.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        public Master(ICommand command) : base(command, "SELECT") { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Master(Master source) : base(source) { }

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override Entry Validate(Fragment.Entry entry)
        {
            if (entry is not Entry valid) throw new ArgumentException(
                $"Entry is not a valid {Clause} one.")
                .WithData(entry);

            return valid;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override Entry CreateEntry(IDbToken body)
        {
            // Intercepting 'All()' method...
            bool allcolumns = false;
            var item = body.RemoveFirst(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Name.ToUpper() is not "ALL") return false;
                return true;
            }
            , out var removed);

            if (removed is not null) // Found...
            {
                var method = (DbTokenMethod)removed;
                var name = method.Name;
                var upper = name.ToUpper();

                if (method.TypeArguments.Length != 0) throw new ArgumentException(
                    $"No type arguments allowed for '{upper}()' virtual method.")
                    .WithData(body);

                if (method.Arguments.Count != 0) throw new ArgumentException(
                    $"'{upper}()' must be a parameterless method.")
                    .WithData(body);

                if (item is DbTokenArgument) throw new ArgumentException(
                    $"Remaining after '{upper}()' cannot be empty.")
                    .WithData(body);

                allcolumns = true;
                body = item;
            }

            // Intercepting 'As()' method...
            string? alias = null;
            item = body.RemoveFirst(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Name.ToUpper() is not "AS") return false;
                return true;
            }
            , out removed);

            if (removed is not null) // Found...
            {
                var method = (DbTokenMethod)removed;
                var name = method.Name;
                var upper = name.ToUpper();

                if (method.TypeArguments.Length != 0) throw new ArgumentException(
                    $"No type arguments allowed for '{upper}()' virtual method.")
                    .WithData(body);

                if (method.Arguments.Count == 0) throw new ArgumentException(
                    $"'{upper}()' canot be a parameterless method.")
                    .WithData(body);

                if (item is DbTokenArgument) throw new ArgumentException(
                    $"Remaining after '{upper}()' cannot be empty.")
                    .WithData(body);

                var visitor = Connection.Records.CreateDbTokenVisitor(Command.Locale);
                alias = visitor.ChainToAlias(method.Arguments);
                body = item;
            }

            // Finishing...
            var entry = new Entry(this, body);
            if (allcolumns) entry._AllColumns = true;
            if (alias is not null) entry._Alias = alias;
            return entry;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override string? EntrySeparator(Fragment.Entry entry) => ", ";
    }
}