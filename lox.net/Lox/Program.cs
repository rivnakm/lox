using System.IO.Abstractions;
using System.Text;

namespace Lox;

internal class Program {
    private static int Main(string[] args) {
        var appArgs = Arguments.Parse(args);
        var file = appArgs.File;
        var filesystem = new FileSystem();
        if (file is not null) {
            var fileInfo = new FileInfoWrapper(filesystem, new FileInfo(file));
            if (!fileInfo.Exists) {
                throw new InvalidOperationException($"File '{fileInfo}' not found");
            }

            RunFile(fileInfo);
        }
        else {
            RunPrompt();
        }

        return (int)StatusCode.Success;
    }

    private static void RunFile(IFileInfo file) {
        var interpreter = new LoxExecutor();
        using var stream = file.OpenRead();

        interpreter.Exec(stream);
    }

    private static void RunPrompt() {
        var interpreter = new LoxExecutor();
        while (true) {
            Console.Write("lox > ");
            Console.Out.Flush();
            var line = Console.ReadLine();
            if (line is null) {
                break;
            }

            interpreter.Exec(new MemoryStream(Encoding.UTF8.GetBytes(line)), false);
        }
    }
}
