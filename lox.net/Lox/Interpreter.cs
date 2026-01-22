using System.Diagnostics;
using Lox.Exceptions;
using Lox.Expressions;
using Lox.Extensions;
using Lox.Statements;
using Lox.Utility;

namespace Lox;

public class Interpreter : Expressions.Visitors.IVisitor<object?>, Statements.Visitors.IVisitor<Unit> {
    public LoxEnvironment Globals { get; }
    private LoxEnvironment _environment;
    private readonly Dictionary<IExpression, int> _locals = new();

    public Interpreter() {
        this.Globals = new LoxEnvironment();
        this._environment = this.Globals;

        this.Globals.Define("clock", LoxStdLib.Clock);
    }

    public void Interpret(IEnumerable<IStatement> statements) {
        try {
            foreach (var stmt in statements) {
                this.Execute(stmt);
            }
        }
        catch (RuntimeException ex) {
            Console.Error.WriteLine(ex.Message);
        }
    }

    public void Resolve(IExpression expr, int depth) {
        this._locals[expr] = depth;
    }

    private object? LookUpVariable(string name, IExpression expr) {
        if (this._locals.TryGetValue(expr, out var distance)) {
            return this._environment.GetAt(distance, name);
        }
        else {
            return this.Globals.Get(name);
        }
    }

    private object? Evaluate(IExpression expr) {
        return expr.Accept(this);
    }

