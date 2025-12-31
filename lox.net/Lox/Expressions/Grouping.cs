using Lox.Expressions.Visitors;

namespace Lox.Expressions;

public record Grouping(Expression Expression) : Expression {
    public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
}
