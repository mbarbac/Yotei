namespace Yotei.ORM.Code;
partial class Identifier
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="IIdentifier.IBuilder"/>
    /// </summary>
    [Cloneable]
    public partial class Builder : IIdentifier.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => throw null;

        /// <summary>
        /// Initializes a new instance with the parts obtained from the given range of values.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(IEngine engine, IEnumerable<string?> range) => throw null;

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => throw null;

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEngine Engine { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string? Value { get => throw null; set => throw null; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Count { get => throw null; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string? this[int index] { get => throw null; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="useTerminators"></param>
        /// <returns></returns>
        public string? this[int index, bool useTerminators] { get => throw null; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="useTerminators"></param>
        /// <returns></returns>
        public IEnumerable<string?> GetParts(bool useTerminators) => throw null;

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public bool Contains(string? part) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public int IndexOf(string? part) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public int LastIndexOf(string? part) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public List<int> IndexesOf(string? part) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Contains(Predicate<string?> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int IndexOf(Predicate<string?> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int LastIndexOf(Predicate<string?> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<int> IndexesOf(Predicate<string?> predicate) => throw null;

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual IIdentifier ToInstance() => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual int Replace(int index, string? value) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual int Add(string? value) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual int AddRange(IEnumerable<string?> range) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual int Insert(int index, string? value) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual int InsertRange(int index, IEnumerable<string?> range) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual int RemoveAt(int index) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual int RemoveRange(int index, int count) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public virtual int Remove(string? part) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public virtual int RemoveLast(string? part) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public virtual int RemoveAll(string? part) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int Remove(Predicate<string?> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int RemoveLast(Predicate<string?> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int RemoveAll(Predicate<string?> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual int Clear() => throw null;
    }
}