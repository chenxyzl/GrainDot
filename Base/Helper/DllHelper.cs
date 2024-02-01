using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Base.Helper;

public static class DllHelper
{
    private static AssemblyLoadContext? _hotfixContext;

    public static Assembly[] GetHotfixAssembly(GameServer game)
    {
        if (_hotfixContext != null)
        {
            _hotfixContext.Unload();
            _hotfixContext = null;
            //GC.Collect();
        }

        _hotfixContext = new AssemblyLoadContext("HotfixContext", isCollectible: true);
        Assembly[] assembly =
        {
            typeof(GameServer).Assembly,
            game.GetType().Assembly,
            _hotfixContext.LoadFromStream(new MemoryStream(File.ReadAllBytes(Path.GetFullPath($"./{game.Role}.Hotfix.dll"))),new MemoryStream(File.ReadAllBytes(Path.GetFullPath($"./{game.Role}.Hotfix.pdb"))))
        };

        return assembly;
    }
}