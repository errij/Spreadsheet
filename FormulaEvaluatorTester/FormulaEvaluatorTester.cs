// See https://aka.ms/new-console-template for more information
using FormulaEvaluator;
using SpreadsheetUtilities;
using System;
using System.Linq.Expressions;
int VariableEvaluator(string varName)
{
    if (varName.Equals("a", StringComparison.OrdinalIgnoreCase))
    {
        return 5;
    }
    else if (varName.Equals("b", StringComparison.OrdinalIgnoreCase))
    {
        return 10;
    }
    else
    {
        throw new ArgumentException($"Unknown variable: {varName}");
    }
}

if (Evaluator.Evaluate("5+5", null) == 10)
{
    Console.WriteLine("Test 1 pass");
}

if (Evaluator.Evaluate("5-5", null) == 0)
{
    Console.WriteLine("Test 2 pass");
}

if (Evaluator.Evaluate("5*5", null) == 25)
{
    Console.WriteLine("Test 3 pass");
}

if (Evaluator.Evaluate("5/5", null) == 1)
{
    Console.WriteLine("Test 4 pass");
}

if (Evaluator.Evaluate("5*(30 - 25)", null) == 25)
{
    Console.WriteLine("Test 5 pass");
}

if (Evaluator.Evaluate("5+(30 - 25)", null) == 10)
{
    Console.WriteLine("Test 6 pass");
}

if (Evaluator.Evaluate("5-(30 - 25)", null) == 0)
{
    Console.WriteLine("Test 7 pass");
}

if (Evaluator.Evaluate("5/(30 - 25)", null) == 1)
{
    Console.WriteLine("Test 8 pass");
}

if (Evaluator.Evaluate("a + b", VariableEvaluator) == 15)
{
    Console.WriteLine("Test 9 pass");
}

if (Evaluator.Evaluate("a * b", VariableEvaluator) == 50)
{
    Console.WriteLine("Test 10 pass");
}

try
{
    Evaluator.Evaluate("5/0", null);
}
catch (Exception ex)
{
    Console.WriteLine($"Error test 1: {ex.Message}");
}

try
{
    Evaluator.Evaluate("5+", null);
}
catch (Exception ex)
{
    Console.WriteLine($"Error test 2: {ex.Message}");
}

try
{
    Evaluator.Evaluate("", null);
}
catch (Exception ex)
{
    Console.WriteLine($"Error test 3: {ex.Message}");
}

try
{
    Evaluator.Evaluate("a + 3", null);
}
catch (Exception ex)
{
    Console.WriteLine($"Error test 4: {ex.Message}");
}

try
{
    Evaluator.Evaluate("3 / a", a => 0);
}
catch (Exception ex)
{
    Console.WriteLine($"Error test 5: {ex.Message}");
}

try
{
    Evaluator.Evaluate("c", VariableEvaluator);
}
catch (Exception ex)
{
    Console.WriteLine($"Error test 6: {ex.Message}");
}

try
{
    Evaluator.Evaluate("5 + 6)", null);
}
catch (Exception ex)
{
    Console.WriteLine($"Error test 7: {ex.Message}");
}

try
{
    Evaluator.Evaluate("5 5 5)", null);
}
catch (Exception ex)
{
    Console.WriteLine($"Error test 8: {ex.Message}");
}

try
{
    Evaluator.Evaluate("+-//)", null);
}
catch (Exception ex)
{
    Console.WriteLine($"Error test 9: {ex.Message}");
}
