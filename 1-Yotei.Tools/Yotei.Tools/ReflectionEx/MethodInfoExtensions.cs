namespace Yotei.Tools;

// ========================================================
public static class MethodInfoEx
{
    extension(MethodInfo source)
    {
        /// <summary>
        /// Determines if the method is a hidding (or new-alike) one by checking that it is not an
        /// override and that there is a method in a base type with the same signature (so this one
        /// hides it). Typically, these methods are decorated with a 'new' keyword, but they don't
        /// need to (at the expense of a compiler warning).
        /// <br/> Notes:
        /// <br/>- abstract override that hides a concrete base method returns 'true'.
        /// </summary>
        public bool IsNewAlike
        {
            get
            {
                // No host, then hides nothing...
                var type = source.DeclaringType;
                if (type == null) return false;

                // If it is an override, then it is not a 'new' one...
                var basedef = source.GetBaseDefinition();
                if (basedef.DeclaringType != source.DeclaringType) return false;

                // // Finding a base method with the same signature...
                var pars = source.GetParameters().Select(x => x.ParameterType).ToArray();

                if (type.IsInterface) // Interfaces...
                {
                    var ifaces = type.GetInterfaces();
                    foreach (var iface in ifaces)
                    {
                        var method = iface.GetMethod(
                            source.Name,
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.Static,
                            null, pars, null);

                        // If found, then the given one hides it...
                        if (method != null) return true;
                    }
                }

                else // Other kinds...
                {
                    while ((type = type.BaseType) != null)
                    {
                        var method = type.GetMethod(
                            source.Name,
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.Static,
                            null, pars, null);

                        // If found, then the given one hides it...
                        if (method != null) return true;
                    }
                }

                // Not found, so hides nothing...
                return false;
            }
        }
    }
}