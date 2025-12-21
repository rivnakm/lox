namespace Lox;

public sealed class Interpreter : IDisposable {
    private readonly TextWriter _stdOutWriter;
    private readonly TextWriter _stdErrWriter;
    private readonly ErrorContext _errorContext;

    public Interpreter(Stream stdOut, Stream stdErr) {
        this._stdOutWriter = new StreamWriter(stdOut);
        this._stdErrWriter = new StreamWriter(stdErr);
        this._errorContext = new ErrorContext(this._stdErrWriter);
    }

    public void Dispose() {
        this._stdOutWriter.Dispose();
        this._stdErrWriter.Dispose();
        this._errorContext.Dispose();
    }

    public void Exec(Stream stream, bool exitOnError = true) {
        var tokens = Lexer.GetTokens(stream, this._errorContext);
        foreach (var token in tokens) {
            this._stdOutWriter.WriteLine(token);
        }

        this._stdOutWriter.Flush();

        if (exitOnError && this._errorContext.HasError) {
            Environment.Exit((int)StatusCode.Failure);
        }
        else {
            this._errorContext.Reset();
        }
    }
}
