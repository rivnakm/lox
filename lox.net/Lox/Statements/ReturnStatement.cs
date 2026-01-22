using Lox.Expressions;
using Lox.Statements.Visitors;

namespace Lox.Statements;

public class ReturnStatement : IStatement {
    public Token Keyword { get; }
    public IExpression? Value { get; }

    public ReturnStatement(Token keyword, IExpression? value) {
        this.Keyword = keyword;
        this.Value = value;
    }

    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
