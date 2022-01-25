using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;

namespace Proto
{
    public class CommandHelper
    {
        private CommandOption client;

        private CommandLineApplication commandLineApplication;
        private CommandOption innerClass;
        private CommandOption innerFiles;
        private CommandOption ns;

        private CommandOption nsOpcode;
        private CommandOption outerClass;
        private CommandOption outerFiles;
        private CommandOption outputInnerFile;

        private CommandOption outputOuterFile;
        private CommandOption outputPath;
        private CommandOption protoPath;
        private CommandOption server;

        private CommandHelper()
        {
        }

        public static CommandHelper Instance { get; } = new();
        public string Namespace => ns.Value();
        public List<string> ProtoPath => protoPath.Values;
        public string OutputPath => outputPath.Value();
        public bool GenServer => server.HasValue();
        public bool GenClient => client.HasValue();
        public List<string> InnerFiles => innerFiles.Values;
        public List<string> OuterFiles => outerFiles.Values;
        public string InnerClass => innerClass.Value();
        public string OuterClass => outerClass.Value();
        public string OuputInnerFile => outputInnerFile.Value();
        public string OutputOuterFile => outputOuterFile.Value();
        public string NamespaceOpcode => nsOpcode.Value();

        public bool Parse(string[] args)
        {
            commandLineApplication = new CommandLineApplication(false);
            ns = commandLineApplication.Option(
                "-n | --namespace",
                "Namespace of protocols",
                CommandOptionType.SingleValue);

            protoPath = commandLineApplication.Option(
                "-p | --protocol",
                "Path of .proto files",
                CommandOptionType.MultipleValue);

            outputPath = commandLineApplication.Option(
                "-o | --output",
                "Destination directory path of the protocol .cs files",
                CommandOptionType.SingleValue);

            server = commandLineApplication.Option(
                "-s | --server",
                "Gen server rpc",
                CommandOptionType.NoValue);

            client = commandLineApplication.Option(
                "-c | --client",
                "Gen Client OpCode operations",
                CommandOptionType.NoValue);

            innerFiles = commandLineApplication.Option(
                "--innerfile",
                "input file of opcode and inner rpc definitions.",
                CommandOptionType.MultipleValue);

            outerFiles = commandLineApplication.Option(
                "--outerfile",
                "input file of opcode and outer rpc definitions.",
                CommandOptionType.MultipleValue);

            innerClass = commandLineApplication.Option(
                "--innerclass",
                "input class name of inner definitions.",
                CommandOptionType.SingleValue);

            outerClass = commandLineApplication.Option(
                "--outerclass",
                "input class name of inner definitions.",
                CommandOptionType.SingleValue);

            outputInnerFile = commandLineApplication.Option(
                "--outputinner",
                "filename of inner cs file.",
                CommandOptionType.SingleValue);

            outputOuterFile = commandLineApplication.Option(
                "--outputouter",
                "filename of outer cs file.",
                CommandOptionType.SingleValue);

            nsOpcode = commandLineApplication.Option(
                "--nsopcode",
                "namespaceo of opcode class",
                CommandOptionType.SingleValue);

            commandLineApplication.HelpOption("-? | -h | --help");

            commandLineApplication.Execute(args);

            if (args.Length == 1 && (args[0] == "-?" || args[0] == "-h" || args[0] == "--help"))
                return true;
            return false;
        }
    }
}