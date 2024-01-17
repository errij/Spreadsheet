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

            for (int i = 0; i < substrings.Length; i++)
            {
                if (CheckExpression(substrings[i]) == 0)
                {
                    int current = int.Parse(substrings[i]);

                    if (OperStack.Count > 0 && (CheckExpression(OperStack.Peek()) == 3 || CheckExpression(OperStack.Peek()) == 4))
                    {
                        int temp = ValStack.Pop();

                        if (CheckExpression(OperStack.Pop()) == 3)
                        {
                            ValStack.Push(temp * current);
                        }
                        else
                        {
                            ValStack.Push(temp / current);
                        }
                    }
                    else
                    {
                        ValStack.Push(current);
                    }
                }
                if (CheckExpression(substrings[i]) > 0)
                {
                    if (CheckExpression(substrings[i]) == 6)
                    {
                        while (OperStack.Count > 0 && CheckExpression(OperStack.Peek()) != 5)
                        {
                            if ((CheckExpression(OperStack.Peek()) == 3 || CheckExpression(OperStack.Peek()) == 4))
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
                            if ((CheckExpression(OperStack.Peek()) == 1 || CheckExpression(OperStack.Peek()) == 2))
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
                        }
                       
                        OperStack.Pop();
                    }
                    else
                    {
                        OperStack.Push(substrings[i]);
                    }
                }
            }

            while (OperStack.Count > 0)
            {
                if (CheckExpression(OperStack.Peek()) == 1 || CheckExpression(OperStack.Peek()) == 2)
                {
                    int num1 = ValStack.Pop();
                    int num2 = ValStack.Pop();

                    if (OperStack.Count > 0 && CheckExpression(OperStack.Pop()) == 1)
                    {
                        ValStack.Push(num2 + num1);
                    }
                    else
                    {
                        ValStack.Push(num2 - num1);
                    }
                }
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
