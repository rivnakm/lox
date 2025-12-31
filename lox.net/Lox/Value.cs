using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Lox.Exceptions;

namespace Lox;

public readonly struct Value : IEquatable<Value> {
    private readonly object? _value;

    public Value(object? value) {
        this._value = value;
        if (this._value is null) {
            this.ValueType = ValueType.Nil;
            return;
        }

        this.ValueType = this._value switch {
            int or long or short or float or double => ValueType.Number,
            string => ValueType.String,
            _ => throw new ArgumentException($"Unsupported type '{this._value.GetType().FullName}'")
        };
    }

    public bool IsNil => this.ValueType == ValueType.Nil;
    public ValueType ValueType { get; }

    public object? GetValue() {
        return this._value;
    }

    public bool GetBoolean() {
        return this.GetValue<bool>(ValueType.Boolean);
    }

    public double GetNumber() {
        return this.GetValue<double>(ValueType.Number);
    }

    public string GetString() {
        return this.GetValue<string>(ValueType.String);
    }

    private T GetValue<T>(ValueType expectedValueType) {
        if (expectedValueType == ValueType.Nil) {
            throw new ArgumentException("Cannot retrieve a value of a nil value", nameof(expectedValueType));
        }

        if (this.ValueType != expectedValueType) {
            throw new InvalidTypeException($"Expected a value of type {expectedValueType}, got {this.ValueType}");
        }

        return (T)this._value!;
    }

    public bool TryGetBoolean(out bool value) {
        return this.TryGetValue(ValueType.Boolean, out value);
    }

    public bool TryGetNumber(out double value) {
        return this.TryGetValue(ValueType.Number, out value);
    }

    public bool TryGetString([NotNullWhen(true)] out string? value) {
        return this.TryGetValue(ValueType.String, out value);
    }

    private bool TryGetValue<T>(ValueType expectedValueType, [NotNullWhen(true)] out T? value) {
        if (expectedValueType == ValueType.Nil) {
            throw new ArgumentException("Cannot retrieve a value of a nil value", nameof(expectedValueType));
        }

        Debug.Assert(this._value is not null);
        if (this.ValueType == expectedValueType) {
            value = (T)this._value;
            return true;
        }

        value = default;
        return false;
    }

    #region IEquatable<Value>

    bool IEquatable<Value>.Equals(Value other) {
        if (this._value is null && other._value is null) {
            return true;
        }

        if (this._value is null || other._value is null) {
            return false;
        }

        if (this._value.GetType() != other._value.GetType()) {
            return false;
        }

        return this._value.Equals(other._value);
    }

    public override bool Equals(object? obj) {
        return obj is Value other && this.Equals(other);
    }

    public override int GetHashCode() {
        return (this._value != null ? this._value.GetHashCode() : 0);
    }

    #endregion

    #region Operators

    public static implicit operator Value(bool value) => new(value);
    public static implicit operator Value(double value) => new(value);
    public static implicit operator Value(int value) => new(value);
    public static implicit operator Value(string value) => new(value);

    public static bool operator ==(Value lhs, Value rhs) => lhs.Equals(rhs);

    public static bool operator !=(Value lhs, Value rhs) => !lhs.Equals(rhs);

    // C# lets us define "truthy" behavior in the implementation itself
    public static bool operator true(Value value) => value._value is not null && value._value is not (false);
    public static bool operator false(Value value) => value._value is null or (false);
    public static bool operator !(Value value) => value._value is null or (false);

    public static Value operator -(Value value) => new(-value.GetNumber());

    public static Value operator >(Value lhs, Value rhs) => new(lhs.GetNumber() > rhs.GetNumber());
    public static Value operator >=(Value lhs, Value rhs) => new(lhs.GetNumber() >= rhs.GetNumber());
    public static Value operator <(Value lhs, Value rhs) => new(lhs.GetNumber() < rhs.GetNumber());
    public static Value operator <=(Value lhs, Value rhs) => new(lhs.GetNumber() <= rhs.GetNumber());

    public static Value operator -(Value lhs, Value rhs) => new(lhs.GetNumber() - rhs.GetNumber());
    public static Value operator /(Value lhs, Value rhs) => new(lhs.GetNumber() / rhs.GetNumber());
    public static Value operator *(Value lhs, Value rhs) => new(lhs.GetNumber() * rhs.GetNumber());

    public static Value operator +(Value lhs, Value rhs) {
        if (lhs.ValueType == ValueType.Number && rhs.ValueType == ValueType.Number) {
            return new Value(lhs.GetNumber() + rhs.GetNumber());
        }

        if (lhs.ValueType == ValueType.String && rhs.ValueType == ValueType.String) {
            return new Value(lhs.GetString() + rhs.GetString());
        }

        throw new InvalidTypeException($"Cannot apply operator '+' to types {lhs.ValueType} and {rhs.ValueType}");
    }

    #endregion

    public override string ToString() {
        return (this._value is null ? "nil" : this._value.ToString()) ?? string.Empty;
    }
}
