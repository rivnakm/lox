using Lox.Exceptions;

namespace Lox;

public class LoxInstance {
    private readonly LoxClass _class;
    private readonly Dictionary<string, object?> _properties = new();

    public LoxInstance(LoxClass @class) {
        this._class = @class;
    }

    public object? Get(string name) {
        if (this._properties.TryGetValue(name, out var value)) {
            return value;
        }

        var method = this._class.FindMethod(name);
        if (method is not null) {
            return method.Bind(this);
        }

        throw new InvalidMemberException($"Undefined property '{name}'");
    }

    public void Set(string name, object? value) {
        this._properties[name] = value;
    }

    public override string ToString() {
        return $"{this._class.Name} instance";
    }
}
