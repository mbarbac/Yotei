using Microsoft.Win32.SafeHandles;

namespace Experimental;

// ========================================================
/// <summary>
/// A reflection-based <see cref="IEqualityComparer{T}"/> that compares instances of an arbitrary
/// reference type, using their value semantics instead of their reference equality. This is then
/// intended for classes that do no implement their own value-based equality semantics (so do not
/// use with records or structs), and it is not recommended for performance-sensitive scenarios.
/// </summary>
/// <typeparam name="T"></typeparam>
public class GenericComparer<T> : IEqualityComparer<T> where T : class
{
    readonly List<Func<T, object>> _propertygetters = [];
    readonly List<Func<T, object>> _fieldgetters = [];

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="usePrivateProperties"></param>
    /// <param name="useFields"></param>
    /// <param name="usePrivateFields"></param>
    public GenericComparer(
        bool usePrivateProperties = false,
        bool useFields = false, bool usePrivateFields = false)
    {
        CreatePropertyGetters(usePrivateProperties);
        CreateFieldGetters(useFields, usePrivateFields);
    }

    /// <summary>
    /// Invoked to collect the property getters.
    /// </summary>
    /// <param name="useprivate"></param>
    void CreatePropertyGetters(bool useprivate)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public;
        if (useprivate) flags |= BindingFlags.NonPublic;

        var props = typeof(T).GetProperties(flags).Where(m => m.GetMethod != null).ToList();
        foreach (var prop in props)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");
            MemberExpression expression = Expression.Property(parameter, prop.Name);
            Expression boxed = Expression.Convert(expression, typeof(object));
            Expression<Func<T, object>> getter = Expression.Lambda<Func<T, object>>(boxed, parameter);
            _propertygetters.Add(getter.Compile());
        }
    }

    /// <summary>
    /// Invoked to collect the field getters.
    /// </summary>
    /// <param name="useprivate"></param>
    void CreateFieldGetters(bool usepublic, bool useprivate)
    {
        if (!usepublic && !useprivate) return;

        var flags = BindingFlags.Instance;
        if (usepublic) flags |= BindingFlags.Public;
        if (useprivate) flags |= BindingFlags.NonPublic;

        var fields = typeof(T).GetFields(flags).ToList();

        foreach (var field in fields)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "f");
            MemberExpression expression = Expression.Field(parameter, field.Name);
            Expression boxed = Expression.Convert(expression, typeof(object));
            Expression<Func<T, object>> getter = Expression.Lambda<Func<T, object>>(boxed, parameter);
            _fieldgetters.Add(getter.Compile());
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(T? x, T? y)
    {
        if (SameValue(x, y)) return true;
        if (x!.GetType() != y!.GetType()) return false;

        foreach (var prop in _propertygetters)
        {
            var vx = prop(x);
            var vy = prop(y);
            if (!SameValue(vx, vy)) return false;
        }

        foreach (var field in _fieldgetters)
        {
            var vx = field(x);
            var vy = field(y);
            if (!SameValue(vx, vy)) return false;
        }

        return true;
    }

    // Determines equality of two values in a null-safe manner...
    static bool SameValue<V>([NotNullWhen(true)] V? x, [NotNullWhen(true)] V? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int GetHashCode([DisallowNull] T obj)
    {
        int hash = 0;

        var props = _propertygetters.Select(x => x(obj)).ToList();
        for (int i = 0; i < props.Count; i += 8)
        {
            hash = HashCode.Combine(hash,
                props.ElementAtOrDefault(i),
                props.ElementAtOrDefault(i + 1),
                props.ElementAtOrDefault(i + 2),
                props.ElementAtOrDefault(i + 3),
                props.ElementAtOrDefault(i + 4),
                props.ElementAtOrDefault(i + 5),
                props.ElementAtOrDefault(i + 6));
        }

        if (_fieldgetters.Count > 0)
        {
            var fields = _fieldgetters.Select(x => x(obj)).ToList();
            for (int i = 0; i < fields.Count; i += 8)
            {
                hash = HashCode.Combine(hash,
                    fields.ElementAtOrDefault(i),
                    fields.ElementAtOrDefault(i + 1),
                    fields.ElementAtOrDefault(i + 2),
                    fields.ElementAtOrDefault(i + 3),
                    fields.ElementAtOrDefault(i + 4),
                    fields.ElementAtOrDefault(i + 5),
                    fields.ElementAtOrDefault(i + 6));
            }
        }

        return hash;
    }
}