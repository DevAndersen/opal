using Opal.Demo;

Func<Task>[] actions = new Func<Task>[]
{
    RawRenderDemo.RunAsync,
    ViewDemo.RunAsync,
    LoadingDemo.RunAsync,
    LinesTestDemo.RunAsync,
    StringCacheDemo.RunAsync,
    DrawingDemo.RunAsync,
    MouseInputDemo.RunAsync,
    MatrixDemo.RunAsync
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
        await actions[inputNumber - 1].Invoke();
    }
} while (!hasValidChoiceBeenMade);

Console.WriteLine("Program exit.");
Console.ReadLine();

static void PrintActions(Func<Task>[] actions)
{
    for (int i = 0; i < actions.Length; i++)
    {
        Func<Task> action = actions[i];
        string act = action.Method.DeclaringType?.Name ?? "[UNKNOWN]";
        Console.WriteLine($"{i + 1} {act}");
    }
    Console.WriteLine();
}
