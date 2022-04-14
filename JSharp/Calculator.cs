using System;

namespace JSharp
{
    public static class Calculator
    {
        public static float Calculate(string val)
        {
            string[] part;
            string[] operations = new string[] { "+", "-", "%", "/", "|", "*", "^" };

            string val2 = Compiler.getParenthis(val, 1);

            if (float.TryParse(val2, out float d))
                return d;
            try
            {
                foreach (string xop in operations)
                {
                    part = Compiler.smartSplit(val2, xop[0], 1);

                    if (part.Length > 1)
                    {
                        float a = 0, b = 0;
                        a = Calculate(part[0]);
                        b = Calculate(part[1]);

                        if (xop == "^")
                        {
                            return (float)Math.Pow(a, b);
                        }
                        if (xop == "*")
                        {
                            return a * b;
                        }
                        if (xop == "/")
                        {
                            return a / b;
                        }
                        if (xop == "|")
                        {
                            return (float)Math.Floor(a / b);
                        }
                        if (xop == "-")
                        {
                            return a - b;
                        }
                        if (xop == "+")
                        {
                            return a + b;
                        }
                        if (xop == "%")
                        {
                            return a % b;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Calculation Error: " + val + e.ToString());
            }
            throw new Exception("Calculation Error: " + val);
        }
        public static bool TryCalculate(string val, out float a)
        {
            try
            {
                a = Calculate(val);
                return true;
            }
            catch
            {
                a = 0;
                return false;
            }
        }
    }
}
