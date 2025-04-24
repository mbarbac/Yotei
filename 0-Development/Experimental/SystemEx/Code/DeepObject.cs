namespace Experimental.SystemEx;

// ========================================================
/// <summary>
/// Represents an object whose properties can be dynamically set, where each of them can also
/// be a nested deep object themselves.
/// </summary>
public class DeepObject : DynamicObject
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="caseSensitiveNames"></param>
    public DeepObject(
        bool caseSensitiveNames = true) => DeepMetadata = new(this, caseSensitiveNames);

    /// <inheritdoc/>
    public override string ToString() => DeepMetadata.ToString();

    /// <summary>
    /// The metadata carried by this instance.
    /// </summary>
    public DeepObjectMetadata DeepMetadata { get; }

    // ----------------------------------------------------

    readonly BindingFlags _Flags =
        BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.FlattenHierarchy;

    MethodInfo[] _Methods = null!;

    /// <summary>
    /// Prevents adding dynamic members whose names are the same as methods in this instance.
    /// </summary>
    /// <param name="name"></param>
    void InterceptMethodName(string name)
    {
        var type = GetType();
        var methods = _Methods ??= type.GetMethods(_Flags);

        var method = methods.FirstOrDefault(
            x => string.Compare(name, x.Name, !DeepMetadata.CaseSensitiveNames) == 0);

        if (method != null) throw new DuplicateException(
            "A method exist in this instance with the requested member name.")
            .WithData(name)
            .WithData(type);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override IEnumerable<string> GetDynamicMemberNames() =>
       DeepMetadata.Members.Select(x => x.DeepMetadata.Name!);

    /// <inheritdoc/>
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        var member = DeepMetadata.FindMember(binder.Name);

        if (member == null)
        {
            result = DeepMetadata.AddMember(binder.Name);
        }
        else result = member.DeepMetadata.HasValue ? member.DeepMetadata.Value : member;

        return true;
    }

    /// <inheritdoc/>
    public override bool TryGetIndex(GetIndexBinder binder, object?[] indexes, out object? result)
    {
        var member = DeepMetadata.FindIndexed(indexes);

        if (member == null)
        {
            result = DeepMetadata.AddIndexed(indexes);
        }
        else result = member.DeepMetadata.HasValue ? member.DeepMetadata.Value : member;

        return true;
    }

    /// <inheritdoc/>
    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        var member = DeepMetadata.FindMember(binder.Name);

        member ??= DeepMetadata.AddMember(binder.Name);

        member.DeepMetadata.Value = value;
        return true;
    }

    /// <inheritdoc/>
    public override bool TrySetIndex(
       SetIndexBinder binder, object?[] indexes, object? value)
    {
        var member = DeepMetadata.FindIndexed(indexes);

        member ??= DeepMetadata.AddIndexed(indexes);

        member.DeepMetadata.Value = value;
        return true;
    }

    /// <inheritdoc/>
    public override bool TryConvert(ConvertBinder binder, out object? result)
    {
        result = DeepMetadata.HasValue ? DeepMetadata.Value : this;
        result = result.ConvertTo(binder.ReturnType);

        return true;
    }

    // ====================================================
    /// <summary>
    /// Maintains the metadata carried by a given <see cref="DeepObject"/> instance.
    /// </summary>
    public sealed class DeepObjectMetadata
    {
        readonly DeepObject _Master;
        readonly bool _CaseSensitiveNames;
        readonly List<DeepObject> _Members = [];

        bool _HasValue;
        object? _Value;
        string? _Name;
        bool _IsIndexed;
        DeepObject? _Parent;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="master"></param>
        /// <param name="caseSensitiveNames"></param>
        internal DeepObjectMetadata(DeepObject master, bool caseSensitiveNames)
        {
            _Master = master.ThrowWhenNull();
            _CaseSensitiveNames = caseSensitiveNames;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append('(');

            sb.Append(Name ?? ".");

            if (HasValue)
            {
                var value = _Value.Sketch();
                sb.Append($"='{value}'");
            }

            if (Count != 0)
            {
                sb.Append('['); sb.Append(string.Join(", ", _Members));
                sb.Append(']');
            }

            sb.Append(')');
            return sb.ToString();
        }

        // ------------------------------------------------

        /// <summary>
        /// Gets or sets the value carried by this member, including null ones. The getter throws
        /// an exception if that instance is just an structural one that carries no value.
        /// </summary>
        public object? Value
        {
            get
            {
                if (!HasValue) throw new InvalidOperationException(
                    "This member carries no value.")
                    .WithData(this);

                return _Value;
            }

            set
            {
                _Value = value;
                _HasValue = true;
            }
        }

        /// <summary>
        /// Determines whether this member, when acting as a dynamic property, carries a value
        /// or not.
        /// </summary>
        public bool HasValue => _HasValue && _Members.Count == 0;

        /// <summary>
        /// Clears any value carried by this member.
        /// </summary>
        public void ClearValue()
        {
            _Value = null;
            _HasValue = false;
        }

        /// <summary>
        /// Whether the names of the dynamic members carried by this one are considered case
        /// sensitive or not.
        /// </summary>
        public bool CaseSensitiveNames => _CaseSensitiveNames;

        /// <summary>
        /// The name of this member, or null if it is not acting as a dynamic one.
        /// </summary>
        public string? Name => _Name;

        /// <summary>
        /// The full member name of this instance.
        /// </summary>
        public string? FullName => Parent == null
            ? Name
            : IsIndexed
            ? $"{Parent.DeepMetadata.FullName}{Name}"
            : $"{Parent.DeepMetadata.FullName}.{Name}";


        /// <summary>
        /// Determines if this member, when acting as a dynamic property, is an indexed one
        /// or not.
        /// </summary>
        public bool IsIndexed => _IsIndexed;

        /// <summary>
        /// The parent instance of this member, or null if this member is a root instance that
        /// does not belong to any parent.
        /// </summary>
        public DeepObject? Parent => _Parent;

        /// <summary>
        /// The collection of dynamic members carried by this instance.
        /// </summary>
        public IEnumerable<DeepObject> Members => _Members;

        /// <summary>
        /// The actual number of dynamic members carried by this instance.
        /// </summary>
        public int Count => _Members.Count;

        // ------------------------------------------------

        /// <summary>
        /// Validates the given member name.
        /// </summary>
        internal static string ValidateName(string name)
        {
            name = name.NotNullNotEmpty();

            if (name.ContainsAny(".[]")) throw new ArgumentException(
               "Member name cannot contain invalid characters.")
               .WithData(name);

            return name;
        }

        /// <summary>
        /// Returns a member name built using the given indexes.
        /// </summary>
        internal static string IndexesToName(params object?[] args)
        {
            args ??= new object?[] { null };
            if (args.Length == 0) throw new EmptyException("Array of indexes is empty.");

            var sb = new StringBuilder();
            sb.Append('[');

            var names = args.Select(x => x.Sketch());
            sb.Append(string.Join(", ", names));

            sb.Append(']');
            return sb.ToString();
        }

        // ------------------------------------------------

        /// <summary>
        /// Private find method.
        /// </summary>
        DeepObject? Find(string name) => _Members.Find(x =>
            string.Compare(name, x.DeepMetadata.Name, !CaseSensitiveNames) == 0);

        /// <summary>
        /// Returns the member with the given name, or null if any can be found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DeepObject? FindMember(string name)
        {
            name = ValidateName(name);
            return Find(name);
        }

        /// <summary>
        /// Returns the indexed member with the given indexes, or null if any can be found.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public DeepObject? FindIndexed(params object?[] args)
        {
            var name = IndexesToName(args);
            return Find(name);
        }

        /// <summary>
        /// Adds into this instance a new dynamic member with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DeepObject AddMember(string name)
        {
            name = ValidateName(name);
            _Master.InterceptMethodName(name);

            var member = Find(name);
            if (member != null) throw new DuplicateException(
                "A member with the given name already exists.")
                .WithData(name);

            member = new(CaseSensitiveNames);
            member.DeepMetadata._Name = name;
            _Members.Add(member);
            return member;
        }

        /// <summary>
        /// Adds into this instance a new dynamic indexed member with the given indexes.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public DeepObject AddIndexed(params object?[] args)
        {
            var name = IndexesToName(args);
            var member = Find(name);
            if (member != null) throw new DuplicateException(
                "An indexed member with the given indexes already exists.")
                .WithData(args);

            member = new(CaseSensitiveNames);
            member.DeepMetadata._Name = name;
            member.DeepMetadata._IsIndexed = true;
            _Members.Add(member);
            return member;
        }

        /// <summary>
        /// Removes the given dynamic member from this instance.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool RemoveMember(DeepObject member)
        {
            member.ThrowWhenNull();

            var r = _Members.Remove(member);
            if (r) member.DeepMetadata._Parent = null;

            return r;
        }

        /// <summary>
        /// Removes all members from this instance.
        /// </summary>
        public void ClearMembers() => _Members.Clear();
    }
}