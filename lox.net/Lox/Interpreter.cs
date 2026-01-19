using Lox.Exceptions;
using Lox.Expressions;
using Lox.Statements;
using Lox.Utility;

namespace Lox;

public class Interpreter : Expressions.Visitors.IVisitor<object?>, Statements.Visitors.IVisitor<Unit> {
    private LoxEnvironment _environment = new();

    public void Interpret(IEnumerable<Statement> statements) {
        try {
            foreach (var stmt in statements) {
                this.Execute(stmt);
            }
        }
        catch (RuntimeException ex) {
            Console.Error.WriteLine(ex.Message);
        }
    }

    public object? Visit(BinaryExpression expr) {
        var left = this.Evaluate(expr.Left);
        var right = this.Evaluate(expr.Right);

        // FIXME: as-is, type checking happens well within the overloaded operator, so we lose info about where the error
        // happened in source code. Not what we want
        switch (expr.Operator.Type) {
            case TokenType.Greater:
                InterpreterValueUtility.CheckNumbers(expr.Operator, left, right);
                return (double)left! > (double)right!;
            case TokenType.GreaterEqual:
                InterpreterValueUtility.CheckNumbers(expr.Operator, left, right);
                return (double)left! >= (double)right!;
            case TokenType.Less:
                InterpreterValueUtility.CheckNumbers(expr.Operator, left, right);
                return (double)left! < (double)right!;
            case TokenType.LessEqual:
                InterpreterValueUtility.CheckNumbers(expr.Operator, left, right);
                return (double)left! <= (double)right!;
            case TokenType.BangEqual:
                return !InterpreterValueUtility.AreEqual(left, right);
            case TokenType.EqualEqual:
                return InterpreterValueUtility.AreEqual(left, right);
            case TokenType.Minus:
                InterpreterValueUtility.CheckNumbers(expr.Operator, left, right);
                return (double)left! - (double)right!;
            case TokenType.Slash:
                InterpreterValueUtility.CheckNumbers(expr.Operator, left, right);
                return (double)left! / (double)right!;
            case TokenType.Star:
                InterpreterValueUtility.CheckNumbers(expr.Operator, left, right);
                return (double)left! * (double)right!;
            case TokenType.Plus:
                if (left is string leftStr && right is string rightStr) {
                    return leftStr + rightStr;
                }

                if (left is double leftDouble && right is double rightDouble) {
                    return leftDouble + rightDouble;
                }

                throw new InvalidTypeException(expr.Operator, "Operands must be two numbers or two strings");
        }

        // unreachable
        throw new InvalidOperationException();
    }

    public object? Visit(Grouping expr) {
        return this.Evaluate(expr.Expression);
    }

    public object? Visit(Literal expr) {
        return expr.Value;
    }

    public object? Visit(UnaryExpression expr) {
        var right = this.Evaluate(expr.Right);

        switch (expr.Operator.Type) {
            case TokenType.Bang:
                return !InterpreterValueUtility.IsTruthy(right);
            case TokenType.Minus:
                InterpreterValueUtility.CheckNumber(expr.Operator, right);
                return -((double)right!);
        }

        // unreachable
        throw new InvalidOperationException();
    }

    public object? Visit(Variable expr) {
        return this._environment.Get(expr.Name);
    }

    public object? Visit(Assignment expr) {
        var value = this.Evaluate(expr.Value);
        this._environment.Assign(expr.Name, value);
        return value;
    }

    public Unit Visit(ExpressionStatement stmt) {
        this.Evaluate(stmt.Expression);
        return default;
    }

    public Unit Visit(PrintStatement stmt) {
        var value = this.Evaluate(stmt.Expression);
        Console.WriteLine(InterpreterValueUtility.Format(value));
        return default;
    }

    public Unit Visit(VarStatement stmt) {
        object? value = null;
        if (stmt.Initializer is not null) {
            value = this.Evaluate(stmt.Initializer);
        }

        this._environment.Define(stmt.Name, value);
        return default;
    }

    public Unit Visit(BlockStatement stmt) {
        this.ExecuteBlock(stmt.Statements, new LoxEnvironment(this._environment));
        return default;
    }

    private object? Evaluate(Expression expr) {
        return expr.Accept(this);
    }

    private void ExecuteBlock(List<Statement> statements, LoxEnvironment env) {
        var previous = this._environment;
        try {
            this._environment = env;
            foreach (var stmt in statements) {
                this.Execute(stmt);
            }
        }
        finally {
            this._environment = previous;
        }
    }

    private void Execute(Statement statement) {
        statement.Accept(this);
    }
}
