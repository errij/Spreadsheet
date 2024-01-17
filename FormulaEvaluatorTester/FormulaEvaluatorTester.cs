// See https://aka.ms/new-console-template for more information
using FormulaEvaluator;


if (Evaluator.Evaluate("5+5", null) == 10)
{
    Console.WriteLine("Happy Day!");
}

Console.WriteLine(Evaluator.Evaluate("5*5", null));


