using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proto
{
    public class CommandHelper
    {
        public static CommandHelper Instance { get; private set; } = new CommandHelper();
        private CommandHelper() { }
        CommandLineApplication commandLineApplication;
        private CommandOption ns;
        public string Namespace => ns.Value();
        private CommandOption protoPath;
        public List<string> ProtoPath => protoPath.Values;
        private CommandOption outputPath;
        public string OutputPath => outputPath.Value();
        private CommandOption server;
        public bool GenServer => server.HasValue();
        private CommandOption client;
        public bool GenClient => client.HasValue();
        private CommandOption innerFiles;
        public List<string> InnerFiles => innerFiles.Values;
        private CommandOption outerFiles;
        public List<string> OuterFiles => outerFiles.Values;
        private CommandOption innerClass;
        public string InnerClass => innerClass.Value();
        private CommandOption outerClass;
        public string OuterClass => outerClass.Value();

        private CommandOption outputOuterFile;
        private CommandOption outputInnerFile;
        public string OuputInnerFile => outputInnerFile.Value();
        public string OutputOuterFile => outputOuterFile.Value();

        private CommandOption nsOpcode;
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
