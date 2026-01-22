using Lox.Utility;

namespace Lox.Extensions;

public static class LoxValueExtensions {
    // I was opposed to this extension method because it can be used on literally anything,
    // but it is very convenient...
    public static bool IsTruthy(this object? value) {
        return InterpreterValueUtility.IsTruthy(value);
    }
}
