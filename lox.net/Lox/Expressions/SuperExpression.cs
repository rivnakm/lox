using Lox.Expressions.Visitors;

namespace Lox.Expressions;

public class SuperExpression : IExpression {
    public Token Keyword { get; }
    public Token Method { get; }

    public SuperExpression(Token keyword, Token method) {
        this.Keyword = keyword;
        this.Method = method;
    }

    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
