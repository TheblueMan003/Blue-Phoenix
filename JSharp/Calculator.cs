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

            if (float.TryParse(val2, out float d))
                return d;
            try
            {
                foreach (string xop in operations)
                {
                    part = Compiler.smartSplit(val2, xop[0], 2);

                    if (part.Length > 1)
                    {
                        float a = 0, b = 0;
                        Task<bool> task1 = Task<bool>.Factory.StartNew(() => TryCalculate(part[0],out a));
                        Task<bool> task2 = Task<bool>.Factory.StartNew(() => TryCalculate(part[1],out b));

                        if (xop == "*")
                        {
                            if (task1.Result && task2.Result)
                                return a * b;
                            else
                                throw new Exception("Calculation Error: " + val);
                        }
                        if (xop == "/")
                        {
                            if (task1.Result && task2.Result)
                                return a / b;
                            else
                                throw new Exception("Calculation Error: " + val);
                        }
                        if (xop == "-")
                        {
                            if (task1.Result && task2.Result)
                                return a - b;
                            else
                                throw new Exception("Calculation Error: " + val);
                        }
                        if (xop == "+")
                        {
                            if (task1.Result && task2.Result)
                                return a + b;
                            else
                                throw new Exception("Calculation Error: " + val);
                        }
                        if (xop == "%")
                        {
                            if (task1.Result && task2.Result)
                                return a % b;
                            else
                                throw new Exception("Calculation Error: " + val);
                        }
                    }
                }
            }
            catch
            {
                throw new Exception("Calculation Error: " + val);
            }
            throw new Exception("Calculation Error: "+val);
        }
        public static bool TryCalculate(string val, out float a)
        {
            try
            {
                a = Calculate(val);
                return true;
            }
            catch(Exception e)
            {
                a = 0;
                return false;
            }
        }
    }
}
