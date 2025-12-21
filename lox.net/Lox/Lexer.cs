using System.Text;
using Lox.Extensions;

namespace Lox;

public sealed class Lexer : IDisposable, IAsyncDisposable {
    private readonly Stream _stream;
    private readonly LookbackStreamReader _reader;
    private readonly IErrorContext _errorContext;
    private uint _lineNumber = 1;
    private StringBuilder _lexemeBuilder = new();
    private bool _builderEnabled = true;

    public static IEnumerable<Token> GetTokens(Stream stream, IErrorContext errorContext) {
        return new Lexer(stream, errorContext).GetTokens();
    }

    private Lexer(Stream stream, IErrorContext errorContext) {
        if (!stream.CanRead) {
            throw new ArgumentException("Stream must be readable", nameof(stream));
        }

        if (!stream.CanSeek) {
            throw new ArgumentException("Stream must be seekable", nameof(stream));
        }

        this._stream = stream;
        this._reader = new LookbackStreamReader(stream, leaveOpen: true);
        this._errorContext = errorContext;
    }

    private IEnumerable<Token> GetTokens() {
        while (!this.IsAtEnd()) {
            var token = this.ScanToken();
            if (token is not null) {
                yield return token;
            }
        }

        yield return new Token(TokenType.Eof, string.Empty, null, this._lineNumber);
    }

    private Token? ScanToken() {
        if (this.IsAtEnd()) {
            return null;
        }

        this._lexemeBuilder.Clear();

        var ch = this.Advance();
        switch (ch) {
            case '(':
                return this.MakeToken(TokenType.LeftParen);
            case ')':
                return this.MakeToken(TokenType.RightParen);
            case '{':
                return this.MakeToken(TokenType.LeftBrace);
            case '}':
                return this.MakeToken(TokenType.RightBrace);
            case ',':
                return this.MakeToken(TokenType.Comma);
            case '.':
                return this.MakeToken(TokenType.Dot);
            case '-':
                return this.MakeToken(TokenType.Minus);
            case '+':
                return this.MakeToken(TokenType.Plus);
            case ';':
                return this.MakeToken(TokenType.Semicolon);
            case '*':
                return this.MakeToken(TokenType.Star);
            case '!':
                return this.Match('=')
                    ? this.MakeToken(TokenType.BangEqual)
                    : this.MakeToken(TokenType.Bang);
            case '=':
                return this.Match('=')
                    ? this.MakeToken(TokenType.EqualEqual)
                    : this.MakeToken(TokenType.Equal);
            case '<':
                return this.Match('=')
                    ? this.MakeToken(TokenType.LessEqual)
                    : this.MakeToken(TokenType.Less);
            case '>':
                return this.Match('=')
                    ? this.MakeToken(TokenType.GreaterEqual)
                    : this.MakeToken(TokenType.Greater);
            case '/':
                if (this.Match('/')) {
                    this._builderEnabled = false;
                    this._lexemeBuilder.Clear();

                    while (!this.IsAtEnd() && this.Peek() != '\n') {
                        this.Advance();
                    }

                    this._builderEnabled = true;
                }
                else {
                    return this.MakeToken(TokenType.Slash);
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
                return this.String();
            default:
                if (char.IsDigit(ch)) {
                    return this.Number();
                }
                else if (char.IsLetter(ch)) {
                    return this.Identifier();
                }

                this._errorContext.Error($"Unexpected character '{ch}'", this._lineNumber);
                this._lexemeBuilder.Clear();
                break;
        }

        return null;
    }

    private Token? String() {
        while (this.Peek() != '"' && !this.IsAtEnd()) {
            if (this.Peek() == '\n') {
                this._lineNumber++;
            }

            this.Advance();
        }

        if (this.IsAtEnd()) {
            this._errorContext.Error("Unterminated string.", this._lineNumber);
            return null;
        }

        this.Advance(); // The closing `"`
        var str = this._lexemeBuilder.ToString().TrimExact('"', 1);
        return this.MakeToken(TokenType.String, str);
    }

    private Token Number() {
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

        var value = double.Parse(this._lexemeBuilder.ToString());
        return this.MakeToken(TokenType.Number, value);
    }

    private Token Identifier() {
        while (char.IsLetterOrDigit(this.Peek())) {
            this.Advance();
        }

        var text = this._lexemeBuilder.ToString();
        if (Keywords.TryGetTokenType(text, out var tokenType)) {
            return this.MakeToken(tokenType);
        }

        return this.MakeToken(TokenType.Identifier);
    }

    private char Advance() {
        var ch = (char)this._reader.Read();

        if (this._builderEnabled) {
            this._lexemeBuilder.Append(ch);
        }

        return ch;
    }

    private char Peek() {
        var current = this._reader.Peek();
        return current == -1 ? '\0' : (char)current;
    }

    private char PeekNext() {
        var next = this._reader.PeekNext();
        return next == -1 ? '\0' : (char)next;
    }

    private bool Match(char expected) {
        if (this.Peek() != expected) {
            return false;
        }

        this.Advance();
        return true;
    }

    private string GetLexeme() {
        var lexeme = this._lexemeBuilder.ToString();
        this._lexemeBuilder.Clear();
        return lexeme;
    }

    private Token MakeToken(TokenType type, object? value = null) {
        return new Token(type, this.GetLexeme(), value, this._lineNumber);
    }

    private bool IsAtEnd() {
        return this._reader.EndOfStream;
    }

    public void Dispose() {
        this._stream.Dispose();
        this._reader.Dispose();
        this._errorContext.Dispose();
    }

    public async ValueTask DisposeAsync() {
        await this._stream.DisposeAsync();
    }
}
