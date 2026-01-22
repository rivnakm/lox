using Lox.Expressions.Visitors;

namespace Lox.Expressions;

public class GroupingExpression : IExpression {
    public IExpression Expression { get; }

    public GroupingExpression(IExpression expression) {
        this.Expression = expression;
    }
    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
