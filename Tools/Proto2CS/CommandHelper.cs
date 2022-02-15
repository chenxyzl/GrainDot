using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;

namespace Proto
{
    public class CommandHelper
    {
        private readonly CommandOption _client;

        private readonly CommandLineApplication _commandLineApplication;
        private readonly CommandOption _innerClass;
        private readonly CommandOption _innerFiles;
        private readonly CommandOption _ns;

        private readonly CommandOption _nsOpcode;
        private readonly CommandOption _outerClass;
        private readonly CommandOption _outerFiles;

        private readonly CommandOption _outputOuterFile;
        private readonly CommandOption _outputPath;
        private readonly CommandOption _protoPath;
        private readonly CommandOption _server;

        private CommandHelper()
        {
            _commandLineApplication = new CommandLineApplication(false);
            _ns = _commandLineApplication.Option(
                "-n | --namespace",
                "Namespace of protocols",
                CommandOptionType.SingleValue);

            _protoPath = _commandLineApplication.Option(
                "-p | --protocol",
                "Path of .proto files",
                CommandOptionType.MultipleValue);

            _outputPath = _commandLineApplication.Option(
                "-o | --output",
                "Destination directory path of the protocol .cs files",
                CommandOptionType.SingleValue);

            _server = _commandLineApplication.Option(
                "-s | --server",
                "Gen server rpc",
                CommandOptionType.NoValue);

            _client = _commandLineApplication.Option(
                "-c | --client",
                "Gen Client OpCode operations",
                CommandOptionType.NoValue);

            _innerFiles = _commandLineApplication.Option(
                "--innerfile",
                "input file of opcode and inner rpc definitions.",
                CommandOptionType.MultipleValue);

            _outerFiles = _commandLineApplication.Option(
                "--outerfile",
                "input file of opcode and outer rpc definitions.",
                CommandOptionType.MultipleValue);

            _innerClass = _commandLineApplication.Option(
                "--innerclass",
                "input class name of inner definitions.",
                CommandOptionType.SingleValue);

            _outerClass = _commandLineApplication.Option(
                "--outerclass",
                "input class name of inner definitions.",
                CommandOptionType.SingleValue);

            _outputOuterFile = _commandLineApplication.Option(
                "--outputouter",
                "filename of outer cs file.",
                CommandOptionType.SingleValue);

            _nsOpcode = _commandLineApplication.Option(
                "--nsopcode",
                "namespaceo of opcode class",
                CommandOptionType.SingleValue);

            _commandLineApplication.HelpOption("-? | -h | --help");
        }

        public static CommandHelper Instance { get; } = new();
        public string Namespace => _ns.Value();
        public List<string> ProtoPath => _protoPath.Values;
        public string OutputPath => _outputPath.Value();
        public bool GenServer => _server.HasValue();
        public bool GenClient => _client.HasValue();
        public List<string> InnerFiles => _innerFiles.Values;
        public List<string> OuterFiles => _outerFiles.Values;
        public string InnerClass => _innerClass.Value();
        public string OuterClass => _outerClass.Value();
        public string OutputOuterFile => _outputOuterFile.Value();
        public string NamespaceOpcode => _nsOpcode.Value();

        public bool Parse(string[] args)
        {
            _commandLineApplication.Execute(args);

            if (args.Length == 1 && (args[0] == "-?" || args[0] == "-h" || args[0] == "--help"))
                return true;
            return false;
        }
    }
}