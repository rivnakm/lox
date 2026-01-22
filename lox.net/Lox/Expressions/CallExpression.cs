using Lox.Expressions.Visitors;

namespace Lox.Expressions;

public class CallExpression : IExpression {
    public IExpression Callee { get; }
    public Token Paren { get; }
    public IReadOnlyList<IExpression> Arguments { get; }

    public CallExpression(IExpression callee, Token paren, IReadOnlyList<IExpression> arguments) {
        this.Callee = callee;
        this.Paren = paren;
        this.Arguments = arguments;
    }

    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
