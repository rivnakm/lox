using System.Collections.Frozen;

namespace Lox;

public static class Keywords {
    private static readonly FrozenDictionary<string, TokenType> Map = new Dictionary<string, TokenType> {
        ["and"] = TokenType.And,
        ["class"] = TokenType.Class,
        ["else"] = TokenType.Else,
        ["false"] = TokenType.False,
        ["for"] = TokenType.For,
        ["fun"] = TokenType.Fun,
        ["if"] = TokenType.If,
        ["nil"] = TokenType.Nil,
        ["or"] = TokenType.Or,
        ["print"] = TokenType.Print,
        ["return"] = TokenType.Return,
        ["super"] = TokenType.Super,
        ["this"] = TokenType.This,
        ["true"] = TokenType.True,
        ["var"] = TokenType.Var,
        ["while"] = TokenType.While
    }.ToFrozenDictionary();

    public static bool TryGetTokenType(string str, out TokenType type) {
        return Map.TryGetValue(str, out type);
    }
}
