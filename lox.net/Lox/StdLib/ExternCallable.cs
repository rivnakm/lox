namespace Lox.StdLib;

public class ExternCallable : ILoxCallable {
    private readonly int _arity;
    private readonly Func<Interpreter, List<object?>, object?> _call;

    public ExternCallable(int arity, Func<Interpreter, List<object?>, object?> call) {
        this._arity = arity;
        this._call = call;
    }

    public int Arity() => this._arity;

    public object? Call(Interpreter interpreter, List<object?> arguments) {
        return this._call.Invoke(interpreter, arguments);
    }

    public override string ToString() {
        return "<native fn>";
    }
}
