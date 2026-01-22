using Lox.StdLib;

namespace Lox;

public static class LoxStdLib {
    public static readonly ILoxCallable Clock =
        new ExternCallable(0, (_, _) => (double)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
}
