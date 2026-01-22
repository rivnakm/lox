namespace Lox.Expressions.Visitors;

public interface IVisitor<out T> {
    T Visit(AssignmentExpression expr);
    T Visit(BinaryExpression expr);
    T Visit(CallExpression expr);
    T Visit(GetExpression expr);
    T Visit(GroupingExpression expr);
    T Visit(LiteralExpression expr);
    T Visit(LogicalExpression expr);
    T Visit(SetExpression expr);
    T Visit(SuperExpression expr);
    T Visit(ThisExpression expr);
    T Visit(UnaryExpression expr);
    T Visit(VariableExpression expr);
}
