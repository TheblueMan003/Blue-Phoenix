using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp
{
    public class CompilerCoreJava : CompilerCore
    {
        private bool OffuscateNeed;
        private static long[] pow64 = new long[11];
        private Dictionary<string, string> offuscationMap;
        public static string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._";
        private string dirVar;

        public CompilerCoreJava(bool offuscate, string project)
        {
            OffuscateNeed = offuscate;

            dirVar = project.Substring(0, Math.Min(3, project.Length));
        }

        public override string CallFunction(string name)
        {
            return "function " + name;
        }

        public override string DefineFunction(string name)
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
                if (selector.Length>1)
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
            return new string[] { "if block " + val+" ", "" };
        }
        public override string[] ConditionBlocks(string val)
        {
            return new string[] { "if blocks " + val+" ", "" };
        }
        public override string Condition(string val)
        {
            return "execute " + val + "run ";
        }
        public override string VariableSetNull(Compiler.Variable var, string selector = "")
        {
            return "scoreboard players reset " + GetSelector(var, selector);
        }
    }
}
