using Lox.Exceptions;
using Lox.Expressions;
using Lox.Statements;

namespace Lox;

public sealed class Parser {
    private readonly IErrorContext _errorContext;
    private readonly IList<Token> _tokens;
    private int _current;

    private enum FunctionKind {
        Function,
        Method
    }

    public Parser(IList<Token> tokens, IErrorContext errorContext) {
        this._errorContext = errorContext;
        this._tokens = tokens;
        this._current = 0;
    }

    public IEnumerable<IStatement> Parse() {
        while (!this.IsAtEnd()) {
            var decl = this.Declaration();
            if (decl is not null) {
                yield return decl;
            }
        }
    }

    private IStatement? Declaration() {
        try {
            if (this.Match(TokenType.Class)) {
                return this.ClassDeclaration();
            }

            if (this.Match(TokenType.Fun)) {
                return this.Function(FunctionKind.Function);
            }

            if (this.Match(TokenType.Var)) {
                return this.VarDeclaration();
            }

            return this.Statement();
        }
        catch (InvalidSyntaxException) {
            this.Synchronize();
            return null;
        }
    }

    private ClassStatement ClassDeclaration() {
        var name = this.Consume(TokenType.Identifier, "Expect class name");

        VariableExpression? superclass = null;
        if (this.Match(TokenType.Less)) {
            this.Consume(TokenType.Identifier, "Expect superclass name");
            superclass = new VariableExpression(this.Previous());
        }

        this.Consume(TokenType.LeftBrace, "Expect '{' before class body");

        var methods = new List<FunctionStatement>();
        while (!this.Check(TokenType.RightBrace) && !this.IsAtEnd()) {
            methods.Add(this.Function(FunctionKind.Method));
        }

        this.Consume(TokenType.RightBrace, "Expect '}' after class body");

        return new ClassStatement(name, superclass, methods);
    }

    private VarStatement VarDeclaration() {
        var name = this.Consume(TokenType.Identifier, "Expect variable name");

        IExpression? initializer = null;
        if (this.Match(TokenType.Equal)) {
            initializer = this.Expression();
        }

        this.Consume(TokenType.Semicolon, "Expect ';' after variable declaration");
        return new VarStatement(name, initializer);
    }

    private IStatement Statement() {
        if (this.Match(TokenType.For)) {
            return this.ForStatement();
        }

        if (this.Match(TokenType.If)) {
            return this.IfStatement();
        }

        if (this.Match(TokenType.Print)) {
            return this.PrintStatement();
        }

        if (this.Match(TokenType.Return)) {
            return this.ReturnStatement();
        }

        if (this.Match(TokenType.While)) {
            return this.WhileStatement();
        }

        if (this.Match(TokenType.LeftBrace)) {
            return new BlockStatement(this.Block());
        }

        return this.ExpressionStatement();
    }

    private ExpressionStatement ExpressionStatement() {
        var expr = this.Expression();
        this.Consume(TokenType.Semicolon, "Expect ';' after expression");
        return new ExpressionStatement(expr);
    }

    private IStatement ForStatement() {
        this.Consume(TokenType.LeftParen, "Expect '(' after 'for'");

        IStatement? initializer;
        if (this.Match(TokenType.Semicolon)) {
            initializer = null;
        }
        else if (this.Match(TokenType.Var)) {
            initializer = this.VarDeclaration();
        }
        else {
            initializer = this.ExpressionStatement();
        }

        IExpression? condition = null;
        if (!this.Check(TokenType.Semicolon)) {
            condition = this.Expression();
        }

        this.Consume(TokenType.Semicolon, "Expect ';' after loop condition");

        IExpression? increment = null;
        if (!this.Check(TokenType.RightParen)) {
            increment = this.Expression();
        }

        this.Consume(TokenType.RightParen, "Expect ')' after 'for' clauses");

        var body = this.Statement();
        if (increment is not null) {
            body = new BlockStatement([body, new ExpressionStatement(increment)]);
        }

        condition ??= new LiteralExpression(true);
        body = new WhileStatement(condition, body);

        if (initializer is not null) {
            body = new BlockStatement([initializer, body]);
        }

        return body;
    }

