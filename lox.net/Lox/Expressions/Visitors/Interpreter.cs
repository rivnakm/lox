using Lox.Exceptions;

namespace Lox.Expressions.Visitors;

public class Interpreter : IVisitor<IValue>
{
    public void Interpret(Expression expr)
    {
        try
        {
            var value = this.Evaluate(expr);
            Console.WriteLine(value.ToString());
        }
        catch (RuntimeException ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }
    public IValue Visit(BinaryExpression binExpr)
    {
        var left = this.Evaluate(binExpr.Left);
        var right = this.Evaluate(binExpr.Right);

        switch (binExpr.Operator.Type)
        {
            case TokenType.Greater:
                return new Value(left.GetValue<double>() > right.GetValue<double>());
            case TokenType.GreaterEqual:
                return new Value(left.GetValue<double>() >= right.GetValue<double>());
            case TokenType.Less:
                return new Value(left.GetValue<double>() < right.GetValue<double>());
            case TokenType.LessEqual:
                return new Value(left.GetValue<double>() <= right.GetValue<double>());
            case TokenType.BangEqual:
                return new Value(!left.Equals(right));
            case TokenType.EqualEqual:
                return new Value(left.Equals(right));
            case TokenType.Minus:
                return new Value(left.GetValue<double>() - right.GetValue<double>());
            case TokenType.Slash:
                return new Value(left.GetValue<double>() / right.GetValue<double>());
            case TokenType.Star:
                return new Value(left.GetValue<double>() * right.GetValue<double>());
            case TokenType.Plus:
                if (left.TryGetValue<double>(out var leftDouble) && right.TryGetValue<double>(out var rightDouble))
                {
                    return new Value(leftDouble + rightDouble);
                }

                if (left.TryGetValue<string>(out var leftString) && right.TryGetValue<string>(out var rightString))
                {
                    return new Value(leftString + rightString);
                }

                break;
        }

        // unreachable
        throw new InvalidOperationException();
    }

    public IValue Visit(Grouping grouping)
    {
        return this.Evaluate(grouping.Expression);
    }

    public IValue Visit(Literal literal)
    {
        return new Value(literal.Value);
    }

    public IValue Visit(UnaryExpression unaryExpr)
    {
        var right = this.Evaluate(unaryExpr.Right);

        switch (unaryExpr.Operator.Type)
        {
            case TokenType.Bang:
                return new Value(!right.IsTruthy);
            case TokenType.Minus:
                return new Value(-right.GetValue<double>());
        }

        // unreachable
        throw new InvalidOperationException();
    }

    private IValue Evaluate(Expression expr)
    {
        return expr.Accept(this);
    }
}
