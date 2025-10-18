namespace Yotei.Tools.WithGenerator.Tests;

[InheritWiths]
public class Persona<T>(string name) : IPersona { public string Name { get; } = name; }