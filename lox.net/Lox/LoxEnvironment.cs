using Lox.Exceptions;

namespace Lox;

public class LoxEnvironment {
    private readonly Dictionary<string, object?> _variables = new();
    private readonly LoxEnvironment? _parent;

    public LoxEnvironment(LoxEnvironment? parent = null) {
        this._parent = parent;
    }

    public object? this[Token name] => this.Get(name);

    public object? Get(Token name) {
        if (this._variables.TryGetValue(name.Lexeme, out var value)) {
            return value;
        }

        if (this._parent is not null) {
            return this._parent[name];
        }

        throw new UndefinedVariableException($"Undefined variable '{name.Lexeme}'");
    }

    public void Define(Token name, object? value) {
        _ = this._variables.TryAdd(name.Lexeme, value);
    }

    public void Assign(Token name, object? value) {
        if (this._variables.ContainsKey(name.Lexeme)) {
            this._variables[name.Lexeme] = value;
        }

        if (this._parent is not null) {
            this._parent.Assign(name, value);
            return;
        }

        throw new UndefinedVariableException($"Undefined variable '{name.Lexeme}'");
    }
}
