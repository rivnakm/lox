using System.Diagnostics;
using Lox.Exceptions;

namespace Lox;

public class LoxEnvironment {
    public LoxEnvironment? Parent { get; }
    private readonly Dictionary<string, object?> _variables = new();

    public LoxEnvironment(LoxEnvironment? parent = null) {
        this.Parent = parent;
    }

    public object? this[string name] => this.Get(name);

    public object? Get(string name) {
        if (this._variables.TryGetValue(name, out var value)) {
            return value;
        }

        if (this.Parent is not null) {
            return this.Parent.Get(name);
        }

        throw new UndefinedVariableException($"Undefined variable '{name}'");
    }

    public void Define(string name, object? value) {
        this._variables[name] = value;
    }

    public void Assign(string name, object? value) {
        if (this._variables.ContainsKey(name)) {
            this._variables[name] = value;
            return;
        }

        if (this.Parent is not null) {
            this.Parent.Assign(name, value);
            return;
        }

        throw new UndefinedVariableException($"Undefined variable '{name}'");
    }

    public object? GetAt(int distance, string name) {
        return this.Ancestor(distance)._variables.GetValueOrDefault(name);
    }

    public void AssignAt(int distance, string name, object? value) {
        this.Ancestor(distance)._variables[name] = value;
    }

    private LoxEnvironment Ancestor(int distance) {
        var environment = this;
        for (var i = 0; i < distance; i++) {
            Debug.Assert(environment.Parent is not null);
            environment = environment.Parent;
        }

        return environment;
    }
}
