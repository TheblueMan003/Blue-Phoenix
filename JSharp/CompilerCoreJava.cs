using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp
{
    public class CompilerCoreJava : CompilerCore
    {
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

        public override string CallFunction(Compiler.Function function)
        {
            return "function " + function.gameName;
        }

        public override string DefineFunction(Compiler.Function function)
        {
            throw new NotImplementedException();
        }

        private string GetSelector(Compiler.Variable var1, string selector="")
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

        public override string DefineVariable(Compiler.Variable var)
        {
            if (var.entity)
            {
                return "scoreboard objectives add " + var.scoreboard().Replace("@s ","") + " " + var.def;
            }
            else
                return "";
        }

        public override string VariableOperation(Compiler.Variable var1, Compiler.Variable var2, string op, string selector1 = "", string selector2 = "")
        {
            return "scoreboard players operation " + GetSelector(var1, selector1) + " "+op+" " + GetSelector(var2, selector2)+"\n";
        }
        public override string VariableOperation(Compiler.Variable var, int value, string op, string selector = "")
        {
            if (op == "=")
                return "scoreboard players set "+ GetSelector(var, selector)+ " " + value.ToString()+"\n";
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
            return "scoreboard players reset " + GetSelector(var, selector)+"\n";
        }

        public override string[] CompareVariable(Compiler.Variable var1, Compiler.Variable var2, string op, string selector1 = "", string selector2 = "")
        {
            if (op == "==")
                op = "=";
            return new string[] { "if score " + GetSelector(var1, selector1) + " " + op + " " + GetSelector(var2, selector2) + " " ,""};
        }
        public override string[] CompareVariable(Compiler.Variable var1, int value, string op, string selector1 = "")
        {
            if (op == "=" || op == "==")
            {
                return new string[] { "if score " + GetSelector(var1, selector1) + " matches " + value.ToString() + " ", "" };
            }
            else if (op == "<=")
            {
                return new string[] { "if score " + GetSelector(var1, selector1) + " matches .." + value.ToString() + " ", "" };
            }
            else if (op == "<")
            {
                return new string[] { "if score " + GetSelector(var1, selector1) + " matches .." + (value-1).ToString() + " ", "" };
            }
            else if (op == ">=")
            {
                return new string[] { "if score " + GetSelector(var1, selector1) + " matches " + value.ToString() + ".. ", "" };
            }
            else if (op == ">")
            {
                return new string[] { "if score " + GetSelector(var1, selector1) + " matches " + (value+1).ToString() + ".. ", "" };
            }
            else if (op == "!=")
            {
                return new string[] { "unless score " + GetSelector(var1, selector1) + " matches " + value.ToString() + " ", "" };
            }
            else
            {
                throw new Exception("Unundelled Operation: "+op);
            }
        }
        public override string[] CompareVariable(Compiler.Variable var1, int value1, int value2, string selector1 = "")
        {
            return new string[] { "if score " + GetSelector(var1, selector1) + " matches " + value1.ToString() + ".."+ value2.ToString()+" ", "" };
        }
        public override string[] ConditionEntity(string entity)
        {
            return new string[] { "if entity " + entity+" ", "" };
        }
        public override string[] ConditionInverse(string[] val)
        {
            if (val[0].StartsWith("if "))
            {
                return new string[] { "unless " + val[0].Substring(3, val[0].Length - 3),val[1]};
            }
            else if (val[0].StartsWith("unless "))
            {
                return new string[] { "if " + val[0].Substring(7, val[0].Length - 7),val[1]};
            }
            else
                throw new Exception("Invalid Condition"+val[0]+";"+val[1]);
        }
        public override string[] ConditionBlock(string val)
        {
            if (Compiler.smartSplit(val, ' ').Length == 1)
            {
                val = "~ ~ ~ " + val;
            }
            return new string[] { "if block " + val+" ", "" };
        }
        public override string[] ConditionBlocks(string val)
        {
            if (!val.EndsWith("all") && !val.EndsWith("masked"))
                val += " all";
            return new string[] { "if blocks " + val+" ", "" };
        }
        public override string Condition(string val)
        {
            return "execute " + val + "run ";
        }

        public override string AsAt(string entity, string cond = "")
        {
            if (cond == "")
                return "execute as " + entity + " at @s run ";
            else
                return "execute as " + entity + " at @s "+cond+"run ";
        }
        public override string As(string entity, string cond = "")
        {
            if (cond == "")
                return "execute as " + entity + " run ";
            else
                return "execute as " + entity + " "+cond+"run ";
        }
        public override string At(string entity, string cond = "")
        {
            if (cond == "")
                return "execute at " + entity + " run ";
            else
                return "execute at " + entity + " " + cond + "run ";
        }
        public override string Positioned(string value)
        {
            return "execute positioned " + value+" run ";
        }
        public override string Align(string value)
        {
            return "execute align " + value+" run ";
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
                part[0] == "limit")
            {
                return float.TryParse(part[1], out float t);
            }
            if (part[0] == "distance" || part[0] == "level" || part[0] == "x_rotation" || part[0] == "y_rotation")
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
            if (part[0] == "gamemode")
            {
                part[1] = part[1].Replace("!", "");
                return part[1] == "adventure" || part[1] == "survival" || part[1] == "creative" || part[1] == "spectator";
            }
            if (part[0] == "sort")
            {
                return part[1] == "nearest" || part[1] == "furthest" || part[1] == "random" || part[1] == "arbitrary";
            }
            if (part[0] == "tag" || part[0] == "name" || part[0] == "team" || part[0] == "type")
            {
                return !part[1].Contains("\"");
            }
            if ("nbt".StartsWith(part[0]) || part[0] == "advancements‌" || part[0] == "predicate‌" || part[0] == "scores")
            {
                return true;
            }

            return false;
        }

        public override string getLibraryFolder() { return "java"; }
    }
}
