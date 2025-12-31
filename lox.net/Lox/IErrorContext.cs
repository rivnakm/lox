namespace Lox;

public interface IErrorContext {
    bool HasError { get; }
    void Error(string message, int line);
    void Error(Token token, string message);
    void Reset();
}
