const std = @import("std");
const chunk = @import("chunk.zig");
const debug = @import("debug.zig");
const OpCode = chunk.OpCode;

pub fn main() !void {
    var debug_allocator = std.heap.DebugAllocator(.{}).init;
    defer _ = debug_allocator.deinit();
    const gpa = debug_allocator.allocator();

    var ch = chunk.Chunk.init();
    defer ch.deinit(gpa);
    try ch.write(OpCode.@"return", gpa);
    try debug.disassembleChunk(ch, "test chunk");
}
