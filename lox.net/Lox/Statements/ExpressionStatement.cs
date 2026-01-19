using Lox.Expressions;
using Lox.Statements.Visitors;

namespace Lox.Statements;

public record ExpressionStatement(Expression Expression) : Statement {
    public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
