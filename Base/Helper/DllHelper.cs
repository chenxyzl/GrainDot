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
                //Assembly.Load(File.ReadAllBytes("./Hotfix.dll"), File.ReadAllBytes("./Hotfix.pdb")),
            //Assembly.Load(File.ReadAllBytes("./Hotfix.dll"), File.ReadAllBytes("./Hotfix.pdb")),
            Assembly.Load(File.ReadAllBytes("./Home.Hotfix.dll"), File.ReadAllBytes("./Home.Hotfix.pdb"))
            };

            return assembly;
        }
    }
}
