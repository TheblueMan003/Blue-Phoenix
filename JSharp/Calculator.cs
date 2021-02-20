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
                    if (xop == "*")
                        return Calculate(part[0]) * Calculate(part[1]);
                    if (xop == "/")
                        return Calculate(part[0]) / Calculate(part[1]);
                    if (xop == "%")
                        return Calculate(part[0]) % Calculate(part[1]);
                    if (xop == "-")
                        return Calculate(part[0]) - Calculate(part[1]);
                    if (xop == "+")
                        return Calculate(part[0]) + Calculate(part[1]);
                }
            }

            throw new Exception("Calculation Error");
        }
    }
}
