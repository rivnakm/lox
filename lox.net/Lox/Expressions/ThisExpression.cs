using Lox.Expressions.Visitors;

namespace Lox.Expressions;

public class ThisExpression : IExpression {
    public Token Keyword { get; }

    public ThisExpression(Token keyword) {
        this.Keyword = keyword;
    }

    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
