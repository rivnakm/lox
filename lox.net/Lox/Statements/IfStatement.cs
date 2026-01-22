using Lox.Expressions;
using Lox.Statements.Visitors;

namespace Lox.Statements;

public class IfStatement : IStatement {
    public IExpression Condition { get; }
    public IStatement Then { get; }
    public IStatement? Else { get; }

    public IfStatement(IExpression condition, IStatement then, IStatement? @else) {
        this.Condition = condition;
        this.Then = then;
        this.Else = @else;
    }

    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
