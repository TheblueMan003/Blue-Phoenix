using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp
{
    public abstract class CompilerCore
    {
        public abstract string DefineVariable(string name, string type, bool entity);
        public abstract string DefineFunction(string name);
        public abstract string CallFunction(string name);
        public abstract string Condition(string name);
        public abstract string ConditionBlock(string name);
    }
}
