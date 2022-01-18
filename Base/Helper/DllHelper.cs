using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Base.Helper
{
    public static class DllHelper
    {
        public static Assembly[] GetHotfixAssembly()
        {
            Assembly[] assembly = {
                typeof(Game).Assembly,
                Assembly.Load(File.ReadAllBytes("./Share.Hotfix.dll"), File.ReadAllBytes("./Share.Hotfix.pdb")),
                Assembly.Load(File.ReadAllBytes("./OM.Hotfix.dll"), File.ReadAllBytes("./OM.Hotfix.pdb")),
                Assembly.Load(File.ReadAllBytes("./World.Hotfix.dll"), File.ReadAllBytes("./World.Hotfix.pdb")),
                Assembly.Load(File.ReadAllBytes("./Login.Hotfix.dll"), File.ReadAllBytes("./Login.Hotfix.pdb")),
                Assembly.Load(File.ReadAllBytes("./Home.Hotfix.dll"), File.ReadAllBytes("./Home.Hotfix.pdb")),
            };

            return assembly;
        }
    }
}
