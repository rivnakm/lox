using Lox.Expressions;
using Lox.Statements.Visitors;

namespace Lox.Statements;

public record VarStatement(Token Name, Expression? Initializer) : Statement {

    public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
