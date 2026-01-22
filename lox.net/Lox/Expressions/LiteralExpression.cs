using Lox.Expressions.Visitors;

namespace Lox.Expressions;

public class LiteralExpression : IExpression {
    public object? Value { get; }

    public LiteralExpression(object? value) {
        this.Value = value;
    }

    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
