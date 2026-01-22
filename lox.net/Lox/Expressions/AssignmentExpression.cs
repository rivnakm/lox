using Lox.Expressions.Visitors;

namespace Lox.Expressions;

public class AssignmentExpression : IExpression {
    public Token Name { get; }
    public IExpression Value { get; }

    public AssignmentExpression(Token name, IExpression value) {
        this.Name = name;
        this.Value = value;
    }

    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
