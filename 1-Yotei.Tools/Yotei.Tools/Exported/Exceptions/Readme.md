# Exported

Files in this folder are intended to both be used in standard scenarios
as well as imported as linked ones in source code generators' projects.

## Standard Usage
- Namespace: 'Yotei.Tools'.
- Types' visibility: 'public'.

## Source Code Generators' Usage
- Namespace: 'Yotei.Tools.Generators'.
- Types' visibility: 'internal'.

The consuming project:
- Must define the 'YOTEI_TOOLS_GENERATORS' symbol.
- Must not reference the 'Yotei.Tools' project or package. Instead, it
  shall import the files under the 'Exported' folder as links.