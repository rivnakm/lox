using Lox.Statements.Visitors;

namespace Lox.Statements;

public class BlockStatement : IStatement {
    public List<IStatement> Statements { get; }

    public BlockStatement(List<IStatement> statements) {
        this.Statements = statements;
    }
    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
