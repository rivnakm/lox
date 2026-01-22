namespace Lox.Exceptions;

public class InvalidSyntaxException : Exception {

    public InvalidSyntaxException(Token token, string message) : base(message) {
        this.Token = token;
    }

    public Token Token { get; }
}
