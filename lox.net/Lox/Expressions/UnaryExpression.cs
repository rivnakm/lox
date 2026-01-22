using Lox.Expressions.Visitors;

namespace Lox.Expressions;

public class UnaryExpression : IExpression {
    public Token Operator { get; }
    public IExpression Right { get; }

    public UnaryExpression(Token @operator, IExpression right) {
        this.Operator = @operator;
        this.Right = right;
    }

    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
