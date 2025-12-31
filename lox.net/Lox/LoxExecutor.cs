using Lox.Expressions.Visitors;

namespace Lox;

public sealed class LoxExecutor {
    private readonly ErrorContext _errorContext;

    public LoxExecutor() {
        this._errorContext = new ErrorContext();
    }

    public void Exec(Stream stream, bool exitOnError = true) {
        using var reader = new StreamReader(stream);
        var lexer = new Lexer(reader.ReadToEnd(), this._errorContext);
        var tokens = lexer.GetTokens();
        var parser = new Parser(tokens.ToList(), this._errorContext);
        var expr = parser.Parse();

        var interpreter = new Interpreter();

        if (exitOnError && (this._errorContext.HasError || expr is null)) {
            Environment.Exit((int)StatusCode.Failure);
        }

        if (expr is not null) {
            interpreter.Interpret(expr);
        }

        this._errorContext.Reset();
    }
}
