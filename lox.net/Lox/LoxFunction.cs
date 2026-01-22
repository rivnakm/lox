using Lox.Statements;

namespace Lox;

public class LoxFunction : ILoxCallable {
    private readonly FunctionStatement _declaration;
    private readonly LoxEnvironment _closure;
    private readonly bool _isInitializer;

    public LoxFunction(FunctionStatement declaration, LoxEnvironment closure, bool isInitializer) {
        this._declaration = declaration;
        this._closure = closure;
        this._isInitializer = isInitializer;
    }

    public LoxFunction Bind(LoxInstance instance) {
        var environment = new LoxEnvironment(this._closure);
        environment.Define("this", instance);
        return new LoxFunction(this._declaration, environment, this._isInitializer);
    }

    public object? Call(Interpreter interpreter, List<object?> arguments) {
        var environment = new LoxEnvironment(this._closure);
        for (var i = 0; i < this._declaration.Parameters.Count; i++) {
            environment.Define(this._declaration.Parameters[i].Lexeme, arguments[i]);
        }

        try {
            interpreter.ExecuteBlock(this._declaration.Body, environment);
        }
        catch (Return returnValue) {
            if (this._isInitializer) {
                return this._closure.GetAt(0, "this");
            }

            return returnValue.Value;
        }

        if (this._isInitializer) {
            return this._closure.GetAt(0, "this");
        }

        return null;
    }

    public int Arity() {
        return this._declaration.Parameters.Count;
    }

    public override string ToString() {
        return $"<fn {this._declaration.Name.Lexeme}>";
    }
}
