using System.Numerics;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    public class Evaluator
    {
        public delegate int Lookup(String var);

        public static int Evaluate(String expression, Lookup variableEvaluator)
        {
            string[] substrings = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            var OperStack = new Stack<String>(); //Operation stack
            var ValStack = new Stack<int>(); //Value stack

            for (int i = 0; i < substrings.Length; i++)
            {
                if (CheckExpression(substrings[i]) == 0)
                {
                    if (CheckExpression(OperStack.Peek()) == 2)
                    {

                    }
                    ValStack.Push(Int32.Parse(substrings[i]));
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
            else if (expression.Equals("+") || expression.Equals("-"))
            {
                return 1;
            }
            else if (expression.Equals("/") || expression.Equals("*"))
            {
                return 2;
            }
            else if (expression.Equals("("))
            {
                return 3;
            }
            else if (expression.Equals(")"))
            {
                return 4;
            }
            else
            {
                return -1;
            }
        }
    }

}
