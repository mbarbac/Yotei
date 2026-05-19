#nullable enable

namespace Yotei.Tools.CloneGenerator;

// ========================================================
[AttributeUsage(AttributeTargets.All)]
[Microsoft.CodeAnalysis.Embedded]
public class CloneableAttribute : Attribute { } // DEBUG-ONLY