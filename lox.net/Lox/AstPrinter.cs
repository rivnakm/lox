using System.Text;
using Lox.Expressions;
using Lox.Expressions.Visitors;
using Lox.Statements;

namespace Lox;

public class AstPrinter : IVisitor<string>, Statements.Visitors.IVisitor<string> {
    public string Print(IExpression expr) {
        return expr.Accept(this);
    }

    public string Print(IStatement stmt) {
        return stmt.Accept(this);
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

    public string Visit(ClassStatement stmt) {
        var builder = new StringBuilder();
        builder.Append("(class ");

        if (stmt.Superclass is not null) {
            builder.Append($" < {this.Print(stmt.Superclass)}");
        }

        foreach (var method in stmt.Methods) {
            builder.Append($" {this.Print(method)}");
        }

        builder.Append(')');
        return builder.ToString();
    }

    public string Visit(ExpressionStatement stmt) {
        return this.Parenthesize(";", stmt.Expression);
    }

    public string Visit(FunctionStatement stmt) {
        var builder = new StringBuilder();
        builder.Append($"(fun {stmt.Name.Lexeme}(");

        foreach (var param in stmt.Parameters) {
            if (param != stmt.Parameters[0]) {
                builder.Append(' ');
            }

            builder.Append(param.Lexeme);
        }

        builder.Append(") ");

        foreach (var body in stmt.Body) {
            builder.Append(body.Accept(this));
        }

        builder.Append(')');
        return builder.ToString();
    }

    public string Visit(IfStatement stmt) {
        throw new NotImplementedException();
    }

    public string Visit(PrintStatement stmt) {
        return this.Parenthesize("print", stmt.Expression);
    }

    public string Visit(ReturnStatement stmt) {
        throw new NotImplementedException();
    }

    public string Visit(VarStatement stmt) {
        if (stmt.Initializer is null) {
            return this.Parenthesize2("var", stmt.Name);
        }

        return this.Parenthesize2("var", stmt.Name, "=", stmt.Initializer);
    }

    public string Visit(WhileStatement stmt) {
        throw new NotImplementedException();
    }

    public string Visit(AssignmentExpression expr) {
        return this.Parenthesize2("=", expr.Name.Lexeme, expr.Value);
    }

    public string Visit(BinaryExpression expr) {
        return this.Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
    }

    public string Visit(CallExpression expr) {
        throw new NotImplementedException();
    }

    public string Visit(GetExpression expr) {
        throw new NotImplementedException();
    }

    public string Visit(GroupingExpression expr) {
        return this.Parenthesize("group", expr.Expression);
    }

    public string Visit(LiteralExpression expr) {
        return expr.Value?.ToString() ?? "nil";
    }

    public string Visit(LogicalExpression expr) {
        throw new NotImplementedException();
    }

    public string Visit(SetExpression expr) {
        throw new NotImplementedException();
    }

    public string Visit(SuperExpression expr) {
        throw new NotImplementedException();
    }

    public string Visit(ThisExpression expr) {
        throw new NotImplementedException();
    }

    public string Visit(UnaryExpression expr) {
        return this.Parenthesize(expr.Operator.Lexeme, expr.Right);
    }

    public string Visit(VariableExpression expr) {
        return expr.Name.Lexeme;
    }

    private string Parenthesize(string name, params IExpression[] exprs) {
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
                case IExpression expr:
                    builder.Append(expr.Accept(this));
                    break;
                case IStatement stmt:
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
