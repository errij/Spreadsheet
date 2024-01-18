using System.Numerics;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    public class Evaluator
    {
        public delegate int Lookup(String var);

        public static int Evaluate(String expression, Lookup variableEvaluator)
        {
            expression = expression.Replace(" ", "");
            String[] substrings = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            var OperStack = new Stack<String>(); // Operation stack
            var ValStack = new Stack<int>();    // Value stack

            foreach (string x in substrings)
            {
                if (CheckExpression(x) == 0 || CheckExpression(x) == -1)
                {
                    int current;

                    if(CheckExpression(x) == -1)
                    {
                        current = variableEvaluator(x);
                    }
                    else
                    {
                        current = int.Parse(x);
                    }

                    if (OperStack.Count > 0 && (CheckExpression(OperStack.Peek()) == 3 || CheckExpression(OperStack.Peek()) == 4))
                    {
                        int temp = ValStack.Pop();

                        if (CheckExpression(OperStack.Pop()) == 3)
                        {
                            ValStack.Push(temp * current);
                        }
                        else
                        {
                            if (current == 0)
                            {
                                throw new Exception("division by zero");
                            }
                            ValStack.Push(temp / current);
                        }
                    }
                    else
                    {
                        ValStack.Push(current);
                    }
                }
                else if (CheckExpression(x) == 6)
                {
                    while (OperStack.Count > 0 && CheckExpression(OperStack.Peek()) != 5)
                    {
                        int num1 = ValStack.Pop();
                        int num2 = ValStack.Pop();

                        if (CheckExpression(OperStack.Pop()) == 1)
                        {
                            ValStack.Push(num2 + num1);
                        }
                        else
                        {
                            ValStack.Push(num2 - num1);
                        }
                    }

                    OperStack.Pop();

                    if (OperStack.Count > 0 && (CheckExpression(OperStack.Peek()) == 3 || CheckExpression(OperStack.Peek()) == 4))
                    {
                        int num1 = ValStack.Pop();
                        int num2 = ValStack.Pop();

                        if (CheckExpression(OperStack.Pop()) == 3)
                        {
                            ValStack.Push(num2 * num1);
                        }
                        else
                        {
                            ValStack.Push(num2 / num1);
                        }
                    }
                }
                else if (CheckExpression(x) > 0)
                {
                     OperStack.Push(x);
                }
            }

            while (OperStack.Count > 0)
            {
                int num1 = ValStack.Pop();
                int num2 = ValStack.Pop();

                if (CheckExpression(OperStack.Peek()) == 3 || CheckExpression(OperStack.Peek()) == 4)
                {
                    throw new Exception("There are more operators than numbers");
                }

                if (CheckExpression(OperStack.Pop()) == 1)
                {
                    ValStack.Push(num2 + num1);
                }
                else
                {
                    ValStack.Push(num2 - num1);
                }
            }

            if(ValStack.Count > 1)
            {
                throw new Exception("There are more numbers than operators");
            }

            return ValStack.Pop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static int CheckExpression(String expression)
        {
            if (int.TryParse(expression, out _))
            {
                return 0;
            }
            else if (expression.Equals("+"))
            {
                return 1;
            }
            else if (expression.Equals("-"))
            {
                return 2;
            }
            else if (expression.Equals("*"))
            {
                return 3;
            }
            else if (expression.Equals("/"))
            {
                return 4;
            }
            else if (expression.Equals("("))
            {
                return 5;
            }
            else if (expression.Equals(")"))
            {
                return 6;
            }
            else
            {
                return -1;
            }
        }
    }

}
