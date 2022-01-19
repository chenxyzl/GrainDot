using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Linq;

namespace Proto
{
    public static class InnerProto2CS
    {
        //private const string protoPath = ".";
        //private const string serverMessagePath = "../Server/Model/Module/Message/";
        private static readonly char[] splitChars = { ' ', '\t' };

        public static void Proto2CS()
        {
            Console.WriteLine($"output path {CommandHelper.Instance.OutputPath}");
            if(!Directory.Exists(CommandHelper.Instance.OutputPath))
            {
                 var dirInfo = Directory.CreateDirectory(CommandHelper.Instance.OutputPath);
                Console.WriteLine($"create edir {dirInfo.FullName}");
            }

            foreach (var protoPath in CommandHelper.Instance.ProtoPath)
            {
                string[] filesPath = Directory.GetFiles(protoPath, "*.proto", SearchOption.AllDirectories);
                foreach (var protoFile in filesPath)
                {
                    Proto2CS(CommandHelper.Instance.Namespace, protoFile, CommandHelper.Instance.OutputPath);
                }
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
        enum State
        {
            Nothings = 0, //当前没事
            Messageing = 1, //当前未message状态
            Enuming = 2, //当前未枚举状态
        }
        public static void Proto2CS(string ns, string protoName, string outputPath)
        {
            //string proto = Path.Combine(protoPath, protoName);
            //Console.WriteLine($"proto {proto}");
            string csPath = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(protoName) + ".cs");

            Console.WriteLine($"read {protoName}");
            string s = File.ReadAllText(protoName);

            StringBuilder sb = new StringBuilder();
            var linesArr = s.Split('\n');
            var lines = new ArrayList();

            var foundPackge = false;
            //过滤无效的行 并解析头
            foreach (string line in linesArr)
            {
                string newline = line.Trim();
                if (newline == "")
                {
                    continue;
                }
                if (newline.StartsWith("syntax"))
                {
                    continue;
                }

                if (newline.StartsWith("package"))
                {
                    foundPackge = true;
                    int idx = newline.IndexOf(";");
                    newline = newline.Remove(idx);
                    string[] ss = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                    PraseImport(sb, ss[1], ns);
                    continue;
                }
                if (newline.StartsWith("import"))
                {
                    continue;
                }

                var addL = false;
                if (newline.Contains('{') && !newline.StartsWith("{"))
                {
                    addL = true;
                    int idx = newline.IndexOf("{");
                    newline = newline.Substring(0, idx) + newline.Substring(idx + 1);
                }
                var addR = false;
                if (newline.Contains('}') && !newline.StartsWith("}"))
                {
                    addR = true;
                    int idx = newline.IndexOf("}");
                    newline = newline.Substring(0, idx) + newline.Substring(idx + 1);
                }
                lines.Add(newline);
                if (addL)
                {
                    lines.Add("{");
                }
                if (addR)
                {
                    lines.Add("}");
                }
            }

            if (!foundPackge)
            {
                throw new Exception($"{protoName}: not found package");
            }


            var state = State.Nothings;
            var index = -1;
            while (++index < lines.Count)
            {
                string newline = lines[index].ToString();
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
                            {
                                throw new Exception($"{protoName}: messsage and enum not suport nest");
                            }
                            if (PraseMessageBody(protoName, newline, sb))
                            {
                                state = State.Nothings;
                            }
                            break;
                        }
                    case State.Enuming:
                        {
                            if (newline.StartsWith("message") || newline.StartsWith("enum"))
                            {
                                throw new Exception($"{protoName}: messsage and enum not suport nest");
                            }
                            if (PraseEnumBody(protoName, newline, sb))
                            {
                                state = State.Nothings;
                            }

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
                int index = newline.IndexOf(";");
                newline = newline.Remove(index);
                string[] ss = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                string type = ss[1];
                type = ConvertType(type);
                string name = ToUpperFirst(ss[2]);
                int n = int.Parse(ss[4]);

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
                int index = newline.IndexOf(";");
                newline = newline.Remove(index);
                //先把类型截取出来
                var tb = newline.IndexOf("<");
                var te = newline.IndexOf(">");
                if (tb >= te || tb == -1 || te == -1)
                {
                    throw new Exception("map格式不对");
                }
                var inType = newline.Substring(tb, te - tb + 1);
                inType = inType.TrimStart('<');
                inType = inType.TrimEnd('>');
                var typeArr = inType.Split(",", StringSplitOptions.RemoveEmptyEntries);
                if (typeArr.Length != 2)
                {
                    throw new Exception("map格式不对");
                }
                var type1 = ConvertType(typeArr[0].Trim());
                var type2 = ConvertType(typeArr[1].Trim());

                newline = newline.Substring(0, tb) + " ? " + newline.Substring(te + 1);
                string[] ss = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                string name = ToUpperFirst(ss[2]);
                int n = int.Parse(ss[4]);

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
            string typeCs = "";
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
            string msgName = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries)[1];
            string[] ss = newline.Split(new[] { "//" }, StringSplitOptions.RemoveEmptyEntries);

            if (ss.Length == 2)
            {
                parentClass = ss[1].Trim();
                parentClass = parentClass == "" ? parentClass : parentClass.Split(" ")[0];
            }

            //sb.Append($"\t[Message({opcodeClassName}.{msgName})]\n");
            sb.Append($"\t[ProtoContract]\n");
            sb.Append($"\tpublic partial class {msgName}");


            string[] msgTypes = { "IMessage", "IRequest", "IResponse", "IRequsetPlayer", "IRequsetWorld" };
            if (msgTypes.ToList().IndexOf(parentClass) != -1)
            {
                sb.Append($": {parentClass}\n");
            }
            else if (parentClass != "")
            {
                sb.Append($": {parentClass}\n");
            }
            else
            {
                sb.Append(": IMessage\n");
            }
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
            {
                Repeated(protoName, sb, newline);
            }
            else if (newline.StartsWith("map"))
            {
                Map(protoName, sb, newline);
            }
            else
            {
                MessageMembers(protoName, sb, newline, true);
            }
            return false;
        }
        private static void MessageMembers(string protoName, StringBuilder sb, string newline, bool isRequired)
        {
            int index = newline.IndexOf(";");
            newline = newline.Remove(index);
            string[] ss = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                string type = ss[0];
                string name = ToUpperFirst(ss[1]);
                int n = int.Parse(ss[3]);
                string typeCs = ConvertType(type);

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
            string msgName = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries)[1];
            string[] ss = newline.Split(new[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
            //sb.Append($"\t[Message({opcodeClassName}.{msgName})]\n"); //用csv来生成rpc的id。各灵活可控一些
            sb.Append($"\t[ProtoContract]\n");
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
            int index = newline.IndexOf(";");
            newline = newline.Remove(index);
            string[] ss = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                string name = ToUpperFirst(ss[0]);
                int n = int.Parse(ss[2]);
                //sb.Append($"\t\t[ProtoEnum]\n"); //v3版本不需要了
                sb.Append($"\t\t{name} = {n},\n\n");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{newline}\n {e}");
            }
        }

        public static unsafe string ToUpperFirst(this string str)
        {
            if (str == null) return null;
#pragma warning disable CS0618 // 类型或成员已过时
            string ret = string.Copy(str);
#pragma warning restore CS0618 // 类型或成员已过时
            fixed (char* ptr = ret)
                *ptr = char.ToUpper(*ptr);
            return ret;
        }
    }
}
