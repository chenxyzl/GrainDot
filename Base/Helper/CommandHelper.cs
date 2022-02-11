using McMaster.Extensions.CommandLineUtils;

namespace Base.Helper;

public class CommandHelper
{
    private CommandOption client;

    private CommandLineApplication commandLineApplication;
    private CommandOption idOption;
    private CommandOption typeOption;

    private CommandHelper()
    {
    }

    public static CommandHelper Instance { get; } = new();
    public string NodeId => idOption.Value();
    public string NodeType => typeOption.Value();

    public bool Parse(string[] args)
    {
        commandLineApplication = new CommandLineApplication(false);
        idOption = commandLineApplication.Option(
            "-i | --id",
            "node id",
            CommandOptionType.SingleValue);

        typeOption = commandLineApplication.Option(
            "-t | --type",
            "node type",
            CommandOptionType.MultipleValue);

        commandLineApplication.HelpOption("-? | -h | --help");

        commandLineApplication.Execute(args);

        if (args.Length == 1 && (args[0] == "-?" || args[0] == "-h" || args[0] == "--help"))
            return true;
        return false;
    }
}