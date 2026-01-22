using Lox.Expressions;
using Lox.Statements.Visitors;

namespace Lox.Statements;

public class WhileStatement : IStatement {
    public IExpression Condition { get; }
    public IStatement Body { get; }

    public WhileStatement(IExpression condition, IStatement body) {
        this.Condition = condition;
        this.Body = body;
    }

    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
