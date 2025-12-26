using System.Diagnostics.CodeAnalysis;
using Lox.Exceptions;

namespace Lox;

public struct Value : IValue
{
    private readonly object? _value;

    public Value(object? value)
    {
        this._value = value;
    }

    public bool IsTruthy => this._value is not null && this._value is not (false);
    public bool IsNil => this._value is null;
    public Type? ValueType => this._value?.GetType();

    public object? GetValue()
    {
        return this._value;
    }

    public T GetValue<T>()
    {
        // FIXME: this returns C# type names, it should return lox types
        // FIXME: also the token should be included in this exception
        if (this._value is null)
        {
            throw new InvalidTypeException($"Expected a value of type {typeof(T).FullName}, got nil");
        }

        if (this._value is T typedValue)
        {
            return typedValue;
        }
        throw new InvalidTypeException($"Expected a value of type {typeof(T).FullName}, got {this._value!.GetType().FullName}");
    }

    public bool TryGetValue<T>([NotNullWhen(true)] out T? value)
    {
        if (this._value is null)
        {
            value = default;
            return false;
        }

        if (this._value is T typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default;
        return false;
    }

    public bool Equals(Value? other)
    {
        if (other is null)
        {
            return false;
        }

        if (this._value is null && other.Value._value is null)
        {
            return true;
        }

        if (this._value is null || other.Value._value is null)
        {
            return false;
        }

        if (this._value.GetType() != other.Value._value.GetType())
        {
            return false;
        }

        return this._value.Equals(other.Value._value);
    }

    bool IEquatable<IValue>.Equals(IValue? other)
    {
        if (other is null)
        {
            return false;
        }

        if (this.IsNil && other.IsNil)
        {
            return true;
        }

        if (other is Value otherValue)
        {
            return this.Equals(otherValue);
        }

        if (this.ValueType != other.ValueType)
        {
            return false;
        }

        return this.GetValue()!.Equals(other.GetValue());
    }

    public override bool Equals(object? obj)
    {
        return obj is Value other && this.Equals(other);
    }

    public override int GetHashCode()
    {
        return (this._value != null ? this._value.GetHashCode() : 0);
    }

    public override string ToString()
    {
        return this._value is null ? "nil" : this._value.ToString();
    }
}
