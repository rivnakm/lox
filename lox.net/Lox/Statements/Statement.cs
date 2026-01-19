using Lox.Statements.Visitors;

namespace Lox.Statements;

public abstract record Statement {
    public abstract T Accept<T>(IVisitor<T> visitor);
}
