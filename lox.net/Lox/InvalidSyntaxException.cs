namespace Lox;

public class InvalidSyntaxException : Exception
{
    public Token Token { get; }

    public InvalidSyntaxException(Token token, string message) : base(message)
    {
        this.Token = token;
    }
}
