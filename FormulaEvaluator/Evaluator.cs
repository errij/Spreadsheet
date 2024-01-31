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
                    if (OperStack.Count > 0 && (CheckExpression(OperStack.Peek()) == 3 || CheckExpression(OperStack.Peek()) == 4))
                    {
                        int temp = ValStack.Pop(); //pop the value in the stack

                        if (CheckExpression(OperStack.Pop()) == 3) //pop the operator and if it's *
                        {
                            ValStack.Push(temp * current); //multiply
                        }
                        else //if it's /
                        {
                            if(current == 0)
                            {
                                throw new ArgumentException("divided by zero");
                            }

                            ValStack.Push(temp / current); //division
                        }
                    }
                    else //if x is either + or -
                    {
                        ValStack.Push(current);
                    }
                }
                else if (CheckExpression(x) == 6) //if x is )
                {
                    while (OperStack.Count > 0 && CheckExpression(OperStack.Peek()) != 5) //while operator stack isn't empty and top operator isn't (
                    {
                        int num1 = ValStack.Pop(); //pop the top value
                        int num2 = ValStack.Pop(); //pop the second value

                        if (CheckExpression(OperStack.Pop()) == 1) //pop the operator and if top operator is +
                        {
                            ValStack.Push(num2 + num1); //plus
                        }
                        else //if top operator is -
                        {
                            ValStack.Push(num2 - num1); //minus
                        }
                    }
                    try
                    {
                        OperStack.Pop(); //when while loop is finished pop the operator //should be ( or throw exception

                    }
                    catch (InvalidOperationException)
                    {
                        throw new ArgumentException("OperStack is empty!");
                    }

                    if (OperStack.Count > 0 && (CheckExpression(OperStack.Peek()) == 3 || CheckExpression(OperStack.Peek()) == 4)) //if there is * or / in front of (
                    {
                        //do the process
                        int num1 = ValStack.Pop();
                        int num2 = ValStack.Pop();

                        if (CheckExpression(OperStack.Pop()) == 3) //multiply
                        {
                            ValStack.Push(num2 * num1);
                        }
                        else //division
                        {
                            if (num1 == 0)
                            {
                                throw new ArgumentException("divided by zero");
                            }

                            ValStack.Push(num2 / num1);
                        }
                    }
                }
                else if (CheckExpression(x) == 1 || CheckExpression(x) == 2) //if x is either + or -
                {
                    if(OperStack.Count != 0 && (CheckExpression(OperStack.Peek()) == 1 || CheckExpression(OperStack.Peek()) == 2)) //if top operator is + or -
                    {
                        if(ValStack.Count > 1)  //if there are more than one value in the stack
                        {
                            int num1 = ValStack.Pop();
                            int num2 = ValStack.Pop();

                            if(CheckExpression(OperStack.Pop()) == 1)
                            {
                                ValStack.Push(num2 + num1);
                            }
                            else
                            {
                                ValStack.Push(num2 - num1);
                            }
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

                        if (CheckExpression(OperStack.Peek()) == 3 || CheckExpression(OperStack.Peek()) == 4) //if remaining operator is other than + or -
                        {
                            throw new ArgumentException("There are more operators than numbers"); //throw exception
                        }

                        if (CheckExpression(OperStack.Pop()) == 1) //if +
                        {
                            ValStack.Push(num2 + num1);
                        }
                        else
                        {
                            ValStack.Push(num2 - num1);
                        }
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
        /// simply check expression type
        /// </summary>
        /// <param name="expression">expression to check</param>
        /// <returns>
        /// returns 0 if integer
        /// returns 1 if +
        /// returns 2 if -
        /// returns 3 if *
        /// returns 4 if /
        /// returns 5 if (
        /// returns 6 if )
        /// returns -1 if not match - could be a variable
        /// </returns>
        private static int CheckExpression(string expression)
        {
            if (int.TryParse(expression, out _)) return 0; //if parsalbe 

            return expression switch
            {
                "+" => 1,
                "-" => 2,
                "*" => 3,
                "/" => 4,
                "(" => 5,
                ")" => 6,
                _ => -1
            };
        }

    }

}