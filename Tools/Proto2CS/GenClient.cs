﻿using System;
using System.IO;
using System.Text;

namespace Proto
{
    public static class GenClient
    {
        private static readonly StringBuilder _cs = new();
        private static StringBuilder _sc = new();

        internal static void Proto2CS()
        {
            _sc = new StringBuilder();
            foreach (var filePath in CommandHelper.Instance.OuterFiles)
                Parse(CommandHelper.Instance.Namespace, filePath);

            Generate(CommandHelper.Instance.Namespace, CommandHelper.Instance.OutputOuterFile,
                CommandHelper.Instance.OutputPath);
        }

        private static void Parse(string ns, string filePath)
        {
            Console.WriteLine($"read {filePath}");
            var lines = File.ReadAllLines(filePath);
            for (var i = 1; i < lines.Length; ++i)
            {
                var line = lines[i].Trim();
                var texts = line.Split(",");
                var opcode = int.Parse(texts[0].Trim());
                if (!string.IsNullOrEmpty(texts[2].Trim()))
                    _cs.AppendLine($"{{{opcode}, typeof({ns}.{texts[2].Trim()})}},");
                if (!string.IsNullOrEmpty(texts[3].Trim()))
                    _sc.AppendLine($"{{{opcode + 1}, typeof({ns}.{texts[3].Trim()})}},");
            }
        }

        private static void Generate(string ns, string fileName, string outputPath)
        {
            var csPath = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(fileName) + ".cs");

            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine($"namespace {ns} \n{{");
            sb.AppendLine("public static class Protocols{");
            sb.AppendLine(
                "public static Dictionary<uint, System.Type> CS_PROTOCOLS = new Dictionary<uint, System.Type>(){");
            sb.AppendLine(_cs.ToString());
            sb.AppendLine("};");

            sb.AppendLine(
                "public static Dictionary<uint, System.Type> SC_PROTOCOLS = new Dictionary<uint, System.Type>(){");
            sb.AppendLine(_sc.ToString());
            sb.AppendLine("};}}");

            File.WriteAllText(csPath, sb.ToString());
        }
    }
}