    private IfStatement IfStatement() {
        this.Consume(TokenType.LeftParen, "Expect ')' after if");
        var condition = this.Expression();
        this.Consume(TokenType.RightParen, "Expect ')' after condition");

        var thenBranch = this.Statement();
        IStatement? elseBranch = null;
        if (this.Match(TokenType.Else)) {
            elseBranch = this.Statement();
        }

        return new IfStatement(condition, thenBranch, elseBranch);
    }

    private PrintStatement PrintStatement() {
        var expr = this.Expression();
        this.Consume(TokenType.Semicolon, "Expect ';' after value");
        return new PrintStatement(expr);
    }

    private ReturnStatement ReturnStatement() {
        var keyword = this.Previous();
        IExpression? value = null;
        if (!this.Check(TokenType.Semicolon)) {
            value = this.Expression();
        }

        this.Consume(TokenType.Semicolon, "Expect ';' after return value");
        return new ReturnStatement(keyword, value);
    }

    private WhileStatement WhileStatement() {
        this.Consume(TokenType.LeftParen, "Expect '(' after 'while'");
        var condition = this.Expression();
        this.Consume(TokenType.RightParen, "Expect ')' after condition");
        var body = this.Statement();

        return new WhileStatement(condition, body);
    }

    private FunctionStatement Function(FunctionKind kind) {
        var name = this.Consume(TokenType.Identifier, $"Expect {kind.ToString().ToLowerInvariant()} name");
        this.Consume(TokenType.LeftParen, $"Expect '(' after {kind.ToString().ToLowerInvariant()} name");
        var parameters = new List<Token>();
        if (!this.Check(TokenType.RightParen)) {
            do {
                if (parameters.Count >= 255) {
                    this.Error(this.Peek(), "Cannot have more than 255 parameters");
                }

                parameters.Add(this.Consume(TokenType.Identifier, "Expect parameter name"));
            } while (this.Match(TokenType.Comma));
        }

        this.Consume(TokenType.RightParen, "Expect ')' after parameters");
        this.Consume(TokenType.LeftBrace, $"Expect '{{' before {kind.ToString().ToLowerInvariant()} body");

        var body = this.Block();
        return new FunctionStatement(name, parameters, body);
    }

    private List<IStatement> Block() {
        var statements = new List<IStatement>();

        while (!this.Check(TokenType.RightBrace) && !this.IsAtEnd()) {
            var decl = this.Declaration();
            if (decl is not null) {
                statements.Add(decl);
            }
        }

        this.Consume(TokenType.RightBrace, "Expect '}' after block");
        return statements;
    }

    private IExpression Expression() {
        return this.Assignment();
    }

    private IExpression Assignment() {
        var expr = this.Or();

        if (this.Match(TokenType.Equal)) {
            var equals = this.Previous();
            var value = this.Assignment();

            if (expr is VariableExpression varExpr) {
                return new AssignmentExpression(varExpr.Name, value);
            }
            else if (expr is GetExpression getExpr) {
                return new SetExpression(getExpr.Object, getExpr.Name, value);
            }

            this.Error(equals, "Invalid assignment target");
        }

        return expr;
    }

    private IExpression Or() {
        var expr = this.And();

        while (this.Match(TokenType.Or)) {
            var op = this.Previous();
            var right = this.And();
            expr = new LogicalExpression(expr, op, right);
        }

        return expr;
    }

    private IExpression And() {
        var expr = this.Equality();

        while (this.Match(TokenType.And)) {
            var op = this.Previous();
            var right = this.Equality();
            expr = new LogicalExpression(expr, op, right);
        }

        return expr;
    }