    internal void ExecuteBlock(IEnumerable<IStatement> statements, LoxEnvironment env) {
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

    private void Execute(IStatement statement) {
        statement.Accept(this);
    }

    #region Expression Visitor Methods

    public object? Visit(AssignmentExpression expr) {
        var value = this.Evaluate(expr.Value);
        if (this._locals.TryGetValue(expr, out var distance)) {
            this._environment.AssignAt(distance, expr.Name.Lexeme, value);
        }
        else {
            this.Globals.Assign(expr.Name.Lexeme, value);
        }

        return value;
    }

    public object? Visit(BinaryExpression expr) {
        var left = this.Evaluate(expr.Left);
        var right = this.Evaluate(expr.Right);

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

    public object? Visit(CallExpression expr) {
        var callee = this.Evaluate(expr.Callee);

        var args = expr.Arguments.Select(this.Evaluate).ToList();

        if (callee is not ILoxCallable callable) {
            throw new InvalidTypeException(expr.Paren, "Can only call functions and classes");
        }

        if (args.Count != callable.Arity()) {
            throw new InvalidArgumentsException($"Expected {callable.Arity()} arguments, got {args.Count}");
        }

        return callable.Call(this, args);
    }

    public object? Visit(GetExpression expr) {
        var obj = this.Evaluate(expr.Object);
        if (obj is not LoxInstance instance) {
            throw new InvalidTypeException("Only instances have properties");
        }

        return instance.Get(expr.Name.Lexeme);
    }

    public object? Visit(GroupingExpression expr) {
        return this.Evaluate(expr.Expression);
    }

    public object? Visit(LiteralExpression expr) {
        return expr.Value;
    }

    public object? Visit(LogicalExpression expr) {
        var left = this.Evaluate(expr.Left);

        if (expr.Operator.Type is TokenType.Or) {
            if (left.IsTruthy()) {
                return left;
            }
        }
        else {
            if (!left.IsTruthy()) {
                return left;
            }
        }

        return this.Evaluate(expr.Right);
    }

    public object? Visit(SetExpression expr) {
        var obj = this.Evaluate(expr.Object);
        if (obj is not LoxInstance instance) {
            throw new InvalidTypeException("Only instances have fields");
        }

        var value = this.Evaluate(expr.Value);
        instance.Set(expr.Name.Lexeme, value);
        return value;
    }

    public object? Visit(SuperExpression expr) {
        var distance = this._locals[expr];
        var superClass = this._environment.GetAt(distance, "super") as LoxClass;
        Debug.Assert(superClass is not null);

        var obj = this._environment.GetAt(distance - 1, "this") as LoxInstance;
        Debug.Assert(obj is not null);

        var method = superClass.FindMethod(expr.Method.Lexeme);

        if (method is null) {
            throw new InvalidMemberException($"Undefined property '{expr.Method.Lexeme}'");
        }

        return method.Bind(obj);
    }

    public object? Visit(ThisExpression expr) {
        return this.LookUpVariable(expr.Keyword.Lexeme, expr);
    }

    public object? Visit(UnaryExpression expr) {
        var right = this.Evaluate(expr.Right);

        switch (expr.Operator.Type) {
            case TokenType.Bang:
                return !right.IsTruthy();
            case TokenType.Minus:
                InterpreterValueUtility.CheckNumber(expr.Operator, right);
                return -(double)right!;
        }

        // unreachable
        throw new InvalidOperationException();
    }

    public object? Visit(VariableExpression expr) {
        return this.LookUpVariable(expr.Name.Lexeme, expr);
    }

    #endregion

    #region Statement Visitor Methods

    public Unit Visit(BlockStatement stmt) {
        this.ExecuteBlock(stmt.Statements, new LoxEnvironment(this._environment));
        return default;
    }

    public Unit Visit(ClassStatement stmt) {
        object? superclass = null;
        if (stmt.Superclass is not null) {
            superclass = this.Evaluate(stmt.Superclass);
            if (superclass is not LoxClass) {
                throw new InvalidTypeException(stmt.Superclass.Name, "Superclass must be a class");
            }
        }

        this._environment.Define(stmt.Name.Lexeme, null);

        if (stmt.Superclass is not null) {
            this._environment = new LoxEnvironment(this._environment);
            this._environment.Define("super", superclass);
        }

        var methods = new Dictionary<string, LoxFunction>();
        foreach (var method in stmt.Methods) {
            var func = new LoxFunction(method, this._environment, method.Name.Lexeme == "init");
            methods[method.Name.Lexeme] = func;
        }

        var @class = new LoxClass(stmt.Name.Lexeme, superclass as LoxClass, methods);
        if (superclass is not null) {
            Debug.Assert(this._environment.Parent is not null);
            this._environment = this._environment.Parent;
        }

        this._environment.Assign(stmt.Name.Lexeme, @class);
        return default;
    }

    public Unit Visit(ExpressionStatement stmt) {
        this.Evaluate(stmt.Expression);
        return default;
    }

    public Unit Visit(FunctionStatement stmt) {
        var func = new LoxFunction(stmt, this._environment, false);
        this._environment.Define(stmt.Name.Lexeme, func);
        return default;
    }

    public Unit Visit(IfStatement stmt) {
        if (InterpreterValueUtility.IsTruthy(this.Evaluate(stmt.Condition))) {
            this.Execute(stmt.Then);
        }
        else if (stmt.Else is not null) {
            this.Execute(stmt.Else);
        }

        return default;
    }

    public Unit Visit(PrintStatement stmt) {
        var value = this.Evaluate(stmt.Expression);
        Console.WriteLine(InterpreterValueUtility.Format(value));
        return default;
    }

    public Unit Visit(ReturnStatement stmt) {
        object? value = null;
        if (stmt.Value is not null) {
            value = this.Evaluate(stmt.Value);
        }

        throw new Return(value);
    }

    public Unit Visit(VarStatement stmt) {
        object? value = null;
        if (stmt.Initializer is not null) {
            value = this.Evaluate(stmt.Initializer);
        }

        this._environment.Define(stmt.Name.Lexeme, value);
        return default;
    }

    public Unit Visit(WhileStatement stmt) {
        while (this.Evaluate(stmt.Condition).IsTruthy()) {
            this.Execute(stmt.Body);
        }

        return default;
    }

    #endregion

}
