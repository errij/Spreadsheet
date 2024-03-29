﻿// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private List<string> formulaContainer; //an array container to store formula


        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            formulaContainer = new List<string>();  
            IEnumerable<string> temp = GetTokens(formula);

            if (formula.Equals("")) throw new FormulaFormatException("Formula is empty!");
            foreach (var item in temp)
            {
                if (CheckExpression(item) == -1)
                {
                    if (!isValid(item)) throw new FormulaFormatException($"not a valid item: {item}"); //isValid delegate

                    try
                    {
                        string normalString = normalize(item);
                        formulaContainer.Add(normalString);
                    }
                    catch //catch an error thrown by normalize delegate
                    {
                        throw new FormulaFormatException($"not a item to normalize: {item}");
                    }
                }
                else
                {
                    formulaContainer.Add(item);
                }
            }
            checkFormula(formulaContainer);
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            var OperStack = new Stack<String>(); // Operation stack
            var ValStack = new Stack<double>();    // Value stack

            foreach (string item in formulaContainer)
            {
                if (CheckExpression(item) == 0 || CheckExpression(item) == -1) //if x is an integer or variable
                {
                    double current;

                    if (CheckExpression(item) == -1) //if x is a variable
                    {
                        try
                        {
                            current = lookup(item);
                        }
                        catch
                        {
                            return new FormulaError($"variable {item} is not valid!");
                        }
                    }
                    else //if x is an integer
                    {
                        current = double.Parse(item); //parse
                    }
                    //if operator stack isn't empty and top operator is either * or /
                    if (OperStack.Count > 0 && (CheckExpression(OperStack.Peek()) == 2))
                    {
                        double temp = ValStack.Pop(); //pop the value in the stack

                        try
                        {
                            ValStack.Push(DoMath(temp, current, OperStack.Pop()));
                        }
                        catch
                        {
                            return new FormulaError($"Divided by {current} is not valid!");
                        }
                    }
                    else 
                    {
                        ValStack.Push(current);
                    }
                }
                else if (CheckExpression(item) == 4) //if x is )
                {
                    while (OperStack.Count > 0 && CheckExpression(OperStack.Peek()) != 3) //while operator stack isn't empty and top operator isn't (
                    {
                        double num1 = ValStack.Pop(); //pop the top value
                        double num2 = ValStack.Pop(); //pop the second value

                        ValStack.Push(DoMath(num2, num1, OperStack.Pop()));
                    }
                    try
                    {
                        OperStack.Pop(); //when while loop is finished pop the operator //should be ( or throw exception

                    }
                    catch (InvalidOperationException)
                    {
                        return new FormulaError("Operation stack is empty! Check formula is valid!");
                    }

                    if (OperStack.Count > 0 && (CheckExpression(OperStack.Peek()) == 2)) //if there is * or / in front of (
                    {
                        //do the process
                        double back = ValStack.Pop();
                        double front = ValStack.Pop();

                        try
                        {
                            ValStack.Push(DoMath(front, back, OperStack.Pop()));
                        }
                        catch
                        {
                            return new FormulaError($"Divided by {back} is not valid!");
                        }
                    }
                }
                else if (CheckExpression(item) == 1) //if x is either + or -
                {
                    if (OperStack.Count != 0 && (CheckExpression(OperStack.Peek()) == 1)) //if top operator is + or -
                    {
                        if (ValStack.Count > 1)  //if there are more than one value in the stack
                        {
                            double back = ValStack.Pop();
                            double front = ValStack.Pop();

                            ValStack.Push(DoMath(front, back, OperStack.Pop()));
                        }
                    }
                    OperStack.Push(item);
                }
                else //if x is * or / 
                {
                    OperStack.Push(item);//push to the stack
                }
            }

            while (OperStack.Count > 0) //deal with the remaining operators //should be + or - 
            {
                try
                {
                    double back = ValStack.Pop();
                    double front = ValStack.Pop();

                    if (CheckExpression(OperStack.Peek()) == 2) //if remaining operator is other than + or -
                    {
                        return new FormulaError("There are more expression than other variables! Check formula is valid!");  
                    }

                    ValStack.Push(DoMath(front, back, OperStack.Pop()));
                }
                catch
                {
                    return new FormulaError("ValStack is empty! Check formula is valid!");
                }
            }


            if (ValStack.Count > 1) //if there are more than one variable
            {
                return new FormulaError("There are more numbers than operators");//throw exception
            }

            if (ValStack.Count == 0)
            {
                return new FormulaError("Stack is empty");
            }

            return ValStack.Pop(); //return value
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            HashSet<string> temp = new HashSet<string> ();

            foreach(var item in formulaContainer)
            {
                if (CheckExpression(item) == -1)
                {
                    temp.Add(item);
                }
            }

            return temp;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            List<string> temp = new List<string>();

            foreach(var item in formulaContainer)
            {
                if(CheckExpression(item) == 0)
                {
                    temp.Add(double.Parse(item).ToString()); //treats notation such as e
                }
                else
                {
                    temp.Add(item);
                }
            }

            return string.Join("", temp);
        }

        /// <summary>
        ///  <change> make object nullable </change>
        ///
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object? obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }

        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// 
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            return f1.Equals(f2);
        }

        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        ///   <change> Note: != should almost always be not ==, if you get my meaning </change>
        ///   Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !f1.Equals(f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

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
        private static double DoMath(double x, double y, string expression)
        {
            if (expression.Equals("+"))
            {
                return (double)(x + y);
            }
            else if (expression.Equals("-"))
            {
                return (double)(x - y);
            }
            else if (expression.Equals("*"))
            {
                return (double)(x * y);
            }
            else if (expression.Equals("/")) 
            {
                if (y == 0) throw new ArgumentException("Divided by zero is no-no");
                return (double)(x / y); 
            }
            else
            {
                throw new ArgumentException($"{expression} is a no-no expression!");
            }
        }

        private static void checkFormula(List<string> formula)
        {
            string temp = ""; //temporary string to check
            int leftParCheck = 0;
            int rightParCheck = 0;
            if(CheckExpression(formula[formula.Count - 1]) == 1 || CheckExpression(formula[formula.Count - 1]) == 2) throw new FormulaFormatException("there is an extra operator!");
            foreach (string s in formula)
            {
                if (CheckExpression(s) == -1)
                {
                    if (CheckExpression(temp) == 0) throw new FormulaFormatException($"Not a valid variable: {temp}{s}");
                }
                else if (CheckExpression(s) == 0)
                {
                    if (CheckExpression(temp) == 0 || CheckExpression(temp) == 4) throw new FormulaFormatException($"Not a valid format: {temp} and {s}");
                }
                else if (CheckExpression(s) == 1 || CheckExpression(s) == 2)
                {
                    if (CheckExpression(temp) == 1 || CheckExpression(temp) == 2) throw new FormulaFormatException($"Not a valid format: {temp} and {s}");
                }
                else if (CheckExpression(s) == 3)
                {
                    leftParCheck++;
                    if(CheckExpression(temp) == 0 || CheckExpression(temp) == 4) throw new FormulaFormatException($"Not a valid format: {temp} and {s}");
                }
                else if (CheckExpression(s) == 4)
                {
                    rightParCheck++;
                    if(CheckExpression(temp) == 1 || CheckExpression(temp) == 2) throw new FormulaFormatException($"Not a valid format: {temp} and {s}");
                }

                temp = s;
            }
            if (leftParCheck != rightParCheck) throw new FormulaFormatException("Check parenthesis!");
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}


// <change>
//   If you are using Extension methods to deal with common stack operations (e.g., checking for
//   an empty stack before peeking) you will find that the Non-Nullable checking is "biting" you.
//
//   To fix this, you have to use a little special syntax like the following:
//
//       public static bool OnTop<T>(this Stack<T> stack, T element1, T element2) where T : notnull
//
//   Notice that the "where T : notnull" tells the compiler that the Stack can contain any object
//   as long as it doesn't allow nulls!
// </change>
