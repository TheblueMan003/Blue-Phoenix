using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp
{
    public abstract class CompilerCore
    {
        public abstract string DefineVariable(Compiler.Variable var);
        public abstract string VariableOperation(Compiler.Variable var1, Compiler.Variable var2, string op, string selector1 = "", string selector2 = "");
        public abstract string VariableSetImmediate(Compiler.Variable var, string value, string selector = "");
        public abstract string VariableSetNull(Compiler.Variable var, string selector = "");

        public abstract string DefineFunction(string name);
        public abstract string CallFunction(string name);
        public abstract string Condition(string name);
        public abstract string ConditionBlock(string name);
    }
}
