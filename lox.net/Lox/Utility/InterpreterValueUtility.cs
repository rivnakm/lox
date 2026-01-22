using Lox.Exceptions;

namespace Lox.Utility;

public static class InterpreterValueUtility {
    public static bool IsTruthy(object? value) {
        return value is not null && value is not false;
    }

    public static bool AreEqual(object? left, object? right) {
        if (left is null && right is null) {
            return true;
        }

        if (left is null || right is null) {
            return false;
        }

        if (left is double leftDouble && right is double rightDouble) {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return leftDouble == rightDouble;
        }

        return left.Equals(right);
    }

    public static void CheckNumber(Token @operator, object? operand) {
        if (operand is double) {
            return;
        }

        throw new InvalidTypeException(@operator, "Operand must be a number");
    }

    public static void CheckNumbers(Token @operator, object? left, object? right) {
        if (left is double && right is double) {
            return;
        }

        throw new InvalidTypeException(@operator, "Operands must both be numbers");
    }

    public static string Format(object? value) {
        if (value is null) {
            return "nil";
        }

        if (value is bool boolValue) {
            return boolValue ? "true" : "false";
        }

        return value.ToString() ?? string.Empty;
    }
}
