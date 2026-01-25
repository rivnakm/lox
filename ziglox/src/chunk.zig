const std = @import("std");

pub const OpCode = enum(u8) { @"return" };
pub const Chunk = struct {
    code: std.ArrayList(u8),
    count: usize,

    pub fn init() Chunk {
        return Chunk{ .code = .empty, .count = 0 };
    }

    pub fn deinit(self: *Chunk, gpa: std.mem.Allocator) void {
        self.code.deinit(gpa);
    }

    pub fn write(self: *Chunk, byte: OpCode, gpa: std.mem.Allocator) std.mem.Allocator.Error!void {
        try self.code.append(gpa, @intFromEnum(byte));
        self.count += 1;
    }
};
