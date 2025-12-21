using System.IO.Abstractions;
using System.Text;

namespace Lox;

class Program {
    static int Main(string[] args) {
        var appArgs = Arguments.Parse(args);
        var file = appArgs.File;
        var filesystem = new FileSystem();
        if (file is not null) {
            var fileInfo = new FileInfoWrapper(filesystem, new FileInfo(file));
            if (!fileInfo.Exists) {
                throw new InvalidOperationException($"File '{fileInfo}' not found.");
            }

            RunFile(fileInfo);
        }
        else {
            RunPrompt();
        }

        return (int)StatusCode.Success;
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
            stdoutWriter.Write("lox > ");
            stdoutWriter.Flush();
            var line = stdinReader.ReadLine();
            if (line is null) {
                break;
            }

            interpreter.Exec(new MemoryStream(Encoding.UTF8.GetBytes(line)), false);
        }
    }
}
