using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace Proto
{
    public static class InnerProto2CS
    {
        //private const string protoPath = ".";
        //private const string serverMessagePath = "../Server/Model/Module/Message/";
        private static readonly char[] splitChars = {' ', '\t'};

        public static void Proto2CS()
        {
            Console.WriteLine($"output path {CommandHelper.Instance.OutputPath}");
            if (!Directory.Exists(CommandHelper.Instance.OutputPath))
            {
                var dirInfo = Directory.CreateDirectory(CommandHelper.Instance.OutputPath);
                Console.WriteLine($"create edir {dirInfo.FullName}");
            }

            foreach (var protoPath in CommandHelper.Instance.ProtoPath)
            {
                var filesPath = Directory.GetFiles(protoPath, "*.proto", SearchOption.AllDirectories);
                foreach (var protoFile in filesPath)
                    Proto2CS(CommandHelper.Instance.Namespace, protoFile, CommandHelper.Instance.OutputPath);
            }
        }


        public static void PraseImport(StringBuilder sb, string us, string ns)
        {
            //序列化的头
            sb.Append("using ProtoBuf;\n");
            sb.Append("using System.Collections.Generic;\n");

            //包名
            sb.Append($"namespace {ns}\n");
            sb.Append("{\n");
        }

        public static void Proto2CS(string ns, string protoName, string outputPath)
        {
            //string proto = Path.Combine(protoPath, protoName);
            //Console.WriteLine($"proto {proto}");
            var csPath = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(protoName) + ".cs");

            Console.WriteLine($"read {protoName}");
            var s = File.ReadAllText(protoName);

            var sb = new StringBuilder();
            var linesArr = s.Split('\n');
            var lines = new ArrayList();

            var foundPackge = false;
            //过滤无效的行 并解析头
            foreach (var line in linesArr)
            {
                var newline = line.Trim();
                if (newline == "") continue;

                if (newline.StartsWith("syntax")) continue;

                if (newline.StartsWith("package"))
                {
                    foundPackge = true;
                    var idx = newline.IndexOf(";");
                    newline = newline.Remove(idx);
                    var ss = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                    PraseImport(sb, ss[1], ns);
                    continue;
                }

                if (newline.StartsWith("import")) continue;

                var addL = false;
                if (newline.Contains('{') && !newline.StartsWith("{"))
                {
                    addL = true;
                    var idx = newline.IndexOf("{");
                    if (newline[idx - 1] != ' ')
                        newline = newline.Substring(0, idx) + ' ' + newline.Substring(idx + 1);
                    else
                        newline = newline.Substring(0, idx) + newline.Substring(idx + 1);
                }

                var addR = false;
                if (newline.Contains('}') && !newline.StartsWith("}"))
                {
                    addR = true;
                    var idx = newline.IndexOf("}");
                    newline = newline.Substring(0, idx) + newline.Substring(idx + 1);
                }

                lines.Add(newline);
                if (addL) lines.Add("{");

                if (addR) lines.Add("}");
            }

            if (!foundPackge) throw new Exception($"{protoName}: not found package");


            var state = State.Nothings;
            var index = -1;
            while (++index < lines.Count)
            {
                var newline = lines[index]!.ToString()!;
                if (newline.StartsWith("//"))
                {
                    sb.Append($"{newline}\n");
                    continue;
                }

                switch (state)
                {
                    case State.Nothings:
                    {
                        if (newline.StartsWith("message"))
                        {
                            state = State.Messageing;
                            PraseMessageHeader(protoName, newline, sb);
                        }

                        if (newline.StartsWith("enum"))
                        {
                            state = State.Enuming;
                            PraseEnumHeader(protoName, newline, sb);
                        }

                        break;
                    }
                    case State.Messageing:
                    {
                        if (newline.StartsWith("message") || newline.StartsWith("enum"))
                            throw new Exception($"{protoName}: messsage and enum not suport nest");

                        if (PraseMessageBody(protoName, newline, sb)) state = State.Nothings;

                        break;
                    }
                    case State.Enuming:
                    {
                        if (newline.StartsWith("message") || newline.StartsWith("enum"))
                            throw new Exception($"{protoName}: messsage and enum not suport nest");

                        if (PraseEnumBody(protoName, newline, sb)) state = State.Nothings;

                        break;
                    }
                    default:
                    {
                        throw new Exception($"{protoName}: state:{state} error");
                    }
                }
            }

            sb.Append("}\n");

            File.WriteAllText(csPath, sb.ToString());
        }

        private static void Repeated(string protoName, StringBuilder sb, string newline)
        {
            try
            {
                var index = newline.IndexOf(";");
                newline = newline.Remove(index);
                var ss = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                var type = ss[1];
                type = ConvertType(type);
                var name = ToUpperFirst(ss[2]);
                var n = int.Parse(ss[4]);

                sb.Append($"\t\t[ProtoMember({n})]\n");
                sb.Append($"\t\tpublic List<{type}> {name} = new List<{type}>();\n\n");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{newline}\n {e}");
            }
        }

        private static void Map(string protoName, StringBuilder sb, string newline)
        {
            try
            {
                var index = newline.IndexOf(";");
                newline = newline.Remove(index);
                //先把类型截取出来
                var tb = newline.IndexOf("<");
                var te = newline.IndexOf(">");
                if (tb >= te || tb == -1 || te == -1) throw new Exception("map格式不对");

                var inType = newline.Substring(tb, te - tb + 1);
                inType = inType.TrimStart('<');
                inType = inType.TrimEnd('>');
                var typeArr = inType.Split(",", StringSplitOptions.RemoveEmptyEntries);
                if (typeArr.Length != 2) throw new Exception("map格式不对");

                var type1 = ConvertType(typeArr[0].Trim());
                var type2 = ConvertType(typeArr[1].Trim());

                newline = newline.Substring(0, tb) + " ? " + newline.Substring(te + 1);
                var ss = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                var name = ToUpperFirst(ss[2]);
                var n = int.Parse(ss[4]);

                sb.Append($"\t\t[ProtoMember({n})]\n");
                sb.Append($"\t\tpublic Dictionary<{type1},{type2}> {name} = new Dictionary<{type1},{type2}>();\n\n");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{newline}\n {e}");
            }
        }

        private static string ConvertType(string type)
        {
            var typeCs = "";
            switch (type)
            {
                case "int16":
                    typeCs = "short";
                    break;
                case "int32":
                    typeCs = "int";
                    break;
                case "bytes":
                    typeCs = "byte[]";
                    break;
                case "uint32":
                    typeCs = "uint";
                    break;
                case "long":
                    typeCs = "long";
                    break;
                case "int64":
                    typeCs = "long";
                    break;
                case "uint64":
                    typeCs = "ulong";
                    break;
                case "uint16":
                    typeCs = "ushort";
                    break;
                default:
                    typeCs = type;
                    break;
            }

            return typeCs;
        }

        private static void PraseMessageHeader(string protoName, string newline, StringBuilder sb)
        {
            var parentClass = "";
            var msgName = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries)[1];
            var ss = newline.Split(new[] {"//"}, StringSplitOptions.RemoveEmptyEntries);

            if (ss.Length == 2)
            {
                parentClass = ss[1].Trim();
                parentClass = parentClass == "" ? parentClass : parentClass.Split(" ")[0];
            }

            //sb.Append($"\t[Message({opcodeClassName}.{msgName})]\n");
            sb.Append("\t[ProtoContract]\n");
            sb.Append($"\tpublic partial class {msgName}");


            string[] msgTypes = {"IRequsetPlayer", "IRequsetWorld", "IHttpRequest", "IHttpResponse"};
            if (msgTypes.ToList().IndexOf(parentClass) != -1)
                sb.Append($": {parentClass}\n");
            else if (parentClass != "")
                sb.Append($": {parentClass}\n");
            else
                sb.Append(": IMessage\n");
        }

        private static bool PraseMessageBody(string protoName, string newline, StringBuilder sb)
        {
            if (newline == "{")
            {
                sb.Append("\t{\n");
                return false;
            }

            if (newline == "}")
            {
                sb.Append("\t}\n\n");
                return true;
            }

            if (newline.StartsWith("repeated"))
                Repeated(protoName, sb, newline);
            else if (newline.StartsWith("map"))
                Map(protoName, sb, newline);
            else
                MessageMembers(protoName, sb, newline, true);

            return false;
        }

        private static void MessageMembers(string protoName, StringBuilder sb, string newline, bool isRequired)
        {
            var index = newline.IndexOf(";");
            newline = newline.Remove(index);
            var ss = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                var type = ss[0];
                var name = ToUpperFirst(ss[1]);
                var n = int.Parse(ss[3]);
                var typeCs = ConvertType(type);

                sb.Append($"\t\t[ProtoMember({n})]\n");
                sb.Append($"\t\tpublic {typeCs} {name} {{ get; set; }}\n\n");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{newline}\n {e}");
            }
        }

        private static void PraseEnumHeader(string protoName, string newline, StringBuilder sb)
        {
            var msgName = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries)[1];
            var ss = newline.Split(new[] {"//"}, StringSplitOptions.RemoveEmptyEntries);
            //sb.Append($"\t[Message({opcodeClassName}.{msgName})]\n"); //用csv来生成rpc的id。各灵活可控一些
            sb.Append("\t[ProtoContract]\n");
            sb.Append($"\tpublic enum {msgName}\n");
        }

        private static bool PraseEnumBody(string protoName, string newline, StringBuilder sb)
        {
            if (newline == "{")
            {
                sb.Append("\t{\n");
                return false;
            }

            if (newline == "}")
            {
                sb.Append("\t}\n\n");
                return true;
            }

            EnumMembers(protoName, sb, newline, true);
            return false;
        }

        private static void EnumMembers(string protoName, StringBuilder sb, string newline, bool isRequired)
        {
            var index = newline.IndexOf(";");
            newline = newline.Remove(index);
            var ss = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                var name = ToUpperFirst(ss[0]);
                var n = int.Parse(ss[2]);
                //sb.Append($"\t\t[ProtoEnum]\n"); //v3版本不需要了
                sb.Append($"\t\t{name} = {n},\n\n");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{newline}\n {e}");
            }
        }

        private static unsafe string ToUpperFirst(this string str)
        {
            var ret = str;
            fixed (char* ptr = ret)
            {
                *ptr = char.ToUpper(*ptr);
            }

            return ret;
        }

        private enum State
        {
            Nothings = 0, //当前没事
            Messageing = 1, //当前未message状态
            Enuming = 2 //当前未枚举状态
        }
    }
}