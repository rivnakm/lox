using Lox.Expressions.Visitors;

namespace Lox.Expressions;

public class SetExpression : IExpression {
    public IExpression Object { get; }
    public Token Name { get; }
    public IExpression Value { get; }

    public SetExpression(IExpression @object, Token name, IExpression value) {
        this.Object = @object;
        this.Name = name;
        this.Value = value;
    }

    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
