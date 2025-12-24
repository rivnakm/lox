namespace Lox;

public record Token(TokenType Type, string Lexeme, object? Value, int Line)
{
    public override string ToString()
    {
        return $"[{this.Type};{this.Lexeme};{this.Value}]";
    }
}
