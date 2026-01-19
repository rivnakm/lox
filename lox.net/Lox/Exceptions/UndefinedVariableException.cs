namespace Lox.Exceptions;

public class UndefinedVariableException : RuntimeException {
    public UndefinedVariableException(string message) : base(message) {
    }
}
