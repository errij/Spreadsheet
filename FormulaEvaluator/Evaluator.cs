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
                    HandleNumericExpression(substrings[i], OperStack, ValStack);
                }

                if (CheckExpression(substrings[i]) > 0)
                {
                    if (CheckExpression(substrings[i]) == 6)
                    {
                        HandleClosingParenthesis(OperStack, ValStack);
                    }
                    OperStack.Push(substrings[i]);
                }
            }

            HandleRemainingOperations(OperStack, ValStack);

            return ValStack.Pop();
        }

        private static void HandleNumericExpression(string substring, Stack<string> OperStack, Stack<int> ValStack)
        {
            int current = int.Parse(substring);

            if (CheckExpression(OperStack.Peek()) == 3 || CheckExpression(OperStack.Peek()) == 4)
            {
                int temp = ValStack.Pop();
                HandleBinaryOperation(temp, current, OperStack, ValStack);
            }
            else
            {
                ValStack.Push(current);
            }
        }

        private static void HandleBinaryOperation(int num1, int num2, Stack<string> OperStack, Stack<int> ValStack)
        {
            if (CheckExpression(OperStack.Pop()) == 3)
            {
                ValStack.Push(num2 * num1);
            }
            else
            {
                ValStack.Push(num2 / num1);
            }
        }

        private static void HandleClosingParenthesis(Stack<string> OperStack, Stack<int> ValStack)
        {
            while (CheckExpression(OperStack.Peek()) != 5)
            {
                if (CheckExpression(OperStack.Peek()) == 3 || CheckExpression(OperStack.Peek()) == 4)
                {
                    HandleBinaryOperation(ValStack.Pop(), ValStack.Pop(), OperStack, ValStack);
                }
                if (CheckExpression(OperStack.Peek()) == 1 || CheckExpression(OperStack.Peek()) == 2)
                {
                    HandleBinaryOperation(ValStack.Pop(), ValStack.Pop(), OperStack, ValStack);
                }
            }
            OperStack.Pop();
        }

        private static void HandleRemainingOperations(Stack<string> OperStack, Stack<int> ValStack)
        {
            while (OperStack.Count > 0)
            {
                if (CheckExpression(OperStack.Peek()) == 1 || CheckExpression(OperStack.Peek()) == 2)
                {
                    HandleBinaryOperation(ValStack.Pop(), ValStack.Pop(), OperStack, ValStack);
                }
            }
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
