using Lox.Expressions;
using Lox.Statements.Visitors;

namespace Lox.Statements;

public class VarStatement : IStatement {
    public Token Name { get; }
    public IExpression? Initializer { get; }

    public VarStatement(Token name, IExpression? initializer) {
        this.Name = name;
        this.Initializer = initializer;
    }

    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
