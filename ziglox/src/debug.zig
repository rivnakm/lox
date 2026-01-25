const std = @import("std");
const chunk = @import("chunk.zig");

const DisassembleInstructionError = error{
    UnknownOpCode,
};

pub fn disassembleChunk(ch: chunk.Chunk, name: []const u8) DisassembleInstructionError!void {
    std.debug.print("== {s} ==\n", .{name});

    var offset: usize = 0;
    while (offset < ch.count) {
        offset = try disassembleInstruction(ch, offset);
    }
}

pub fn disassembleInstruction(ch: chunk.Chunk, offset: usize) DisassembleInstructionError!usize {
    std.debug.print("{X:04} ", .{offset});

    const instruction: chunk.OpCode = @enumFromInt(ch.code.items[offset]);
    return switch (instruction) {
        chunk.OpCode.@"return" => simpleInstruction("OP_RETURN", offset),
    };
}

fn simpleInstruction(name: []const u8, offset: usize) usize {
    std.debug.print("{s}\n", .{name});
    return offset + 1;
}
