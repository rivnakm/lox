namespace Lox.Extensions;

public static class StringExtensions {
    public static string TrimExact(this string str, char trimChar, int count) {
        ArgumentNullException.ThrowIfNull(str);

        if (str.Length == 0) {
            return string.Empty;
        }

        if (count <= 0) {
            throw new ArgumentOutOfRangeException(nameof(count), "count must be greater than 0");
        }

        var chars = str.ToCharArray();
        var start = 0;
        var end = chars.Length - 1;

        for (var i = 0; i < count; i++) {
            if (chars[start] == trimChar) {
                start++;
            }

            if (chars[end] == trimChar) {
                end--;
            }
        }

        return new string(chars[start..(end+1)]);
    }
}
