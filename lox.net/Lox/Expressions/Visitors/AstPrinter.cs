using System.Text;

namespace Lox.Expressions.Visitors;

public class AstPrinter : IVisitor<string> {
    public string Print(Expression expr) => expr.Accept(this);

    public string Visit(BinaryExpression binExpr) {
        return this.Parenthesize(binExpr.Operator.Lexeme, binExpr.Left, binExpr.Right);
    }

    public string Visit(Grouping grouping) {
        return this.Parenthesize("group", grouping.Expression);
    }

    public string Visit(Literal literal) {
        return literal.Value?.ToString() ?? "nil";
    }

    public string Visit(UnaryExpression unaryExpr) {
        return this.Parenthesize(unaryExpr.Operator.Lexeme, unaryExpr.Right);
    }

    private string Parenthesize(string name, params Expression[] exprs) {
        var builder = new StringBuilder();

        builder.Append('(');
        builder.Append(name);

        foreach (var expr in exprs) {
            builder.Append(' ');
            builder.Append(expr.Accept(this));
        }

        builder.Append(')');

        return builder.ToString();
    }
}
