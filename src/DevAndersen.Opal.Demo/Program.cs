using DevAndersen.Opal.Demo;

Action[] actions = new Action[]
{
    RawRenderDemo.Run,
    ViewDemo.Run,
    LoadingDemo.Run,
    LinesTestDemo.Run,
};

bool hasValidChoiceBeenMade = false;

do
{
    Console.Clear();
    PrintActions(actions);

    Console.Write("> ");
    string? input = Console.ReadLine();
    bool inputIsValidNumber = int.TryParse(input, out int inputNumber);

    if (inputIsValidNumber && inputNumber > 0 && inputNumber <= actions.Length)
    {
        hasValidChoiceBeenMade = true;
        actions[inputNumber - 1].Invoke();
    }
} while (!hasValidChoiceBeenMade);

Console.WriteLine("Program exit.");
Console.ReadLine();

static void PrintActions(Action[] actions)
{
    for (int i = 0; i < actions.Length; i++)
    {
        Action action = actions[i];
        string act = action.Method.DeclaringType?.Name ?? "[UNKNOWN]";
        Console.WriteLine($"{i + 1} {act}");
    }
    Console.WriteLine();
}
