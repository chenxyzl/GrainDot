using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Base.Helper
{
    public static class DllHelper
    {
        public static Assembly[] GetHotfixAssembly(RoleType role)
        {
            Assembly[] assembly = {
                typeof(GameServer).Assembly,
                Assembly.Load(File.ReadAllBytes("./Share.Hotfix.dll"), File.ReadAllBytes("./Share.Hotfix.pdb")),
                Assembly.Load(File.ReadAllBytes($"./{role}.Hotfix.dll"), File.ReadAllBytes($"./{role}.Hotfix.pdb")),
            };

            return assembly;
        }
    }
}
