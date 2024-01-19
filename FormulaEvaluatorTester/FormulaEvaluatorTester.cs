// See https://aka.ms/new-console-template for more information
using FormulaEvaluator;


if (Evaluator.Evaluate("5+5", null) == 10)
{
    Console.WriteLine("Happy Day!");
}

Console.WriteLine(Evaluator.Evaluate("5*(30 - 25)", null));

int VariableEvaluator(string varName)
{
    // For simplicity, assume variables are represented as integers
    // In a real-world scenario, you might fetch the variable value from a database or other source
    if (varName.Equals("a", StringComparison.OrdinalIgnoreCase))
    {
        return 5; // Value of variable 'a'
    }
    else if (varName.Equals("b", StringComparison.OrdinalIgnoreCase))
    {
        return 10; // Value of variable 'b'
    }
    else
    {
        throw new ArgumentException($"Unknown variable: {varName}");
    }
}

// Example expression containing variables
string expression = "a + (3 - 2)";

try
{
    // Call Evaluate with the expression and the variable evaluator
    int result = Evaluator.Evaluate(expression, a => 1);

    Console.WriteLine($"Result: {result}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}