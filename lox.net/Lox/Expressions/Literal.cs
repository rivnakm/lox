using Lox.Expressions.Visitors;

namespace Lox.Expressions;

public record Literal(object? Value) : Expression
{
    public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
}
