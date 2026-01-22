using Lox.Expressions.Visitors;

namespace Lox.Expressions;

public class GetExpression : IExpression {
    public IExpression Object { get; }
    public Token Name { get; }

    public GetExpression(IExpression @object, Token name) {
        this.Object = @object;
        this.Name = name;
    }

    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
