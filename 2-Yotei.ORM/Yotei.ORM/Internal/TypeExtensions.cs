namespace Yotei.ORM.Code.Internal;

// ========================================================
public static class TypeExtensions
{
    /// <summary>
    /// Determines if the two given types are the same one, or not.
    /// <br/> We use this method because it happens the target type is sometimes a compiler
    /// construct, and for instance has not the same full name as the original target one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool Compare(this Type source, Type target)
    {
        source.ThrowWhenNull();
        target.ThrowWhenNull();

        return
            (source.Name == target.Name) &&
            (source.ReflectedType == target.ReflectedType);
    }

    /// <summary>
    /// Gets the interfaces the given type implements directly.
    /// <br/> This method is not completely accurate: it may not consider some interfaces if it
    /// happens them to be implemented by other first-level ones. In any case, it is enough for
    /// our purposes.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type[] GetDirectInterfaces(this Type type)
    {
        type.ThrowWhenNull();

        var items = type.GetInterfaces();
        return items.Except(items.SelectMany(x => x.GetInterfaces())).ToArray();
    }

    /// <summary>
    /// Determines if the source type implements the given target interface, or not, and if so,
    /// provides an *estimation* of the "distance" between the source and the target. A distance
    /// of <see cref="int.MaxValue"/> means it is not implemented.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public static bool FindInterface(this Type source, Type target, ref int num)
    {
        source.ThrowWhenNull();
        target.ThrowWhenNull();

        if (!target.IsInterface)
        {
            num = int.MaxValue;
            return false;
        }

        return Recursive(source, target, ref num);

        static bool Recursive(Type source, Type target, ref int num)
        {
            // Special case when source is not an interface itself...
            if (!source.IsInterface)
            {
                var temp = num;
                var parent = source.BaseType; while (parent != null)
                {
                    if (Recursive(parent, target, ref temp))
                    {
                        num = temp;
                        return true;
                    }
                    temp++;
                    parent = parent.BaseType;
                }
            }

            // Direct interfaces, we assume distance is already set...
            var ifaces = source.GetDirectInterfaces();
            foreach (var iface in ifaces) if (iface.Compare(target)) return true;

            // Next level...
            num++;
            foreach (var iface in ifaces) if (Recursive(iface, target, ref num)) return true;

            // Not found...
            return false;
        }
    }

    /// <summary>
    /// Determines if the given source type inherits from the given target one and, if so, gets
    /// an *estimation* between the source type and target one, either as a class inheritance,
    /// or as an interface implementation. A distance  of <see cref="int.MaxValue"/> means it
    /// does not inherit.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="parentDistance"></param>
    /// <param name="ifaceDistance"></param>
    /// <returns></returns>
    public static bool InheritsFrom(
        this Type source, Type target,
        out int parentDistance, out int ifaceDistance)
    {
        source.ThrowWhenNull();
        target.ThrowWhenNull();

        // Case: the same...
        if (source.Compare(target))
        {
            parentDistance = source.IsInterface ? int.MaxValue : 0;
            ifaceDistance = source.IsInterface ? 0 : int.MaxValue;
            return true;
        }

        // Case: source is not an interface...
        if (!source.IsInterface)
        {
            var num = 1;
            var parent = source.BaseType; while (parent != null)
            {
                if (parent.Compare(target))
                {
                    parentDistance = num;
                    ifaceDistance = int.MaxValue;
                    return true;
                }
                parent = parent.BaseType;
                num++;
            }
        }

        // Case: target must be an interface
        if (target.IsInterface)
        {
            var num = 1;
            if (source.FindInterface(target, ref num))
            {
                parentDistance = int.MaxValue;
                ifaceDistance = num;
                return true;
            }
        }

        // Not found...
        parentDistance = int.MaxValue;
        ifaceDistance = int.MaxValue;
        return false;
    }

    /// <summary>
    /// Determines if the given source type inherits from the given target one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool InheritsFrom(this Type source, Type target)
    {
        return InheritsFrom(source, target, out _, out _);
    }
}