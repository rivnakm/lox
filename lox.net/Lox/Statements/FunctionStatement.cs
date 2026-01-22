using Lox.Statements.Visitors;

namespace Lox.Statements;

public class FunctionStatement
    : IStatement {
    public Token Name { get; }
    public IReadOnlyList<Token> Parameters { get; }
    public IReadOnlyList<IStatement> Body { get; }

    public FunctionStatement(Token name, IReadOnlyList<Token> parameters, IReadOnlyList<IStatement> body) {
        this.Name = name;
        this.Parameters = parameters;
        this.Body = body;
    }

    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
