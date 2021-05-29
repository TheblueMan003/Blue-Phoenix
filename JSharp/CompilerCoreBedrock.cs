using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp
{
    public class CompilerCoreBedrock : CompilerCore
    {
        private int condInv=0;
        public override string LoadBase()
        {
            return "scoreboard objectives add tbms.value dummy\n" +
                "scoreboard objectives add tbms.const dummy\n" +
                "scoreboard objectives add tbms.tmp dummy\n";
        }
        public override string MainBase()
        {
            return "";
        }

        public override string Align(string value)
        {
            throw new NotImplementedException();
        }
        public override string As(string entity, string cond = "")
        {
            return cond + "execute " + entity + " ~ ~ ~ ";
        }
        public override string AsAt(string entity, string cond = "")
        {
            return cond + "execute " + entity + " ~ ~ ~ ";
        }
        public override string At(string entity, string cond = "")
        {
            return cond + "execute " + entity + " ~ ~ ~ ";
        }
        public override string Positioned(string value)
        {
            return "execute @s " + value + " ";
        }


        public override string[] CompareVariable(Compiler.Variable var1, Compiler.Variable var2, string op, string selector1 = "", string selector2 = "")
        {
            var c = condInv++;
            var name = "__cond_bedrock__" + c.ToString();
            string def = Compiler.parseLine(var1.type.ToString().ToLower()+" "+name + " = "+var1.gameName);
            def += Compiler.parseLine(name + " -= " + var2.gameName);
            string[] res = CompareVariable(Compiler.GetVariableByName(name), 0, op, selector1);
            
            return new string[] { res[0], res[1]+"\n"+def };
        }
        public override string[] CompareVariable(Compiler.Variable var1, int value, string op, string selector1 = "")
        {
            if (op == "=" || op == "==")
                return new string[] { "execute " + GetSelectorEntity(var1, value.ToString(), selector1) + " ~ ~ ~ ", "" };
            else if (op == "<=")
                return new string[] { "execute " + GetSelectorEntity(var1, ".." + value.ToString(), selector1) + " ~ ~ ~ ", "" };
            else if (op == ">=")
                return new string[] { "execute " + GetSelectorEntity(var1, value.ToString() + "..", selector1) + " ~ ~ ~ ", "" };
            else if (op == "<")
                return new string[] { "execute " + GetSelectorEntity(var1, ".." + (value - 1).ToString(), selector1) + " ~ ~ ~ ", "" };
            else if (op == ">")
                return new string[] { "execute " + GetSelectorEntity(var1, (value + 1).ToString() + "..", selector1) + " ~ ~ ~ ", "" };
            else
                throw new Exception("Unsupported Operator " + op);
        }
        public override string[] CompareVariable(Compiler.Variable var1, int value1, int value2, string selector1 = "")
        {
            return new string[] { "execute " + GetSelectorEntity(var1, value1.ToString()+".."+value2.ToString(), selector1) + " ~ ~ ~ ", "" };
        }
        public override string Condition(string val)
        {
            return val;
        }
        public override string[] ConditionBlock(string val)
        {
            return new string[] { "execute @s ~ ~ ~ detect " + val +" ",""};
        }
        public override string[] ConditionBlocks(string val)
        {
            throw new NotImplementedException();
        }
        public override string[] ConditionEntity(string entity)
        {
            return new string[] { "execute "+entity+" ~ ~ ~ ","" };
        }

        public override string[] ConditionInverse(string[] val)
        {
            var c = condInv++;
            string def = Compiler.parseLine("bool __cond_bedrock__" + c.ToString() + " = true");
            string condMod = val[0]+Compiler.parseLine("__cond_bedrock__" + c.ToString() + " = false");
            return new string[] { Compiler.getCondition("__cond_bedrock__" + c.ToString()), val[1] + "\n" + def + "\n" + condMod };
        }

        public override string DefineFunction(Compiler.Function function)
        {
            throw new NotImplementedException();
        }
        public override string CallFunction(Compiler.Function function)
        {
            return "function " + function.gameName.Replace(":", "/");
        }
        public override string[] FileNameSplitter()
        {
            return new string[] { "/", "/" };
        }

        public override string DefineScoreboard(Compiler.Scoreboard var)
        {
            return "scoreboard objectives add " + var.name + " " + var.property;
        }
        public override string VariableOperation(Compiler.Variable var1, Compiler.Variable var2, string op, string selector1 = "", string selector2 = "")
        {
            return "scoreboard players operation " + GetSelector(var1, selector1) + " " + op + " " + GetSelector(var2, selector2) + "\n";
        }
        public override string VariableOperation(Compiler.Variable var, int value, string op, string selector = "")
        {
            if (op == "=")
                return "scoreboard players set " + GetSelector(var, selector) + " " + value.ToString() + "\n";
            if (op == "+=")
                return "scoreboard players add " + GetSelector(var, selector) + " " + value.ToString() + "\n";
            if (op == "-=")
                return "scoreboard players remove " + GetSelector(var, selector) + " " + value.ToString() + "\n";
            if (op == "*=")
                return "scoreboard players operation " + GetSelector(var, selector) + " *= " + GetSelector(Compiler.GetConstant(value), "") + "\n";
            if (op == "/=")
                return "scoreboard players operation " + GetSelector(var, selector) + " /= " + GetSelector(Compiler.GetConstant(value), "") + "\n";
            if (op == "%=")
                return "scoreboard players operation " + GetSelector(var, selector) + " %= " + GetSelector(Compiler.GetConstant(value), "") + "\n";
            throw new Exception("Unsupported Operator: " + op);
        }
        public override string VariableSetNull(Compiler.Variable var, string selector = "")
        {
            return "scoreboard players reset " + GetSelector(var, selector);
        }

        private string GetSelector(Compiler.Variable var1, string selector = "")
        {
            if (var1.entity)
            {
                if (selector != "")
                {
                    return var1.scoreboard().Replace("@s", selector);
                }
                else
                {
                    return var1.scoreboard();
                }
            }
            else
            {
                if (selector.Length > 1)
                {
                    throw new Exception("Can not asign " + var1.gameName + " to " + selector);
                }
                else
                {
                    return var1.scoreboard();
                }
            }
        }
        private string GetSelectorEntity(Compiler.Variable var1, string val, string selector = "")
        {
            if (selector == "")
                return "@s[scores={" + var1.scoreboard().Replace("@s ", "") + "=" + val + "}]";
            else
            {
                if (selector.Contains("]"))
                    return selector.Substring(0,selector.LastIndexOf("]"))+
                        ",scores={" + var1.scoreboard().Replace("@s ", "") + "=" + val + "}]";
                else
                    return selector+"[scores={" + var1.scoreboard().Replace("@s ", "") + "=" + val + "}]";
            }
        }

        public override bool isValidSelector(string selector)
        {
            selector = Compiler.smartEmpty(selector);
            if (selector.Contains("["))
            {
                if (!selector.EndsWith("]"))
                    return false;
                string args = selector.Substring(selector.IndexOf("[") + 1,
                    selector.LastIndexOf("]") - selector.IndexOf("[") - 1);
                foreach (string arg in Compiler.smartSplitJson(args, ',', -1))
                {
                    if (!isValidSelectorArgument(arg))
                        return false;
                }
                if (selector.StartsWith("@s") || selector.StartsWith("@p") ||
                    selector.StartsWith("@a") || selector.StartsWith("@r") || selector.StartsWith("@e"))
                    return true;
                else
                    return false;
            }
            else
            {
                if (selector == "@s" || selector == "@p" || selector == "@a" || selector == "@r" || selector == "@e")
                    return true;
                else
                    return false;
            }
        }
        public override bool isValidSelectorArgument(string arg)
        {
            arg = Compiler.smartEmpty(arg);
            string[] part = Compiler.smartSplit(arg, '=');
            if (part.Length != 2)
                return false;

            if (part[0] == "x" || part[0] == "y" || part[0] == "z" || part[0] == "dx" || part[0] == "dy" || part[0] == "dz" ||
                part[0] == "c")
            {
                return float.TryParse(part[1], out float t);
            }
            if (part[0] == "r" || part[0] == "level" || part[0] == "rx" || part[0] == "ry")
            {
                if (part[1].Contains(".."))
                {
                    string[] part2 = part[1].Replace("..", ",").Split(',');

                    return (part2[0] == "" || float.TryParse(part2[0], out float t)) && (part2[1] == "" || float.TryParse(part2[1], out float u));
                }
                else
                {
                    return float.TryParse(part[1], out float t);
                }
            }
            if (part[0] == "g")
            {
                part[1] = part[1].Replace("!", "");
                return part[1] == "adventure" || part[1] == "survival" || part[1] == "creative" || part[1] == "spectator";
            }
            if (part[0] == "tag" || part[0] == "name" || part[0] == "team" || part[0] == "type")
            {
                return !part[1].Contains("\"");
            }
            if (part[0] == "scores")
            {
                return true;
            }

            return false;
        }

        public override string getLibraryFolder() { return "bedrock";}

        public override string FormatTagsPath(string path)
        {
            throw new NotImplementedException();
        }

        public override string FormatFunctionPath(string path)
        {
            throw new NotImplementedException();
        }

        public override string[] FormatJson(string[] args, Compiler.Context context, int start = 0)
        {
            string[] outs = FormatJsonInternal(args, context, start);
            return new string[] { "{ \"rawtext\" :"+outs[0]+"}", outs[1], outs[2] };
        }
        public string[] FormatJsonInternal(string[] args, Compiler.Context context, int start = 0)
        {
            string output = "[";
            string output2 = "";
            string output3 = "";
            int strTag = 0;
            HashSet<string> unpackedFloat = new HashSet<string>();

            for (int i = start; i < args.Length; i++)
            {
                string arg = Compiler.smartEmpty(args[i]).StartsWith("(") ? Compiler.smartEmpty(args[i].Substring(args[i].IndexOf('(') + 1, args[i].LastIndexOf(')') - args[i].IndexOf('(') - 1)) : args[i];
                string[] subargs = Compiler.smartSplit(arg, ',');
                bool ignoreFormat = false;
                if (subargs[0].Contains("\""))
                {
                    string ext = Compiler.smartExtract(subargs[0]);
                    if (ext.StartsWith("\"") && ext.EndsWith("\""))
                    {
                        output += ",{\"text\":" + subargs[0];
                    }
                    else if (ext.StartsWith("f\"") && ext.EndsWith("\""))
                    {
                        string[] part = Compiler.extractString(ext.Substring(1, ext.Length - 1)).Replace("{", "}").Split('}');

                        var a2 = Enumerable.Range(0, part.Length);
                        var recArg = (subargs.Length > 1) ? subargs.Skip(1).Aggregate((x, y) => x + "," + y) : "";
                        var part2 = part.Zip(a2, (first, second) => second % 2 == 0 ? "\"" + first + "\"" : first)
                                        .Where(x => x != "\"\"")
                                        .Select(x => "(" + x + "," + recArg + ")")
                                        .Aggregate((x, y) => x + "," + y);


                        string[] outs = FormatJsonInternal(Compiler.smartSplit(part2, ','), context);
                        output += "," + outs[0].Substring(1, outs[0].Length - 2);
                        output2 += outs[1];
                        output3 += outs[2];


                        ignoreFormat = true;
                    }
                    else
                    {
                        throw new Exception("JSON Syntaxe Error");
                    }
                }
                else if (subargs[0].Contains("@"))
                {
                    output += ",{\"selector\":\"" + subargs[0] + "\"";
                }
                else if (float.TryParse(subargs[0], out float _))
                {
                    output += ",{\"text\":\"" + subargs[0] + "\"";
                }
                else if (Compiler.isStringVar(subargs[0]))
                {
                    string tmp = Compiler.getString(subargs[0]);
                    tmp += "tag @e[tag=__str__,tag=!__str__tag__] add __str_" + strTag.ToString() + "\n";
                    tmp += "tag @e[tag=__str__,tag=!__str__tag__] add __str__tag__" + "\n";
                    output += ",{\"selector\":\"" + "@e[tag=__str_" + strTag.ToString() + "]" + "\"";
                    strTag++;
                    output2 += tmp;
                }
                else if (context.GetVarType(subargs[0]) == Compiler.Type.ENTITY)
                {
                    output += ",{\"selector\":\"" + context.GetEntitySelector(subargs[0]) + "\"";
                }
                else if (context.GetVarType(subargs[0]) == Compiler.Type.ARRAY)
                {
                    int nb = Compiler.GetVariable(context.GetVariable(subargs[0])).arraySize;
                    ignoreFormat = true;
                    for (int j = 0; j < Compiler.GetVariable(context.GetVariable(subargs[0])).arraySize; j++)
                    {
                        var va = Compiler.GetVariable(context.GetVariable(subargs[0] + "." + j.ToString()));
                        if (va.type == Compiler.Type.STRING)
                        {
                            string tmp = Compiler.getString(subargs[0]);
                            tmp += "tag @e[tag=__str__,tag=!__str__tag__] add __str_" + strTag.ToString() + "\n";
                            tmp += "tag @e[tag=__str__,tag=!__str__tag__] add __str__tag__" + "\n";
                            output += ",{\"selector\":\"" + "@e[tag=__str_" + strTag.ToString() + "]" + "\"";
                            output += jsonSubArg(subargs, context);
                            output += "}";
                            strTag++;
                            output2 += tmp;
                        }
                        else
                        {
                            if (j == 0)
                            {
                                output += ",{\"text\":" + "\"[\"";
                                output += jsonSubArg(subargs, context);
                                output += "}";
                            }
                            string[] v = Compiler.GetVariableByName(subargs[0] + "." + j.ToString()).scoreboard().Split(' ');
                            output += ",{ \"score\":{ \"name\":\"" + v[0] + "\",\"objective\":\"" + v[1] + "\"}";
                            output += jsonSubArg(subargs, context);
                            output += "}";
                            output += ",{\"text\":" + "\", \"";
                            output += jsonSubArg(subargs, context);
                            output += "}";
                            if (j == nb - 1)
                            {
                                output += ",{\"text\":" + "\"]\"";
                                output += jsonSubArg(subargs, context);
                            }
                        }
                    }
                }
                else if (context.GetVarType(subargs[0]) == Compiler.Type.STRUCT)
                {
                    Compiler.Structure s = Compiler.structs[Compiler.GetVariableByName(subargs[0]).enums];
                    output += ",{\"text\":\"" + s.name + "(" + "\"";
                    output += jsonSubArg(subargs, context);
                    output += "}";
                    foreach (var v in s.fields)
                    {
                        output += ",{\"text\":\"" + v.name + " = " + "\"";
                        output += jsonSubArg(subargs, context);
                        output += "}";

                        string tmp = FormatJsonInternal(new string[] { subargs[0] + "." + v.name }, context, 0)[0];
                        output += "," + tmp.Substring(1, tmp.Length - 2);

                        output += ",{\"text\":\"" + ", " + "\"";
                        output += jsonSubArg(subargs, context);
                        output += "}";
                    }
                    output += ",{\"text\":\"" + ")" + "\"";
                }
                else if (context.GetVarType(subargs[0]) == Compiler.Type.FLOAT)
                {
                    var v = Compiler.GetVariableByName(subargs[0]);
                    if (!unpackedFloat.Contains(v.gameName))
                    {
                        output2 += Compiler.parseLine($"int {v.gameName}.u #= {v.gameName}") + "\n";
                        output2 += Compiler.parseLine($"{v.gameName}.u /={Compiler.compilerSetting.FloatPrecision}") + "\n";
                        output2 += Compiler.parseLine($"int {v.gameName}.l #= {v.gameName}") + "\n";
                        output2 += Compiler.parseLine($"{v.gameName}.l %={Compiler.compilerSetting.FloatPrecision}") + "\n";

                        unpackedFloat.Add(v.gameName);
                    }
                    var vu = Compiler.GetVariableByName($"{v.gameName}.u").scoreboard().Split(' ');
                    var vl = Compiler.GetVariableByName($"{v.gameName}.l").scoreboard().Split(' ');
                    output += ",{ \"score\":{ \"name\":\"" + vu[0] + "\",\"objective\":\"" + vu[1] + "\"}";
                    output += jsonSubArg(subargs, context) + "}";
                    output += ",{\"text\":\".\"";
                    output += jsonSubArg(subargs, context) + "}";
                    output += ",{ \"score\":{ \"name\":\"" + vl[0] + "\",\"objective\":\"" + vl[1] + "\"}";
                }
                else
                {
                    string[] v = Compiler.GetVariableByName(subargs[0]).scoreboard().Split(' ');
                    output += ",{ \"score\":{ \"name\":\"" + v[0] + "\",\"objective\":\"" + v[1] + "\"}";
                }
                if (!ignoreFormat)
                    output += jsonSubArg(subargs, context);

                output += "}";
            }

            output += "]";

            if (strTag > 0)
            {
                output3 += "\nkill @e[tag=__str__]";
            }
            output = "[" + output.Substring(2, output.Length - 2);
            return new string[] { output, output2, output3 };
        }
        public string jsonSubArg(string[] subargs, Compiler.Context context)
        {
            string output = "";
            for (int j = 1; j < subargs.Length; j++)
            {
                //throw new Exception("Not supported!");
            }
            return output;
        }
    }
}
