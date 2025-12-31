using Lox.Expressions.Visitors;

namespace Lox.Expressions;

public abstract record Expression {
    public abstract T Accept<T>(IVisitor<T> visitor);
}
