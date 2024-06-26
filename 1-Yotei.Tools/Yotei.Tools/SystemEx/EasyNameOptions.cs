namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Provides options for the <see cref="EasyNameExtensions"/> methods.
/// </summary>
public record EasyTypeOptions
{
    /// <summary>
    /// Use the namespace of the given type, or <c>false</c> to ignore it.
    /// </summary>
    public bool UseNamespace
    {
        get => _UseNamespace;
        init
        {
            if ((_UseNamespace = value))
            {
                _UseHostType = true;
                _UseTypeName = true;
            }
        }
    }
    bool _UseNamespace = false;

    /// <summary>
    /// Use the declaring type of the given one, or <c>false</c> to ignore it.
    /// </summary>
    public bool UseHostType
    {
        get => _UseHostType;
        init
        {
            if ((_UseHostType = value)) _UseTypeName = true;
            else _UseNamespace = false;
        }
    }
    bool _UseHostType = false;

    /// <summary>
    /// Use the type short name, or <c>false</c> to leave it blank.
    /// <br/> The default value of this property is <c>true</c>.
    /// </summary>
    public bool UseTypeName
    {
        get => _UseTypeName;
        init
        {
            if (!(_UseTypeName = value))
            {
                _UseNamespace = false;
                _UseHostType = false;
            }
        }
    }
    bool _UseTypeName = true;

    /// <summary>
    /// Use the type's generic arguments, or <c>false</c> to ignore them.
    /// </summary>
    public bool UseTypeArguments
    {
        get => _UseTypeArguments;
        init => _UseTypeArguments = value;
    }
    bool _UseTypeArguments = false;

    /// <summary>
    /// Use the namespaces of the type's generic arguments, or <c>false</c> to ignore them.
    /// </summary>
    public bool UseTypeArgumentNamespaces
    {
        get => _UseTypeArgumentNamespaces;
        init
        {
            if ((_UseTypeArgumentNamespaces = value))
            {
                _UseTypeArguments = true;
                _UseTypeArgumentHostTypes = true;
                _UseTypeArgumentTypeNames = true;
            }
        }
    }
    bool _UseTypeArgumentNamespaces = false;

    /// <summary>
    /// Use the host types of the type's generic arguments, or <c>false</c> to ignore them.
    /// </summary>
    public bool UseTypeArgumentHostTypes
    {
        get => _UseTypeArgumentHostTypes;
        init
        {
            if ((_UseTypeArgumentNamespaces = value))
            {
                _UseTypeArguments = true;
                _UseTypeArgumentTypeNames = true;
            }
            else _UseTypeArgumentNamespaces = false;
        }
    }
    bool _UseTypeArgumentHostTypes = false;

    /// <summary>
    /// Use the type names of the type's generic arguments, or <c>false</c> to ignore them.
    /// <br/> The default value of this property is <c>true</c>.
    /// </summary>
    public bool UseTypeArgumentTypeNames
    {
        get => _UseTypeArgumentTypeNames;
        init
        {
            if ((_UseTypeArgumentTypeNames)) _UseTypeArguments = true;
            else
            {
                _UseTypeArgumentNamespaces = false;
                _UseTypeArgumentHostTypes = false;
            }
        }
    }
    bool _UseTypeArgumentTypeNames = true;

    // ----------------------------------------------------

    /// <summary>
    /// A common shared default instance.
    /// </summary>
    public static EasyTypeOptions Default { get; } = new();

    /// <summary>
    /// A common shared instance with all options set.
    /// </summary>
    public static EasyTypeOptions True { get; } = new EasyTypeOptions() with
    {
        UseNamespace = true,
        UseHostType = true,
        UseTypeName = true,
        UseTypeArguments = true,
        UseTypeArgumentNamespaces = true,
        UseTypeArgumentHostTypes = true,
        UseTypeArgumentTypeNames = true,
    };

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    public EasyTypeOptions() { }
}