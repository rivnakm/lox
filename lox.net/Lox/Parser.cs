using Lox.Exceptions;
using Lox.Expressions;
using Lox.Statements;

namespace Lox;

public sealed class Parser {
    private readonly IErrorContext _errorContext;
    private readonly IList<Token> _tokens;
    private int _current;

    public Parser(IList<Token> tokens, IErrorContext errorContext) {
        this._errorContext = errorContext;
        this._tokens = tokens;
        this._current = 0;
    }

    public IEnumerable<Statement> Parse() {
        while (!this.IsAtEnd()) {
            var decl = this.Declaration();
            if (decl is not null) {
                yield return decl;
            }
        }
    }

    private Statement? Declaration() {
        try {
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

    private VarStatement VarDeclaration() {
        var name = this.Consume(TokenType.Identifier, "Expect variable name.");

        Expression? initializer = null;
        if (this.Match(TokenType.Equal)) {
            initializer = this.Expression();
        }

        this.Consume(TokenType.Semicolon, "Expect ';' after variable declaratio.");
        return new VarStatement(name, initializer);
    }

    private Statement Statement() {
        if (this.Match(TokenType.Print)) {
            return this.PrintStatement();
        }

        if (this.Match(TokenType.LeftBrace)) {
            return new BlockStatement(this.Block());
        }

        return this.ExpressionStatement();
    }

    private PrintStatement PrintStatement() {
        var expr = this.Expression();
        this.Consume(TokenType.Semicolon, "Expect ';' after expression.");
        return new PrintStatement(expr);
    }

    private ExpressionStatement ExpressionStatement() {
        var expr = this.Expression();
        this.Consume(TokenType.Semicolon, "Expect ';' after expression.");
        return new ExpressionStatement(expr);
    }

    private List<Statement> Block() {
        var statements = new List<Statement>();

        while (!this.Check(TokenType.RightBrace) && !this.IsAtEnd()) {
            var decl = this.Declaration();
            if (decl is not null) {
                statements.Add(decl);
            }
        }

        this.Consume(TokenType.RightBrace, "Expect '}' after block.");
        return statements;
    }

    private Expression Expression() {
        return this.Assignment();
    }

    private Expression Assignment() {
        var expr = this.Equality();

        if (this.Match(TokenType.Equal)) {
            var equals = this.Previous();
            var value = this.Assignment();

            if (expr is Variable varExpr) {
                return new Assignment(varExpr.Name, value);
            }

            this.Error(equals, "Invalid assignment target.");
        }

        return expr;
    }

    private Expression Equality() {
        var expr = this.Comparison();

        while (this.Match(TokenType.BangEqual, TokenType.EqualEqual)) {
            var op = this.Previous();
            var right = this.Comparison();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private Expression Comparison() {
        var expr = this.Term();

        while (this.Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual)) {
            var op = this.Previous();
            var right = this.Term();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private Expression Term() {
        var expr = this.Factor();

        while (this.Match(TokenType.Minus, TokenType.Plus)) {
            var op = this.Previous();
            var right = this.Factor();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private Expression Factor() {
        var expr = this.Unary();

        while (this.Match(TokenType.Slash, TokenType.Star)) {
            var op = this.Previous();
            var right = this.Unary();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    private Expression Unary() {
        if (this.Match(TokenType.Bang, TokenType.Minus)) {
            var op = this.Previous();
            var right = this.Unary();
            return new UnaryExpression(op, right);
        }

        return this.Primary();
    }

    private Expression Primary() {
        if (this.Match(TokenType.False)) {
            return new Literal(false);
        }

        if (this.Match(TokenType.True)) {
            return new Literal(true);
        }

        if (this.Match(TokenType.Nil)) {
            return new Literal(null);
        }

        if (this.Match(TokenType.Number, TokenType.String)) {
            return new Literal(this.Previous().Value);
        }

        if (this.Match(TokenType.Identifier)) {
            return new Variable(this.Previous());
        }

        if (this.Match(TokenType.LeftParen)) {
            var expr = this.Expression();
            this.Consume(TokenType.RightParen, "Expect ')' after expression.");
            return new Grouping(expr);
        }

        throw this.Error(this.Peek(), "Expect expression.");
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
