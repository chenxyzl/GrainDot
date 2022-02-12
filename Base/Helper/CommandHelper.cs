using McMaster.Extensions.CommandLineUtils;

namespace Base.Helper;

public class CommandHelper
{
    private CommandLineApplication _commandLineApplication;
    private CommandOption _idOption;
    private CommandOption _typeOption;

    private CommandHelper()
    {
    }

    public static CommandHelper Instance { get; } = new();
    public string NodeId => _idOption.Value();
    public string NodeType => _typeOption.Value();

    public bool Parse(string[] args)
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

        _commandLineApplication.Execute(args);

        if (args.Length == 1 && (args[0] == "-?" || args[0] == "-h" || args[0] == "--help"))
            return true;
        return false;
    }
}