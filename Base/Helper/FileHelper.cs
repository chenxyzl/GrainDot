using System.Collections.Generic;
using System.IO;

namespace Base.Helper;

public static class FileHelper
{
    public static void GetAllFiles(List<string> files, string dir)
    {
        var fls = Directory.GetFiles(dir);
        foreach (var fl in fls) files.Add(fl);

        var subDirs = Directory.GetDirectories(dir);
        foreach (var subDir in subDirs) GetAllFiles(files, subDir);
    }

    public static void CleanDirectory(string dir)
    {
        foreach (var subdir in Directory.GetDirectories(dir)) Directory.Delete(subdir, true);

        foreach (var subFile in Directory.GetFiles(dir)) File.Delete(subFile);
    }

    public static void CopyDirectory(string srcDir, string tgtDir)
    {
        var source = new DirectoryInfo(srcDir);
        var target = new DirectoryInfo(tgtDir);

        if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
            throw new Exception("父目录不能拷贝到子目录！");

        if (!source.Exists) return;

        if (!target.Exists) target.Create();

        var files = source.GetFiles();

        for (var i = 0; i < files.Length; i++)
            File.Copy(files[i].FullName, Path.Combine(target.FullName, files[i].Name), true);

        var dirs = source.GetDirectories();

        for (var j = 0; j < dirs.Length; j++)
            CopyDirectory(dirs[j].FullName, Path.Combine(target.FullName, dirs[j].Name));
    }
}