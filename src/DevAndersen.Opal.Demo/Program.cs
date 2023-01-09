using DevAndersen.Opal.Demo;

bool keepGoing = true;

(string DisplayName, Action Action)[] options =
{
    ("Raw render demo", RawRenderDemo.Run),
    ("View demo", ViewDemo.Run),
    ("Drawing demo", DrawingDemo.Run),
    ("Exit", () => keepGoing = false)
};

do
{
    Console.Clear();
    for (int i = 0; i < options.Length; i++)
    {
        Console.WriteLine($"{i + 1} {options[i].DisplayName}");
    }
    Console.WriteLine();

    Console.Write("> ");
    bool inputIsValidNumber = int.TryParse(Console.ReadLine(), out int inputNumber);

    if (inputIsValidNumber && inputNumber > 0 && inputNumber <= options.Length)
    {
        options[inputNumber - 1].Action.Invoke();
    }
} while (keepGoing);

Console.WriteLine("Program exit.");
