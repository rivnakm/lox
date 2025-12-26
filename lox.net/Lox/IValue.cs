using System.Diagnostics.CodeAnalysis;

namespace Lox;

public interface IValue : IEquatable<IValue>
{
    bool IsTruthy { get; }
    bool IsNil { get; }
    Type? ValueType { get; }
    object? GetValue();
    T GetValue<T>();
    bool TryGetValue<T>([NotNullWhen(true)] out T? value);
}
