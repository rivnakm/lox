namespace Lox.Exceptions;

public class InvalidTypeException : RuntimeException
{
    public Token? Token { get; }

    public InvalidTypeException(string message) : base(message) { }

    public InvalidTypeException(Token token, string message) : base(message)
    {
        this.Token = token;
    }
}
