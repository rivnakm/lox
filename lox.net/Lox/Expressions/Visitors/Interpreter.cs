using Lox.Exceptions;

namespace Lox.Expressions.Visitors;

public class Interpreter : IVisitor<Value> {
    public void Interpret(Expression expr) {
        try {
            var value = this.Evaluate(expr);
            Console.WriteLine(value.ToString());
        }
        catch (RuntimeException ex) {
            Console.Error.WriteLine(ex.Message);
        }
    }
    public Value Visit(BinaryExpression binExpr) {
        var left = this.Evaluate(binExpr.Left);
        var right = this.Evaluate(binExpr.Right);

        // FIXME: as-is, type checking happens well within the overloaded operator, so we lose info about where the error
        // happened in source code. Not what we want
        switch (binExpr.Operator.Type) {
            case TokenType.Greater:
                return left > right;
            case TokenType.GreaterEqual:
                return left >= right;
            case TokenType.Less:
                return left < right;
            case TokenType.LessEqual:
                return left <= right;
            case TokenType.BangEqual:
                return left != right;
            case TokenType.EqualEqual:
                return left == right;
            case TokenType.Minus:
                return left - right;
            case TokenType.Slash:
                return left / right;
            case TokenType.Star:
                return left * right;
            case TokenType.Plus:
                return left + right;
        }

        // unreachable
        throw new InvalidOperationException();
    }

    public Value Visit(Grouping grouping) {
        return this.Evaluate(grouping.Expression);
    }

    public Value Visit(Literal literal) {
        return new Value(literal.Value);
    }

    public Value Visit(UnaryExpression unaryExpr) {
        var right = this.Evaluate(unaryExpr.Right);

        switch (unaryExpr.Operator.Type) {
            case TokenType.Bang:
                return !right;
            case TokenType.Minus:
                return -right;
        }

        // unreachable
        throw new InvalidOperationException();
    }

    private Value Evaluate(Expression expr) {
        return expr.Accept(this);
    }
}
