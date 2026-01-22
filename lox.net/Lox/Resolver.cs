using Lox.Expressions;
using Lox.Statements;

namespace Lox;

public class Resolver : Expressions.Visitors.IVisitor<Unit>, Statements.Visitors.IVisitor<Unit> {
    private readonly Interpreter _interpreter;
    private readonly IErrorContext _errorContext;
    private readonly Stack<Dictionary<string, bool>> _scopes = new();
    private FunctionType _currentFunction = FunctionType.None;
    private ClassType _currentClass = ClassType.None;

    public Resolver(Interpreter interpreter, IErrorContext errorContext) {
        this._interpreter = interpreter;
        this._errorContext = errorContext;
    }

    private enum FunctionType {
        None,
        Function,
        Method,
        Initializer
    }

    private enum ClassType {
        None,
        Class,
        Subclass
    }

    public void Resolve(IEnumerable<IStatement> statements) {
        foreach (var stmt in statements) {
            this.Resolve(stmt);
        }
    }

    private void Resolve(IStatement stmt) {
        stmt.Accept(this);
    }

    private void Resolve(IExpression expr) {
        expr.Accept(this);
    }

    private void ResolveFunction(FunctionStatement func, FunctionType funcType) {
        var enclosingFunction = this._currentFunction;
        this._currentFunction = funcType;

        this.BeginScope();
        foreach (var param in func.Parameters) {
            this.Declare(param);
            this.Define(param);
        }

        this.Resolve(func.Body);
        this.EndScope();

        this._currentFunction = enclosingFunction;
    }

    private void BeginScope() {
        this._scopes.Push(new Dictionary<string, bool>());
    }

    private void EndScope() {
        this._scopes.Pop();
    }

    private void Declare(Token name) {
        if (this._scopes.Count == 0) {
            return;
        }

        var scope = this._scopes.Peek();
        if (!scope.TryAdd(name.Lexeme, false)) {
            this._errorContext.Error(name, "Already a variable with this name in this scope");
        }
    }

    private void Define(Token name) {
        if (this._scopes.Count == 0) {
            return;
        }

        this._scopes.Peek()[name.Lexeme] = true;
    }

    private void ResolveLocal(IExpression expr, Token name) {
        var scopesList = this._scopes.ToList();
        for (var i = 0; i < scopesList.Count; i++) {
            if (scopesList[i].ContainsKey(name.Lexeme)) {
                this._interpreter.Resolve(expr, i);
                return;
            }
        }
    }

    #region Expression Visitor Methods

    public Unit Visit(AssignmentExpression expr) {
        this.Resolve(expr.Value);
        this.ResolveLocal(expr, expr.Name);
        return default;
    }

    public Unit Visit(BinaryExpression expr) {
        this.Resolve(expr.Left);
        this.Resolve(expr.Right);
        return default;
    }

    public Unit Visit(CallExpression expr) {
        this.Resolve(expr.Callee);

        foreach (var argument in expr.Arguments) {
            this.Resolve(argument);
        }

        return default;
    }

    public Unit Visit(GetExpression expr) {
        this.Resolve(expr.Object);
        return default;
    }

    public Unit Visit(GroupingExpression expr) {
        this.Resolve(expr.Expression);
        return default;
    }

    public Unit Visit(LiteralExpression expr) {
        return default;
    }

    public Unit Visit(LogicalExpression expr) {
        this.Resolve(expr.Left);
        this.Resolve(expr.Right);
        return default;
    }

    public Unit Visit(SetExpression expr) {
        this.Resolve(expr.Value);
        this.Resolve(expr.Object);
        return default;
    }

    public Unit Visit(SuperExpression expr) {
        if (this._currentClass is ClassType.None) {
            this._errorContext.Error(expr.Keyword, "Cannot use 'super' outside of a class");
        }
        else if (this._currentClass is not ClassType.Subclass) {
            this._errorContext.Error(expr.Keyword, "Cannot use 'super' in a class with no superclass");
        }

        this.ResolveLocal(expr, expr.Keyword);
        return default;
    }

