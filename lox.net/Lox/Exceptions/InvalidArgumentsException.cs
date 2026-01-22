namespace Lox.Exceptions;

public class InvalidArgumentsException(string message) : RuntimeException(message);
