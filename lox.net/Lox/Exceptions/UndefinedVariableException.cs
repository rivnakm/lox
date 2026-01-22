namespace Lox.Exceptions;

public class UndefinedVariableException(string message) : RuntimeException(message);
