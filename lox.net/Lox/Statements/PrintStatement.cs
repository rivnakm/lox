using Lox.Expressions;
using Lox.Statements.Visitors;

namespace Lox.Statements;

public class PrintStatement : IStatement {
    public IExpression Expression { get; }

    public PrintStatement(IExpression expression) {
        this.Expression = expression;
    }

    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
