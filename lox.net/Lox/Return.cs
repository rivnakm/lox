namespace Lox;

public class Return : Exception {
    public object? Value { get; }

    public Return(object? value) {
        this.Value = value;
    }
}
