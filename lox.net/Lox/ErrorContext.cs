namespace Lox;

public sealed class ErrorContext : IErrorContext
{
    public bool HasError { get; private set; }

    public void Error(string message, int line)
    {
        this.Report(line, "", message);
    }

    public void Error(Token token, string message)
    {
        if (token.Type == TokenType.Eof)
        {
            this.Report(token.Line, " at end", message);
        }
        else
        {
            this.Report(token.Line, $" at '{token.Lexeme}'", message);
        }
    }

    private void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[{line}]: Error{where}: {message}");
        this.HasError = true;
    }

    public void Reset()
    {
        Console.Error.Flush();
        this.HasError = false;
    }
}
