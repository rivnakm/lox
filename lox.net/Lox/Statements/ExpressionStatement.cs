using Lox.Expressions;
using Lox.Statements.Visitors;

namespace Lox.Statements;

public class ExpressionStatement : IStatement {
    public IExpression Expression { get; }

    public ExpressionStatement(IExpression expression) {
        this.Expression = expression;
    }

    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
