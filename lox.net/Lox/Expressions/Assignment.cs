using Lox.Expressions.Visitors;

namespace Lox.Expressions;

public record Assignment(Token Name, Expression Value) : Expression {
    public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
