using Lox.Statements.Visitors;

namespace Lox.Statements;

public record BlockStatement(List<Statement> Statements) : Statement {
    public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
