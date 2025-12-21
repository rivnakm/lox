using Lox.Collections;

namespace Lox;

public sealed class LookbackStreamReader : IDisposable {
    private readonly StreamReader _reader;
    private readonly Deque<int> _lookback = new();

    public LookbackStreamReader(Stream stream, bool leaveOpen = false) {
        this._reader = new StreamReader(stream, leaveOpen);
    }

    public bool EndOfStream => this._reader.EndOfStream && this._lookback.Count == 0;

    public int Peek() {
        if (this.EndOfStream) {
            return -1;
        }

        return this._lookback.Count == 0 ? this._reader.Peek() : this._lookback.Peek();
    }

    public int PeekNext() {
        if (this.EndOfStream) {
            return -1;
        }

        int current;
        if (this._lookback.Count == 0) {
            current = this._reader.Read();
            this._lookback.Push(current);
            return this._reader.Peek();
        }

        if (this._lookback.Count == 1) {
            return this._reader.Peek();
        }

        current = this._lookback.Pop();
        var next = this._lookback.Peek();
        this._lookback.Unpop(current);
        return next;
    }

    public int Read() {
        if (this.EndOfStream) {
            return -1;
        }

        if (this._lookback.Count > 0) {
            return this._lookback.Pop();
        }

        return this._reader.Read();
    }

    public void Dispose() {
        this._reader.Dispose();
    }
}
