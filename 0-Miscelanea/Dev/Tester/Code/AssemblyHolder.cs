namespace Dev.Tester;

// ========================================================
/// <summary>
/// Represents a holder for a given assembly.
/// </summary>
internal class AssemblyHolder
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="assembly"></param>
    public AssemblyHolder(Assembly assembly)
    {
        Assembly = assembly.ThrowIfNull();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string ToString() => Name;

    /// <summary>
    /// The assembly this instance refers to.
    /// </summary>
    public Assembly Assembly { get; }

    /// <summary>
    /// The name of the assembly this instance refers to.
    /// </summary>
    public string Name => Assembly.GetName().Name!;

    // ----------------------------------------------------

    /// <summary>
    /// The collection of child holders maintained by this instance.
    /// </summary>
    public TypeHolderList TypeHolders { get; } = new();

    /// <summary>
    /// Populates the child holders of this instance using the given specifications.
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="methodName"></param>
    public void Populate(string? typeName = null, string? methodName = null)
    {
        typeName = typeName?.NotNullNotEmpty();
        methodName = methodName?.NotNullNotEmpty();

        // All types...
        if (typeName == null)
        {
            var types = Assembly.DefinedTypes;
            foreach (var type in types)
            {
                if (!type.IsValidTest(out _)) continue;

                var holder = TypeHolders.Add(type);
                holder.Populate(methodName);
            }
        }

        // Requested type...
        else
        {
            var type = Assembly.DefinedTypes
                .Where(x => x.Name == typeName)
                .SingleOrDefault();

            if (type == null) throw new NotFoundException(
                "Requested type not found.")
                .WithData(typeName);

            if (!type.IsValidTest(out var ex)) throw ex;
            var holder = TypeHolders.Add(type);
            holder.Populate(methodName);
        }
    }

    /// <summary>
    /// Purges the child holders of this instance using the given specifications.
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="methodName"></param>
    public void Purge(string? typeName = null, string? methodName = null)
    {
        typeName = typeName?.NotNullNotEmpty();
        methodName = methodName?.NotNullNotEmpty();

        // All types...
        if (typeName == null)
        {
            TypeHolders.Clear();
        }

        // Requested type...
        else
        {
            var holder = TypeHolders.Find(typeName);
            if (holder != null)
            {
                holder.Purge(methodName);
                if (holder.MethodHolders.Count == 0) TypeHolders.Remove(holder);
            }
        }
    }
}