    public Unit Visit(ThisExpression expr) {
        if (this._currentClass is ClassType.None) {
            this._errorContext.Error(expr.Keyword, "Cannot use 'this' outside of a class");
        }

        this.ResolveLocal(expr, expr.Keyword);
        return default;
    }

    public Unit Visit(UnaryExpression expr) {
        this.Resolve(expr.Right);
        return default;
    }

    public Unit Visit(VariableExpression expr) {
        if (this._scopes.Count > 0 && this._scopes.Peek().TryGetValue(expr.Name.Lexeme, out var value) && !value) {
            this._errorContext.Error(expr.Name, "Cannot read local variable in its own initializer");
        }

        this.ResolveLocal(expr, expr.Name);
        return default;
    }

    #endregion

    #region Statement Visitor Methods

    public Unit Visit(BlockStatement stmt) {
        this.BeginScope();
        this.Resolve(stmt.Statements);
        this.EndScope();
        return default;
    }

    public Unit Visit(ClassStatement stmt) {
        var enclosingClass = this._currentClass;
        this._currentClass = ClassType.Class;

        this.Declare(stmt.Name);
        this.Define(stmt.Name);

        if (stmt.Superclass is not null && stmt.Name.Lexeme == stmt.Superclass.Name.Lexeme) {
            this._errorContext.Error(stmt.Superclass.Name, "A class cannot inherit from itself");
        }

        if (stmt.Superclass is not null) {
            this._currentClass = ClassType.Subclass;
            this.Resolve(stmt.Superclass);
        }

        if (stmt.Superclass is not null) {
            this.BeginScope();
            this._scopes.Peek()[SpecialNames.Super] = true;
        }

        this.BeginScope();
        this._scopes.Peek()[SpecialNames.This] = true;

        foreach (var method in stmt.Methods) {
            var decl = FunctionType.Method;

            if (method.Name.Lexeme == SpecialNames.Init) {
                decl = FunctionType.Initializer;
            }

            this.ResolveFunction(method, decl);
        }

        this.EndScope();

        if (stmt.Superclass is not null) {
            this.EndScope();
        }

        this._currentClass = enclosingClass;
        return default;
    }

    public Unit Visit(ExpressionStatement stmt) {
        this.Resolve(stmt.Expression);
        return default;
    }

    public Unit Visit(FunctionStatement stmt) {
        this.Declare(stmt.Name);
        this.Define(stmt.Name);

        this.ResolveFunction(stmt, FunctionType.Function);

        return default;
    }

    public Unit Visit(IfStatement stmt) {
        this.Resolve(stmt.Condition);
        this.Resolve(stmt.Then);
        if (stmt.Else is not null) {
            this.Resolve(stmt.Else);
        }

        return default;
    }

    public Unit Visit(PrintStatement stmt) {
        this.Resolve(stmt.Expression);
        return default;
    }

    public Unit Visit(ReturnStatement stmt) {
        if (this._currentFunction is FunctionType.None) {
            this._errorContext.Error(stmt.Keyword, "Cannot return from top-level code");
        }

        if (stmt.Value is not null) {
            if (this._currentFunction is FunctionType.Initializer) {
                this._errorContext.Error(stmt.Keyword, "Cannot return a value from an initializer");
            }

            this.Resolve(stmt.Value);
        }

        return default;
    }

    public Unit Visit(VarStatement stmt) {
        this.Declare(stmt.Name);
        if (stmt.Initializer is not null) {
            this.Resolve(stmt.Initializer);
        }

        this.Define(stmt.Name);
        return default;
    }

    public Unit Visit(WhileStatement stmt) {
        this.Resolve(stmt.Condition);
        this.Resolve(stmt.Body);
        return default;
    }

    #endregion

}
