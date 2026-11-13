#  Yotei.Tools

## StringSpan.IndexOfAny(string-alike values)

Para `ReadOnlySpan<char>` no puedo definir métodos de extensión tipo
`IndexOfAny(IEnumerable<ReadOnlySpan<char>> values)`, pues es una
`ref readonly struct` y no está permitido. La solución obvia es usar como
argumento un array `string[] values`, pero no sería lo eficiente que
queremos que sea.

Una primera aproximación es definir una struct con métodos de conversion:

```csharp
readonly struct StringMemory(ReadOnlyMemory<char> memory)
{
   public ReadOnlyMemory<char> Memory { get; } = memory;
   public static implicir StringMemory(string value) => new(value.AsMemory());
   public static implicir StringMemory(ReadOnlyMemory<char> value) => new(value);
}
```
y definir el método con argumento: `IEnumerable<StringMemory> values`. Esto
permite hacer invocaciones del tipo `IndexOfAny(["a", "b"])` pues se ejecuta
la conversion implícita.

Sin embargo, falla si declaro una variable como `string[] values` y luego
intento pasarla como argumento.
