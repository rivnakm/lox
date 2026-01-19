using Lox.Expressions.Visitors;

namespace Lox.Expressions;

public record Variable(Token Name) : Expression {
    public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
