using Lox.Exceptions;
using Lox.Utility;

namespace Lox.Expressions.Visitors;

public class Interpreter : IVisitor<object?> {
    public void Interpret(Expression expr) {
        try {
            var value = this.Evaluate(expr);
            Console.WriteLine(InterpreterValueUtility.Format(value));
        }
        catch (RuntimeException ex) {
            Console.Error.WriteLine(ex.Message);
        }
    }

    public object? Visit(BinaryExpression binExpr) {
        var left = this.Evaluate(binExpr.Left);
        var right = this.Evaluate(binExpr.Right);

        // FIXME: as-is, type checking happens well within the overloaded operator, so we lose info about where the error
        // happened in source code. Not what we want
        switch (binExpr.Operator.Type) {
            case TokenType.Greater:
                InterpreterValueUtility.CheckNumbers(binExpr.Operator, left, right);
                return (double)left! > (double)right!;
            case TokenType.GreaterEqual:
                InterpreterValueUtility.CheckNumbers(binExpr.Operator, left, right);
                return (double)left! >= (double)right!;
            case TokenType.Less:
                InterpreterValueUtility.CheckNumbers(binExpr.Operator, left, right);
                return (double)left! < (double)right!;
            case TokenType.LessEqual:
                InterpreterValueUtility.CheckNumbers(binExpr.Operator, left, right);
                return (double)left! <= (double)right!;
            case TokenType.BangEqual:
                return !InterpreterValueUtility.AreEqual(left, right);
            case TokenType.EqualEqual:
                return InterpreterValueUtility.AreEqual(left, right);
            case TokenType.Minus:
                InterpreterValueUtility.CheckNumbers(binExpr.Operator, left, right);
                return (double)left! - (double)right!;
            case TokenType.Slash:
                InterpreterValueUtility.CheckNumbers(binExpr.Operator, left, right);
                return (double)left! / (double)right!;
            case TokenType.Star:
                InterpreterValueUtility.CheckNumbers(binExpr.Operator, left, right);
                return (double)left! * (double)right!;
            case TokenType.Plus:
                if (left is string leftStr && right is string rightStr) {
                    return leftStr + rightStr;
                }

                if (left is double leftDouble && right is double rightDouble) {
                    return leftDouble + rightDouble;
                }

                throw new InvalidTypeException(binExpr.Operator, "Operands must be two numbers or two strings");
        }

        // unreachable
        throw new InvalidOperationException();
    }

    public object? Visit(Grouping grouping) {
        return this.Evaluate(grouping.Expression);
    }

    public object? Visit(Literal literal) {
        return literal.Value;
    }

    public object? Visit(UnaryExpression unaryExpr) {
        var right = this.Evaluate(unaryExpr.Right);

        switch (unaryExpr.Operator.Type) {
            case TokenType.Bang:
                return !InterpreterValueUtility.IsTruthy(right);
            case TokenType.Minus:
                InterpreterValueUtility.CheckNumber(unaryExpr.Operator, right);
                return -((double)right!);
        }

        // unreachable
        throw new InvalidOperationException();
    }

    private object? Evaluate(Expression expr) {
        return expr.Accept(this);
    }
}
