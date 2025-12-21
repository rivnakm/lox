using System.CommandLine;
using System.IO.Abstractions;
using System.Text;

namespace Lox;

class Program {
    static void Main(string[] args) {
        var rootCommand = new RootCommand("Lox interpreter");

        var fileArgument = new Argument<FileInfo>("FILE");
        rootCommand.Arguments.Add(fileArgument);

        var parseResult = rootCommand.Parse(args);

        var file = parseResult.GetValue(fileArgument);
        var filesystem = new FileSystem();
        if (file is not null) {
            var fileInfo = new FileInfoWrapper(filesystem, file);
            if (!fileInfo.Exists) {
                throw new InvalidOperationException($"File '{file.FullName}' not found.");
            }

            RunFile(fileInfo);
        }
        else {
            RunPrompt();
        }
    }

    private static void RunFile(IFileInfo file) {
        var interpreter = new Interpreter(Console.OpenStandardOutput(), Console.OpenStandardError());
        using var stream = file.OpenRead();

        interpreter.Exec(stream);
    }

    private static void RunPrompt() {
        var stdout = Console.OpenStandardOutput();
        var interpreter = new Interpreter(stdout, Console.OpenStandardError());
        using var stdin = Console.OpenStandardInput();

        using var stdoutWriter = new StreamWriter(stdout, leaveOpen: true);
        using var stdinReader = new StreamReader(stdin, leaveOpen: true);
        while (true) {
            stdoutWriter.Write("> ");
            var line = stdinReader.ReadLine();
            if (line is null) {
                break;
            }

            interpreter.Exec(new MemoryStream(Encoding.UTF8.GetBytes(line)), false);
        }
    }
}
