using Clap.Net;

namespace Lox;

[Command(Name = "lox")]
public partial class Arguments {
    [Arg()]
    public string? File { get; init; }
}
