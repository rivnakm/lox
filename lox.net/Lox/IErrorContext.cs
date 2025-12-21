namespace Lox;

public interface IErrorContext : IDisposable {
    bool HasError { get; }
    void Error(string message, uint line);
    void Reset();
}
