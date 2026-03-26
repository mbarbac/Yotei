# Yotei Shared Tools
Contains common tools code that can be used in .NET
10 and .NET Standard 2.0 source code generation
projects.

For the later, the compilation symbol
'YOTEI_TOOLS_COREGENERATOR'
must be defined at the host project, and it must
import the 'System.Memory' NuGet package.

* YOTEI_TOOLS_COREGENERATOR: forces all elements to
be internal ones, and in its speciic namespace, so
that they can be used in generators with no name
colisions and no external visibility.

* System.Memory: used to import Span-alike
capabilities

In addition, to use records, the host project must:

* Set its language version to 9.0 or greater.
* Define an static IsExternalInit class in the
'System.Runtime.CompilerServices' namespace.
