using Lox.Extensions;

namespace Lox;

public sealed class Lexer {
    private readonly IErrorContext _errorContext;
    private readonly string _source;
    private readonly List<Token> _tokens;
    private int _current;
    private int _lineNumber = 1;
    private int _start;

    public Lexer(string source, IErrorContext errorContext) {
        this._source = source;
        this._errorContext = errorContext;
        this._tokens = new List<Token>();
    }

    public IList<Token> GetTokens() {
        while (!this.IsAtEnd()) {
            this._start = this._current;
            this.ScanToken();
        }

        this._tokens.Add(new Token(TokenType.Eof, string.Empty, null, this._lineNumber));
        return this._tokens;
    }

    private void ScanToken() {
        if (this.IsAtEnd()) {
            return;
        }

        var ch = this.Advance();
        switch (ch) {
            case '(':
                this.AddToken(TokenType.LeftParen);
                break;
            case ')':
                this.AddToken(TokenType.RightParen);
                break;
            case '{':
                this.AddToken(TokenType.LeftBrace);
                break;
            case '}':
                this.AddToken(TokenType.RightBrace);
                break;
            case ',':
                this.AddToken(TokenType.Comma);
                break;
            case '.':
                this.AddToken(TokenType.Dot);
                break;
            case '-':
                this.AddToken(TokenType.Minus);
                break;
            case '+':
                this.AddToken(TokenType.Plus);
                break;
            case ';':
                this.AddToken(TokenType.Semicolon);
                break;
            case '*':
                this.AddToken(TokenType.Star);
                break;
            case '!':
                this.AddToken(this.Match('=') ? TokenType.BangEqual : TokenType.Bang);
                break;
            case '=':
                this.AddToken(this.Match('=') ? TokenType.EqualEqual : TokenType.Equal);
                break;
            case '<':
                this.AddToken(this.Match('=') ? TokenType.LessEqual : TokenType.Less);
                break;
            case '>':
                this.AddToken(this.Match('=') ? TokenType.GreaterEqual : TokenType.Greater);
                break;
            case '/':
                if (this.Match('/')) {
                    while (!this.IsAtEnd() && this.Peek() != '\n') {
                        this.Advance();
                    }
                }
                else {
                    this.AddToken(TokenType.Slash);
                }

                break;
            case ' ':
            case '\t':
            case '\r':
                break;
            case '\n':
                this._lineNumber++;
                break;
            case '"':
                this.String();
                break;
            default:
                if (char.IsDigit(ch)) {
                    this.Number();
                }
                else if (char.IsLetter(ch)) {
                    this.Identifier();
                }
                else {
                    this._errorContext.Error($"Unexpected character '{ch}'", this._lineNumber);
                }

                break;
        }
    }

    private void String() {
        while (this.Peek() != '"' && !this.IsAtEnd()) {
            if (this.Peek() == '\n') {
                this._lineNumber++;
            }

            this.Advance();
        }

        if (this.IsAtEnd()) {
            this._errorContext.Error("Unterminated string", this._lineNumber);
            return;
        }

        this.Advance(); // The closing `"`
        var str = this.GetLexeme().TrimExact('"', 1);
        this.AddToken(TokenType.String, str);
    }

    private void Number() {
        while (char.IsDigit(this.Peek())) {
            this.Advance();
        }

        if (this.Peek() == '.' && char.IsDigit(this.PeekNext())) {
            // consume the `.`
            this.Advance();

            while (char.IsDigit(this.Peek())) {
                this.Advance();
            }
        }

        var value = double.Parse(this.GetLexeme());
        this.AddToken(TokenType.Number, value);
    }

    private void Identifier() {
        while (IsIdentifier(this.Peek())) {
            this.Advance();
        }

        var text = this.GetLexeme();
        if (Keywords.TryGetTokenType(text, out var tokenType)) {
            this.AddToken(tokenType);
            return;
        }

        this.AddToken(TokenType.Identifier);
    }

    private char Advance() {
        return this._source[this._current++];
    }

    private char Peek() {
        return this.IsAtEnd() ? '\0' : this._source[this._current];
    }

    private char PeekNext() {
        return this._current + 1 >= this._source.Length ? '\0' : this._source[this._current + 1];
    }

    private bool Match(char expected) {
        if (this.Peek() != expected) {
            return false;
        }

        this.Advance();
        return true;
    }

    private string GetLexeme() {
        return this._source.Substring(this._start, this._current - this._start);
    }

    private void AddToken(TokenType type, object? value = null) {
        this._tokens.Add(new Token(type, this.GetLexeme(), value, this._lineNumber));
    }

    private bool IsAtEnd() {
        return this._current >= this._source.Length;
    }

    private static bool IsIdentifier(char ch) {
        return char.IsLetterOrDigit(ch) || ch == '_';
    }
}
