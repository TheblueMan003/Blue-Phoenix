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
            for (int i = 0; i < 11; i++)
            {
                pow64[i] = IntPow(alphabet.Length, i);
            }
            dirVar = project.Substring(0, Math.Min(3, project.Length));
        }

        public override string CallFunction(string name)
        {
            return "function " + name;
        }

        public override string Condition(string name)
        {
            throw new NotImplementedException();
        }
        public override string ConditionBlock(string name)
        {
            throw new NotImplementedException();
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
                    return selector + " " + offuscate(var1.gameName);
                }
                else
                {
                    return "@s " + offuscate(var1.gameName);
                }
            }
            else
            {
                if (selector != "")
                {
                    throw new Exception("Can not asign " + var1.gameName + " to " + selector);
                }
                else
                {
                    return offuscate(var1.gameName) + (var1.isConst ? " tbms.const" : " tbms.value");
                }
            }
        }

        public override string DefineVariable(Compiler.Variable var)
        {
            if (var.entity)
            {
                return "scoreboard objective add " + offuscate(var.gameName) + " " + var.def;
            }
            else
                return "";
        }

        public override string VariableOperation(Compiler.Variable var1, Compiler.Variable var2, string op, string selector1 = "", string selector2 = "")
        {
            return "scoreboard players operation " + GetSelector(var1, selector1) + " "+op+" " + GetSelector(var2, selector2);
        }

        public override string VariableSetImmediate(Compiler.Variable var, string value, string selector = "")
        {
            return "scoreboard players set "+ GetSelector(var, selector)+ " " + value;
        }

        public override string VariableSetNull(Compiler.Variable var, string selector = "")
        {
            return "scoreboard players reset " + GetSelector(var, selector);
        }



        #region offuscation
        public static long IntPow(long x, long pow)
        {
            long ret = 1;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return ret;
        }
        public string offuscate(string text)
        {
            if (!OffuscateNeed)
                return text;

            if (offuscationMap.ContainsKey(text))
                return offuscationMap[text];

            string rText = "";
            foreach (char ch in text.Reverse())
            {
                rText += ch;
            }
            long hash = text.GetHashCode() + rText.GetHashCode() * (long)(int.MaxValue);
            long c = Math.Abs(hash % pow64[10]);

            string map = getMap(c);

            if (offuscationMap.ContainsKey(map))
            {
                return offuscate(text + "'");
            }

            offuscationMap.Add(text, map);
            return map;
        }
        public string getMap(long c)
        {
            return dirVar + "."
                        + "" + alphabet[(int)((c >> 52) & 63)]
                        + "" + alphabet[(int)((c >> 48) & 63)]
                        + "" + alphabet[(int)((c >> 42) & 63)]
                        + "" + alphabet[(int)((c >> 36) & 63)]
                        + "" + alphabet[(int)((c >> 30) & 63)]
                        + "" + alphabet[(int)((c >> 24) & 63)]
                        + "" + alphabet[(int)((c >> 18) & 63)]
                        + "" + alphabet[(int)((c >> 12) & 63)]
                        + "" + alphabet[(int)((c >> 6) & 63)]
                        + "" + alphabet[(int)(c & 63)];
        }
        #endregion
    }
}
