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

        public override string DefineVariable(Compiler.Variable var)
        {
            if (var.entity)
            {
                return "scoreboard objectives add " + var.scoreboard().Replace("@s ", "") + " " + var.def;
            }
            else
                return "";
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
    }
}
