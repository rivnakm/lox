namespace Lox;

public record Token(TokenType Type, string Lexeme, object? Value, uint Line) {
    public override string ToString() {
        return $"[{this.Type};{this.Lexeme};{this.Value}]";
    }
}
