using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp
{
    public static class Calculator
    {
        public static float Calculate(string val)
        {
            string[] part;
            string[] operations = new string[] { "+", "-", "%", "/", "*" };

            string val2 = Compiler.getParenthis(val, 1);

            if (float.TryParse(val2, out float a))
                return a;

            foreach (string xop in operations)
            {
                part = Compiler.smartSplit(val2, xop[0], 2);

                if (part.Length > 1)
                {
                    Task<float> task1 = Task<float>.Factory.StartNew(() => Calculate(part[0]));
                    Task<float> task2 = Task<float>.Factory.StartNew(() => Calculate(part[1]));

                    if (xop == "*")
                        return task1.Result * task2.Result;
                    if (xop == "/")
                        return task1.Result / task2.Result;
                    if (xop == "%")
                        return task1.Result % task2.Result;
                    if (xop == "-")
                        return task1.Result - task2.Result;
                    if (xop == "+")
                        return task1.Result + task2.Result;
                }
            }

            throw new Exception("Calculation Error: "+val);
        }
    }
}
