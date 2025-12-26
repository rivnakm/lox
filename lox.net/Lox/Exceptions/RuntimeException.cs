namespace Lox.Exceptions;

public abstract class RuntimeException : Exception
{
    protected RuntimeException(string message) : base(message) { }
}
