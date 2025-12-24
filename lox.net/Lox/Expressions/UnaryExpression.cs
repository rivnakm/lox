using Lox.Expressions.Visitors;

namespace Lox.Expressions;

public record UnaryExpression(Token Operator, Expression Right) : Expression
{
    public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
}
