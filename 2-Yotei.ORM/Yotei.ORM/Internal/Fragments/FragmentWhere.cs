using System.Security.Cryptography;

namespace Yotei.ORM.Internal;

public partial class FragmentWhere
{
    // ====================================================
    /// <summary>
    /// Represents an entry in a collection of fragments that is used to build a WHERE clause.
    /// </summary>
    [Cloneable]
    public partial class Entry : Fragment.Entry
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public Entry(bool useOr, DbToken body) : base(body) => UseOr = useOr;

        /// <summary>
        /// Determines if this instance chains with any previous ones using an 'OR' connector,
        /// or rather it shall use the default 'AND' one.
        /// </summary>
        public bool UseOr { get; }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Entry(Entry source) : base(source) { }
    }

    // ====================================================
    /// <summary>
    /// Represents a list-alike collection of fragments used to build a WHERE clause.
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
        public Master(Master source) : base(source) { }

        /// <inheritdoc/>
        protected override void Validate(Fragment.Entry item)
        {
            if (item is not Entry) throw new ArgumentException(
                "Fragment entry is not a WHERE related one.")
                .WithData(item);
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        protected override Fragment.Entry Create(DbTokenInvoke? head, DbToken body, DbTokenInvoke? tail)
        {
            var useOr = false;

            // Intercepting heading 'And' or 'Or' methods...
            body = body.RemoveFirst(x =>
            {
                if (x is not DbTokenMethod method) return false;
                if (method.Host is not DbTokenArgument) return false;
                if (method.Name.ToUpper() is not "AND" and not "OR") return false;

                return true;
            },
            out var removed); if (removed != null)
            {
                var method = (DbTokenMethod)removed;

                useOr = method.Name.Equals("OR", StringComparison.OrdinalIgnoreCase);

                if (method.Arguments.Count == 0)
                { }
                else if (method.Arguments.Count == 1)
                {
                    if (body is not DbTokenArgument) throw new ArgumentException(
                        $"Remaining body after '{method.Name}(...)' must be empty.")
                        .WithData(body);

                    body = method.Arguments[0];
                }
                else
                {
                    throw new ArgumentException(
                        $"Too many arguments in '{method.Name}(...)' method.")
                        .WithData(body);
                }
            }

            // Validating...
            switch (body)
            {
                case DbTokenArgument: break;
                case DbTokenBinary: break;

                default:
                    throw new ArgumentException(
                        "Specification does not resolve into a binary-alike token.")
                        .WithData(body);
            }

            // Finishing...
            return new Entry(useOr, body) { Head = head, Tail = tail };
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public override ICommandInfo.IBuilder Visit()
        {
            throw null;
        }
    }
}