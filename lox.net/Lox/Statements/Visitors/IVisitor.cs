namespace Lox.Statements.Visitors;

public interface IVisitor<out T> {
    T Visit(ExpressionStatement stmt);
    T Visit(PrintStatement stmt);
    T Visit(VarStatement stmt);
    T Visit(BlockStatement stmt);
}
