using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp
{
    public abstract class CompilerCore
    {
        public abstract string LoadBase();
        public abstract string MainBase();
        public abstract string DefineScoreboard(Compiler.Scoreboard var);
        public abstract string VariableOperation(Compiler.Variable var1, Compiler.Variable var2, string op, string selector1 = "", string selector2 = "");
        public abstract string VariableOperation(Compiler.Variable var, int value, string op, string selector = "");
        public abstract string[] CompareVariable(Compiler.Variable var1, Compiler.Variable var2, string op, string selector1 = "", string selector2 = "");
        public abstract string[] CompareVariable(Compiler.Variable var1, int value, string op, string selector1 = "");
        public abstract string[] CompareVariable(Compiler.Variable var1, int value1, int value2, string selector1 = "");
        public abstract string[] ConditionEntity(string entity);
        public abstract string[] ConditionInverse(string[] val);
        public abstract string[] ConditionBlock(string val);
        public abstract string[] ConditionBlocks(string val);
        public abstract string Condition(string val);
        public abstract string VariableSetNull(Compiler.Variable var, string selector = "");
        public abstract string FormatTagsPath(string path);

        public abstract string AsAt(string entity, string cond = "");
        public abstract string As(string entity, string cond = "");
        public abstract string At(string entity, string cond = "");
        public abstract string Positioned(string value);
        public abstract string Align(string value);

        public abstract string DefineFunction(Compiler.Function function);
        public abstract string CallFunction(Compiler.Function function);

        public abstract bool isValidSelector(string selector);
        public abstract bool isValidSelectorArgument(string arg);
        public abstract string getLibraryFolder();
    }
}
