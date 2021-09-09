using JSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluePhoenix
{
    public static class ExpressionTokenizer
    {
        public abstract class Expression
        {
            public abstract Expression Shorted();
        }
        public class Variable : Expression
        {
            private Compiler.Variable variable;

            public Variable(Compiler.Variable variable)
            {
                this.variable = variable;
            }
            public override Expression Shorted()
            {
                return this;
            }
        }
        public class Constant : Expression
        {
            public Compiler.Variable variable;
            public double valueDouble;
            public int valueInt;
            public bool isFloat;
            public Constant(double value)
            {
                variable = Compiler.GetConstant(value);
                this.valueDouble = value;
                isFloat = true;
            }
            public Constant(int value)
            {
                variable = Compiler.GetConstant(value);
                this.valueInt = value;
                isFloat = false;
            }
            public override Expression Shorted()
            {
                return this;
            }
            public double GetDoubleValue()
            {
                return isFloat ? valueDouble : valueInt;
            }
        }
        public class Addition : Expression
        {
            private Expression left, right;
            public Addition(Expression left, Expression right)
            {
                this.left = left;
                this.right = right;
            }
            public override Expression Shorted()
            {
                if (left is Constant && right is Constant)
                {
                    Constant l = (Constant)left;
                    Constant r = (Constant)right;
                    if (!l.isFloat && !r.isFloat)
                    {
                        return new Constant(l.valueInt + r.valueInt);
                    }
                    else
                    {
                        return new Constant(l.GetDoubleValue() + r.GetDoubleValue());
                    }
                }
                else
                {
                    return this;
                }
            }
        }
        public class Multiplication : Expression
        {
            private Expression left, right;
            public Multiplication(Expression left, Expression right)
            {
                this.left = left;
                this.right = right;
            }
            public override Expression Shorted()
            {
                if (left is Constant && right is Constant)
                {
                    Constant l = (Constant)left;
                    Constant r = (Constant)right;
                    if (!l.isFloat && !r.isFloat)
                    {
                        return new Constant(l.valueInt * r.valueInt);
                    }
                    else
                    {
                        return new Constant(l.GetDoubleValue() * r.GetDoubleValue());
                    }
                }
                else
                {
                    return this;
                }
            }
        }
    }
}
