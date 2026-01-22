using Lox.Expressions.Visitors;

namespace Lox.Expressions;

public class VariableExpression : IExpression {
    public Token Name { get; }

    public VariableExpression(Token name) {
        this.Name = name;
    }

    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
