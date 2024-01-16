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
                if (CheckExpression(substrings[i]))
                {

                }
            }

            return ValStack.Pop();
        }

        private static Boolean CheckExpression(String expression)
        {
            if (expression is int)
            {
                return true;
            }

            return false;
        }
    }

}
