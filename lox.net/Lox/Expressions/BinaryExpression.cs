using Lox.Expressions.Visitors;

namespace Lox.Expressions;

public record BinaryExpression(Expression Left, Token Operator, Expression Right) : Expression {
    public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
}