    private IExpression Equality() {
        var expr = this.Comparison();

        while (this.Match(TokenType.BangEqual, TokenType.EqualEqual)) {
            var op = this.Previous();
            var right = this.Comparison();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private IExpression Comparison() {
        var expr = this.Term();

        while (this.Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual)) {
            var op = this.Previous();
            var right = this.Term();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private IExpression Term() {
        var expr = this.Factor();

        while (this.Match(TokenType.Minus, TokenType.Plus)) {
            var op = this.Previous();
            var right = this.Factor();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private IExpression Factor() {
        var expr = this.Unary();

        while (this.Match(TokenType.Slash, TokenType.Star)) {
            var op = this.Previous();
            var right = this.Unary();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private IExpression Unary() {
        if (this.Match(TokenType.Bang, TokenType.Minus)) {
            var op = this.Previous();
            var right = this.Unary();
            return new UnaryExpression(op, right);
        }

        return this.Call();
    }

    private IExpression Call() {
        var expr = this.Primary();

        while (true) {
            if (this.Match(TokenType.LeftParen)) {
                expr = this.FinishCall(expr);
            }
            else if (this.Match(TokenType.Dot)) {
                var name = this.Consume(TokenType.Identifier, "Expect property name after '.'");
                expr = new GetExpression(expr, name);
            }
            else {
                break;
            }
        }

        return expr;
    }

    private CallExpression FinishCall(IExpression callee) {
        var arguments = new List<IExpression>();
        if (!this.Check(TokenType.RightParen)) {
            do {
                if (arguments.Count >= 255) {
                    this.Error(this.Peek(), "Cannot have more than 255 arguments");
                }

                arguments.Add(this.Expression());
            } while (this.Match(TokenType.Comma));
        }

        var paren = this.Consume(TokenType.RightParen, "Expect ')' after arguments");

        return new CallExpression(callee, paren, arguments);
    }

    private IExpression Primary() {
        if (this.Match(TokenType.False)) {
            return new LiteralExpression(false);
        }

        if (this.Match(TokenType.True)) {
            return new LiteralExpression(true);
        }

        if (this.Match(TokenType.Nil)) {
            return new LiteralExpression(null);
        }


        if (this.Match(TokenType.Number, TokenType.String)) {
            return new LiteralExpression(this.Previous().Value);
        }

        if (this.Match(TokenType.Super)) {
            var keyword = this.Previous();
            this.Consume(TokenType.Dot, "Expect '.' after 'super'");
            var method = this.Consume(TokenType.Identifier, "Expect superclass method name");
            return new SuperExpression(keyword, method);
        }

        if (this.Match(TokenType.This)) {
            return new ThisExpression(this.Previous());
        }

        if (this.Match(TokenType.Identifier)) {
            return new VariableExpression(this.Previous());
        }

        if (this.Match(TokenType.LeftParen)) {
            var expr = this.Expression();
            this.Consume(TokenType.RightParen, "Expect ')' after expression");
            return new GroupingExpression(expr);
        }

        throw this.Error(this.Peek(), "Expect expression");
    }

    private bool Match(params TokenType[] tokenTypes) {
        // ReSharper disable once InvertIf
        if (tokenTypes.Any(this.Check)) {
            this.Advance();
            return true;
        }

        return false;
    }

    private bool Check(TokenType tokenType) {
        if (this.IsAtEnd()) {
            return false;
        }

        return this.Peek().Type == tokenType;
    }

    private Token Advance() {
        if (!this.IsAtEnd()) {
            this._current++;
        }

        return this.Previous();
    }

    private bool IsAtEnd() {
        return this.Peek().Type == TokenType.Eof;
    }

    private Token Peek() {
        return this._tokens[this._current];
    }

    private Token Previous() {
        return this._tokens[this._current - 1];
    }

    private Token Consume(TokenType tokenType, string message) {
        if (this.Check(tokenType)) {
            return this.Advance();
        }

        throw this.Error(this.Peek(), message);
    }

    private void Synchronize() {
        this.Advance();

        while (!this.IsAtEnd()) {
            if (this.Previous().Type == TokenType.Semicolon) {
                return;
            }

            if (this.Peek().Type is
                TokenType.Class or
                TokenType.Fun or
                TokenType.Var or
                TokenType.For or
                TokenType.If or
                TokenType.While or
                TokenType.Print or
                TokenType.Return) {
                return;
            }

            this.Advance();
        }
    }

    private InvalidSyntaxException Error(Token token, string message) {
        this._errorContext.Error(token, message);

        return new InvalidSyntaxException(token, message);
    }
}
