using System.Text;
using Lox.Expressions;
using Lox.Statements;

namespace Lox;

public class AstPrinter : Expressions.Visitors.IVisitor<string>, Statements.Visitors.IVisitor<string> {
    public string Print(Expression expr) => expr.Accept(this);

    public string Visit(ExpressionStatement stmt) {
        return this.Parenthesize(";", stmt.Expression);
    }

    public string Visit(PrintStatement stmt) {
        return this.Parenthesize("print", stmt.Expression);
    }

    public string Visit(VarStatement stmt) {
        if (stmt.Initializer is null) {
            return this.Parenthesize2("var", stmt.Name);
        }

        return this.Parenthesize2("var", stmt.Name, "=", stmt.Initializer);
    }

    public string Visit(BlockStatement stmt) {
        var builder = new StringBuilder();
        builder.Append("(block ");

        foreach (var s in stmt.Statements) {
            builder.Append(s.Accept(this));
        }

        builder.Append(')');
        return builder.ToString();
    }

    public string Visit(BinaryExpression expr) {
        return this.Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
    }

    public string Visit(Grouping expr) {
        return this.Parenthesize("group", expr.Expression);
    }

    public string Visit(Literal expr) {
        return expr.Value?.ToString() ?? "nil";
    }

    public string Visit(UnaryExpression expr) {
        return this.Parenthesize(expr.Operator.Lexeme, expr.Right);
    }

    public string Visit(Variable expr) {
        return expr.Name.Lexeme;
    }

    public string Visit(Assignment expr) {
        return this.Parenthesize2("=", expr.Name.Lexeme, expr.Value);
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

    private string Parenthesize2(string name, params object?[] parts) {
        var builder = new StringBuilder();

        builder.Append('(').Append(name);
        this.Transform(builder, parts);
        builder.Append(')');

        return builder.ToString();
    }

    private void Transform(StringBuilder builder, object?[] parts) {
        foreach (var part in parts) {
            builder.Append(part);
            switch (part) {
                case Expression expr:
                    builder.Append(expr.Accept(this));
                    break;
                case Statement stmt:
                    builder.Append(stmt.Accept(this));
                    break;
                case Token token:
                    builder.Append(token.Lexeme);
                    break;
                case IEnumerable<object?> list:
                    this.Transform(builder, list.ToArray());
                    break;
                default:
                    builder.Append(part?.ToString() ?? "nil");
                    break;
            }
        }
    }
}
