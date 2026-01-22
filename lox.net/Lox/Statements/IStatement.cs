using Lox.Statements.Visitors;

namespace Lox.Statements;

public interface IStatement {
    T Accept<T>(IVisitor<T> visitor);
}
