namespace Lox;

public class LoxClass : ILoxCallable {
    public string Name { get; }
    private readonly LoxClass? _superClass;
    private readonly IReadOnlyDictionary<string, LoxFunction> _methods;

    public LoxClass(string name, LoxClass? superClass, Dictionary<string, LoxFunction> methods) {
        this.Name = name;
        this._superClass = superClass;
        this._methods = methods;
    }

    public LoxFunction? FindMethod(string name) {
        if (this._methods.TryGetValue(name, out var func)) {
            return func;
        }

        if (this._superClass is not null) {
            return this._superClass.FindMethod(name);
        }

        return null;
    }

    public object? Call(Interpreter interpreter, List<object?> arguments) {
        var instance = new LoxInstance(this);
        var initializer = this.FindMethod("init");
        if (initializer is not null) {
            initializer.Bind(instance).Call(interpreter, arguments);
        }

        return instance;
    }

    public int Arity() {
        var initializer = this.FindMethod("init");
        if (initializer is null) {
            return 0;
        }

        return initializer.Arity();
    }

    public override string ToString() {
        return this.Name;
    }
}
