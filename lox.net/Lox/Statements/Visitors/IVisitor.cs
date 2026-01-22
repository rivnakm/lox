namespace Lox.Statements.Visitors;

public interface IVisitor<out T> {
    T Visit(BlockStatement stmt);
    T Visit(ClassStatement stmt);
    T Visit(ExpressionStatement stmt);
    T Visit(FunctionStatement stmt);
    T Visit(IfStatement stmt);
    T Visit(PrintStatement stmt);
    T Visit(ReturnStatement stmt);
    T Visit(VarStatement stmt);
    T Visit(WhileStatement stmt);
}
