using McMaster.Extensions.CommandLineUtils;

namespace Base.Helper;

public class CommandHelper
{
    private readonly CommandLineApplication _commandLineApplication;
    private readonly CommandOption _idOption;
    private readonly CommandOption _typeOption;

    private CommandHelper()
    {
        _commandLineApplication = new CommandLineApplication(false);
        _idOption = _commandLineApplication.Option(
            "-i | --id",
            "node id",
            CommandOptionType.SingleValue);

        _typeOption = _commandLineApplication.Option(
            "-t | --type",
            "node type",
            CommandOptionType.MultipleValue);

        _commandLineApplication.HelpOption("-? | -h | --help");
    }

    public static CommandHelper Instance { get; } = new();
    public ushort NodeId => ushort.Parse(_idOption.Value() ?? "0");
    public string NodeType => _typeOption.Value() ?? "null";

    public bool Parse(string[] args)
    {
        _commandLineApplication.Execute(args);

        if (args.Length == 1 && (args[0] == "-?" || args[0] == "-h" || args[0] == "--help"))
            return true;
        return false;
    }
}