namespace Runner.Tester;

// ========================================================
/// <summary>
/// Represents a holder for a known assembly.
/// </summary>
public class AssemblyHolder(Assembly assembly)
{
    /// <inheritdoc/>
    public override string ToString() => Name;

    /// <summary>
    /// The assembly this instance refers to.
    /// </summary>
    public Assembly Assembly { get; } = assembly.ThrowWhenNull();

    /// <summary>
    /// The name of this assembly.
    /// </summary>
    public string Name => Assembly.GetName().Name!;

    /// <summary>
    /// The list of type holders in this instance.
    /// </summary>
    public TypeHolderList TypeHolders { get; } = [];

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to populate this instance with its test classes and methods.
    /// </summary>
    public void Populate()
    {
        foreach (var type in Assembly.DefinedTypes)
        {
            if (!TypeHolder.IsValid(type)) continue;

            if (TypeHolders.Find(type) == null)
            {
                var holder = new TypeHolder(type);
                holder.Populate();

                if (holder.MethodHolders.Count > 0) TypeHolders.Add(holder);
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to purge the types and methods in the given request.
    /// </summary>
    /// <param name="item"></param>
    public void PurgeExcludes(Request item)
    {
        foreach (var holder in TypeHolders.ToList())
        {
            if (item.TypeName != null)
            {
                if (holder.FullName.EndsWith(item.TypeName))
                {
                    if (item.MethodName == null)
                    {
                        TypeHolders.Remove(holder);
                        continue;
                    }
                }
                else
                {
                    holder.PurgeExcludes(item);
                }
            }
            else
            {
                holder.PurgeExcludes(item);
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance carries any enforced methods.
    /// </summary>
    /// <returns></returns>
    public bool HasEnforcedMethods()
    {
        return TypeHolders.Any(x => x.HasEnforcedMethods());
    }

    /// <summary>
    /// Invoked to purge not-enforced methods, if any was enforced.
    /// </summary>
    public void PurgeNotEnforcedMethods()
    {
        foreach (var holder in TypeHolders.ToList())
        {
            holder.PurgeNotEnforcedMethods();

            if (holder.MethodHolders.Count == 0) TypeHolders.Remove(holder);
            else holder.IsEnforced = true;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance carries any enforced types.
    /// </summary>
    /// <returns></returns>
    public bool HasEnforcedTypes()
    {
        return TypeHolders.Any(x => x.IsEnforced);
    }

    /// <summary>
    /// Invoked to purge not-enforced types, if any was enforced.
    /// </summary>
    public void PurgeNotEnforcedTypes()
    {
        foreach (var holder in TypeHolders.ToList())
        {
            if (!holder.IsEnforced) TypeHolders.Remove(holder);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to purge the holders in this instance with no test elements.
    /// </summary>
    public void PurgeEmpty()
    {
        foreach (var holder in TypeHolders.ToList())
        {
            if (holder.MethodHolders.Count == 0) TypeHolders.Remove(holder);
        }
    }
}