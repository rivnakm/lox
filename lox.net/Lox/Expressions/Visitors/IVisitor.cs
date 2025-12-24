namespace Lox.Expressions.Visitors;

public interface IVisitor<out T>
{
    T Visit(BinaryExpression binExpr);
    T Visit(Grouping grouping);
    T Visit(Literal literal);
    T Visit(UnaryExpression unaryExpr);
}
