using Lox.Expressions.Visitors;

namespace Lox.Expressions;

public class LogicalExpression : IExpression {
    public IExpression Left { get; }
    public Token Operator { get; }
    public IExpression Right { get; }
    public LogicalExpression(IExpression left, Token @operator, IExpression right) {
        this.Left = left;
        this.Operator = @operator;
        this.Right = right;
    }
    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
