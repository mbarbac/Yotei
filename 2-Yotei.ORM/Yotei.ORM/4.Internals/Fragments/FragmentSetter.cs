using System.Windows.Markup;

namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing SETTER clauses.
/// <br/>- Standard syntax: 'x => Target = Value'.
/// </summary>
public static partial class FragmentSetter
{
    readonly static string CLAUSE = "SETTER";

    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments used to build a SETTER clause.
    /// </summary>
    [Cloneable]
    public partial class Entry : Fragment.Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="body"></param>
        public Entry(DbTokenLiteral body) : base() => Body = body.ThrowWhenNull();

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="body"></param>
        public Entry(DbTokenSetter body) : base() => Body = body.ThrowWhenNull();

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Entry(Entry source) : base(source) => Body = source.Body;

        /// <inheritdoc/>
        public override string ToString() => Body.ToString()!;

        /// <summary>
        /// The actual contents carried by this instance.
        /// <br/> Only <see cref="DbTokenLiteral"/> and <see cref="DbTokenSetter"/> objects are
        /// allowed.
        /// </summary>
        public IDbToken Body { get; }

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit(DbTokenVisitor visitor, bool first, bool last)
        {
            var builder = visitor.Visit(Body);
            return builder;
        }

        /*
        /// <summary>
        /// Visits the name part of this instance.
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        public ICommandInfo.IBuilder VisitName(DbTokenVisitor visitor, bool first, bool last)
        {
            if (Body is DbTokenSetter setter)
            {
                var builder = visitor.Visit(setter.Target);
                return builder;
            }
            else
            {
                throw null;
            }
        }*/

        /*
        /// <summary>
        /// Visits the value part of this instance.
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        public ICommandInfo.IBuilder VisitValue(DbTokenVisitor visitor, bool first, bool last)
        {
            if (Body is DbTokenSetter setter)
            {
                var builder = visitor.Visit(setter.Value);
                return builder;
            }
            else
            {
                throw null;
            }
        }*/
    }

    // ====================================================
    /// <summary>
    /// Represents the collection of fragments used to build a head or tail SETTER clause.
    /// </summary>
    [Cloneable]
    public partial class Master : Fragment.Master
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command"></param>
        public Master(ICommand command) : base(command) { }

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
                $"Entry is not a valid {CLAUSE} one.")
                .WithData(entry);

            return valid;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override Entry Create(IDbToken body)
        {
            // Finishing...
            if (body is DbTokenInvoke invoke &&
                invoke.Arguments.Count == 1 &&
                invoke.Arguments[0] is DbTokenLiteral literal) body = literal;

            return body switch
            {
                DbTokenLiteral temp => new(temp),
                DbTokenSetter temp => new(temp),

                _ => throw new ArgumentException(
                    $"Specification does not resolve into a valid {CLAUSE} clause.")
                    .WithData(body)
            };
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override string? Separator => ", ";

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit()
        {
            static ICommandInfo.IBuilder Itemize(
                Fragment.Entry entry, DbTokenVisitor visitor, bool first, bool last)
            {
                var valid = (Entry)entry;
                var builder = valid.Visit(visitor, first, last);
                var str = builder.Text.UnWrap('(', ')').Wrap('(', ')');

                builder.ReplaceText(str);
                return builder;
            }

            return Visit(Itemize);
        }

        /*
        /// <summary>
        /// Visits the names of this instance.
        /// </summary>
        /// <returns></returns>
        public ICommandInfo.IBuilder VisitNames()
        {
            static ICommandInfo.IBuilder Itemize(
                Fragment.Entry entry, DbTokenVisitor visitor, bool first, bool last)
            {
                var valid = (Entry)entry;
                var builder = valid.VisitName(visitor, first, last);
                return builder;
            }

            return Visit(Itemize);
        }*/

        /*
        /// <summary>
        /// Visits the names of this instance.
        /// </summary>
        /// <returns></returns>
        public ICommandInfo.IBuilder VisitValues()
        {
            static ICommandInfo.IBuilder Itemize(
                Fragment.Entry entry, DbTokenVisitor visitor, bool first, bool last)
            {
                var valid = (Entry)entry;
                var builder = valid.VisitValue(visitor, first, last);
                return builder;
            }

            return Visit(Itemize);
        }*/
    }
}