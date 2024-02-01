using System.Numerics;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    public class Evaluator
    {
        public delegate int Lookup(String var);

        /// <summary>
        /// Calculates an expression and evaluate number
        /// using operateors and values from string expression
        /// </summary>
        /// <param name="expression">string of combination of values and operators</param>
        /// <param name="variableEvaluator">delegate to lookup the variables</param>
        /// <returns>evalutated integer</returns>
        /// <exception cref="Exception"></exception>
        public static int Evaluate(String expression, Lookup variableEvaluator)
        {
            expression = expression.Replace(" ", ""); //removes whitespaces
            String[] substrings = Regex.Split(expression, @"(?<=[\(\)\-\+\*/])|(?=[\(\)\-\+\*/])").Where(s => !string.IsNullOrEmpty(s)).ToArray(); //slipt string

            var OperStack = new Stack<String>(); // Operation stack
            var ValStack = new Stack<int>();    // Value stack

            foreach (string x in substrings)
            {
                if (CheckExpression(x) == 0 || CheckExpression(x) == -1) //if x is an integer or variable
                {
                    int current;

                    if (CheckExpression(x) == -1 && variableEvaluator != null) //if x is a variable and delegate is not null
                    {
                        Char[] check = x.ToCharArray();
                        int count = 0;

                        foreach (Char character in check) //check the valicity of variable 
                        {
                            if (!(int.TryParse(character.ToString(), out _)))
                            {
                                count++;
                            }
                            if (count == 2) //if variable contains more than one character
                            {
                                throw new ArgumentException($"Variable {x} is not valid!");
                            }
                        }

                        try
                        {
                            current = variableEvaluator(x);
                        }
                        catch
                        {
                            throw new ArgumentException($"Variable {x} was not defined!");
                        }
                    }
                    else //if x is an integer
                    {
                        current = int.Parse(x); //parse

                    }
                    //if operator stack isn't empty and top operator is either * or /
                    if (OperStack.Count > 0 && (CheckExpression(OperStack.Peek()) == 2))
                    {
                        int temp = ValStack.Pop(); //pop the value in the stack

                        ValStack.Push(DoMath(temp, current, OperStack.Pop()));
                    }
                    else //if x is either + or -
                    {
                        ValStack.Push(current);
                    }
                }
                else if (CheckExpression(x) == 4) //if x is )
                {
                    while (OperStack.Count > 0 && CheckExpression(OperStack.Peek()) != 3) //while operator stack isn't empty and top operator isn't (
                    {
                        int num1 = ValStack.Pop(); //pop the top value
                        int num2 = ValStack.Pop(); //pop the second value

                        ValStack.Push(DoMath(num2, num1, OperStack.Pop()));
                    }

                    try
                    {
                        OperStack.Pop(); //when while loop is finished pop the operator //should be ( or throw exception

                    }
                    catch (InvalidOperationException)
                    {
                        throw new ArgumentException("OperStack is empty!");
                    }

                    if (OperStack.Count > 0 && (CheckExpression(OperStack.Peek()) == 2)) //if there is * or / in front of (
                    {
                        //do the process
                        int num1 = ValStack.Pop();
                        int num2 = ValStack.Pop();

                        ValStack.Push(DoMath(num2, num1, OperStack.Pop()));
                    }
                }
                else if (CheckExpression(x) == 1) //if x is either + or -
                {
                    if(OperStack.Count != 0 && (CheckExpression(OperStack.Peek()) == 1)) //if top operator is + or -
                    {
                        if(ValStack.Count > 1)  //if there are more than one value in the stack
                        {
                            int num1 = ValStack.Pop();
                            int num2 = ValStack.Pop();

                            ValStack.Push(DoMath(num2, num1, OperStack.Pop()));
                        }
                    }
                    OperStack.Push(x);
                }
                else //if x is * or / 
                {
                    OperStack.Push(x);//push to the stack
                }
            }
            
                while (OperStack.Count > 0) //deal with the remaining operators //should be + or - 
                {
                    try
                    {
                        int num1 = ValStack.Pop();
                        int num2 = ValStack.Pop();

                        if (CheckExpression(OperStack.Peek()) == 2) //if remaining operator is other than + or -
                        {
                            throw new ArgumentException("There are more operators than numbers"); //throw exception
                        }

                    ValStack.Push(DoMath(num2, num1, OperStack.Pop()));
                        
                    }
                    catch(InvalidOperationException)
                    {
                        throw new ArgumentException("ValStack is empty!");
                    }
                }


            if (ValStack.Count > 1) //if there are more than one variable
            {
                throw new ArgumentException("There are more numbers than operators");//throw exception
            }

            if (ValStack.Count == 0)
            {
                throw new ArgumentException("Stack is empty");
            }

            return ValStack.Pop(); //return value
        }

        /// <summary>
        /// Check expression type
        /// </summary>
        /// <param name="expression">expression to check</param>
        /// <returns>
        /// 1 if an expression is either + or -
        /// 2 if an expression is either * or /
        /// 3 if an expression is (
        /// 4 if an expression is )
        /// </returns>
        private static int CheckExpression(string expression)
        {
            if (double.TryParse(expression, out _)) return 0; //if double

            return expression switch
            {
                "+" or "-" => 1,
                "*" or "/" => 2,
                "(" => 3,
                ")" => 4,
                _ => -1
            };
        }

        /// <summary>
        /// This method does hard math for us! 
        /// This method is better than us right?
        /// </summary>
        /// <param name="x">the first value</param>
        /// <param name="y">the second value</param>
        /// <param name="expression">expression</param>
        /// <returns>Evaluated value</returns>
        /// <exception cref="ArgumentException">If no-no value catched (like divided by zero)</exception>
        private static int DoMath(int x, int y, string expression)
        {
            if (expression.Equals("+"))
            {
                return x + y;
            }
            else if (expression.Equals("-"))
            {
                return x-y;
            }
            else if (expression.Equals("*"))
            {
                return x*y;
            }
            else if (expression.Equals("/"))
            {
                if (y == 0) throw new ArgumentException("Divided by zero is no-no");
                return x/y;
            }
            else
            {
                throw new ArgumentException($"{expression} is a no-no expression!");
            }
        }
    }

}
