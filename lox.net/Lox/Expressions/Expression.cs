using Lox.Expressions.Visitors;

namespace Lox.Expressions;

public interface IExpression {
    T Accept<T>(IVisitor<T> visitor);
}
