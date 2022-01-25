using System;
using System.IO;
using System.Text;

namespace Proto
{
    public static class InnerProto2CSn
    {
        public static void Proto2CS()
        {
            foreach (var innerFile in CommandHelper.Instance.InnerFiles)
                GenerateOpcode(CommandHelper.Instance.NamespaceOpcode, innerFile, CommandHelper.Instance.OutputPath,
                    CommandHelper.Instance.InnerClass);

            foreach (var outerFile in CommandHelper.Instance.OuterFiles)
                GenerateOpcode(CommandHelper.Instance.NamespaceOpcode, outerFile, CommandHelper.Instance.OutputPath,
                    CommandHelper.Instance.OuterClass);
        }

        public static void GenerateOpcode(string ns, string protoName, string outputPath, string opcodeClassName)
        {
            var csPath = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(protoName) + ".cs");

            var s = File.ReadAllText(protoName);

            var sb = new StringBuilder();
            sb.Append("using System.Collections.Generic;\n");
            sb.Append("\n");
            sb.Append($"namespace {ns}\n");
            sb.Append("{\n");
            sb.Append("\tpublic partial class RpcItemMessage\n");
            sb.Append("\t{\n");
            sb.Append($"\t\tstatic public List<RpcItem> rpcItems{opcodeClassName} = new List<RpcItem>\n");
            sb.Append("\t\t{\n");
            var i = -1;
            foreach (var line in s.Split('\n'))
            {
                i++;
                var newline = line.Trim();

                if (newline == "" || i == 0) continue;

                var infoArr = newline.Split(',');
                if (infoArr.Length != 6) throw new Exception($"{newline} 格式不对,应该是逗号分隔,且有6个段");

                if (newline.StartsWith("//")) sb.Append($"{newline}\n");

                var param = infoArr[2] == "" ? "null" : $"typeof({CommandHelper.Instance.Namespace}.{infoArr[2]})";
                var ret = infoArr[3] == "" ? "null" : $"typeof({CommandHelper.Instance.Namespace}.{infoArr[3]})";
                sb.Append($"\t\t\tnew RpcItem({infoArr[0]}, OpType.{infoArr[1]}, {param}, {ret}, \"{infoArr[4]}\"),\n");
            }

            sb.Append("\t\t};\n");
            sb.Append("\t}\n");
            sb.Append("}\n");

            File.WriteAllText(csPath, sb.ToString());
        }
    }
}