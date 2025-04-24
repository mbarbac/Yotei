#pragma warning disable CS8618

namespace Experimental.WeakObjects;

// ========================================================
/// <summary>
/// Represents a holder for an underlying reference that can be weakened so that it can be
/// garbage collected as needed.
/// </summary>
/// <typeparam name="T"></typeparam>
public class WeakHolder<T> : IWeakObject where T : class
{
    T? _Hard = null;
    WeakReference _Weak;
    DateTime _LastUsed;
    string _Value = default!;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="target"></param>
    public WeakHolder(T target) => Update(target);

    /// <inheritdoc/>
    public override string ToString() => $"{State}:({Value})";

    string State =>
        _Hard != null ? "Hard" :
        _Weak.IsAlive ? "Weak" : "Collected";

    string Value =>
        _Hard == null ? (_Value = ComputeValue(_Hard)) :
        _Weak.IsAlive ? (_Value = ComputeValue((T?)_Weak.Target)) :
        _Value;

    string ComputeValue(T? item) =>
        item == null ? _Value :
        item?.ToString() ?? _Value;        

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool IsValid => _Weak.IsAlive;

    /// <inheritdoc/>
    public DateTime LastUsed => _LastUsed;

    /// <summary>
    /// Returns the underlying reference kept by this instance, or null if it has been garbage
    /// collected, hydrating it and updating its <see cref="LastUsed"/> property.
    /// </summary>
    public T? Target
    {
        get
        {
            _Hard ??= WeakTarget;
            if (_Hard != null) _LastUsed = DateTime.UtcNow;

            return _Hard;
        }
    }

    /// <summary>
    /// Returns the underlying reference kept by this instance, or null if it has been garbage
    /// collected, without hydrating it.
    /// </summary>
    public T? WeakTarget => (T?)_Weak.Target;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public T? Hydrate() => Target;
    void IWeakObject.Hydrate() => Hydrate();

    /// <inheritdoc/>
    public void Weaken() => _Hard = null;

    /// <inheritdoc/>
    public void Weaken(TimeSpan timeout)
    {
        Validate(timeout);

        var now = DateTime.UtcNow;
        var end = LastUsed + timeout;

        if (now > end) Weaken();
    }

    // Invoked to validate the timeout and to return the number of milliseconds.
    static long Validate(TimeSpan timeout)
    {
        var ms = (long)timeout.TotalMilliseconds;

        if (ms is < 0 or > int.MaxValue) throw new ArgumentOutOfRangeException(
            nameof(timeout),
            $"Invalid timeout: {timeout}");

        return ms;
    }

    /// <summary>
    /// Updates the underlying reference kept by this instance.
    /// </summary>
    /// <param name="target"></param>
    public void Update(T target)
    {
        _Hard = target ?? throw new ArgumentNullException(nameof(target));
        _Weak = new WeakReference(target);
        _LastUsed = DateTime.UtcNow;
        _Value = ComputeValue(target) ?? target.Sketch();
    }
}