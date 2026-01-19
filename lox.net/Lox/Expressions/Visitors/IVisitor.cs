namespace Lox.Expressions.Visitors;

public interface IVisitor<out T> {
    T Visit(BinaryExpression expr);
    T Visit(Grouping expr);
    T Visit(Literal expr);
    T Visit(UnaryExpression expr);
    T Visit(Variable expr);
    T Visit(Assignment expr);
}
