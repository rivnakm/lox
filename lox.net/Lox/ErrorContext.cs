namespace Lox;

public sealed class ErrorContext : IErrorContext {
    private readonly TextWriter _stdErrWriter;

    public bool HasError { get; private set; }

    public ErrorContext(TextWriter stdErrWriter) {
        this._stdErrWriter = stdErrWriter;
    }

    public void Error(string message, uint line) {
        this.Report(line, "", message);
    }

    private void Report(uint line, string where, string message) {
        this._stdErrWriter.WriteLine($"[{line}]: Error{where}: {message}");
        this.HasError = true;
    }

    public void Reset() {
        this._stdErrWriter.Flush();
        this.HasError = false;
    }

    public void Dispose() {
        this._stdErrWriter.Dispose();
    }
}
