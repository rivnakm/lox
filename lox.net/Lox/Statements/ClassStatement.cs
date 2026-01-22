using Lox.Expressions;
using Lox.Statements.Visitors;

namespace Lox.Statements;

public class ClassStatement : IStatement {
    public Token Name { get; }
    public VariableExpression? Superclass { get; }
    public IReadOnlyList<FunctionStatement> Methods { get; }

    public ClassStatement(Token name, VariableExpression? superclass, IReadOnlyList<FunctionStatement> methods) {
        this.Name = name;
        this.Superclass = superclass;
        this.Methods = methods;
    }

    public T Accept<T>(IVisitor<T> visitor) {
        return visitor.Visit(this);
    }
}
