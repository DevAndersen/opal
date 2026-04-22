using Opal.ConsoleHandlers.InputHandlers;
using Opal.Events;
using Opal.Rendering;

namespace Opal.ConsoleHandlers;

/// <summary>
/// A console handler for UNIX-like systems.
/// </summary>
public class UnixConsoleHandler : CommonConsoleHandler
{
    protected UnixInputHandler InputHandler { get; }

    public UnixConsoleHandler()
    {
        InputHandler = new UnixInputHandler(this);
    }

    public override void Start(OpalSettings settings)
    {
        base.Start(settings);
        if (Running)
        {
            throw new InvalidOperationException();
        }

        Running = true;
        Settings = settings;

        if (!ConsoleSizeThread.IsAlive)
        {
            ConsoleSizeThread.Start();
        }

        (Width, Height) = GetClampedConsoleSize(settings);

        if (settings.UseAlternateBuffer)
        {
            Print(SequenceProvider.EnableAlternateBuffer());
        }

        Console.CursorVisible = false;
        InputHandler.StartInputListening();
    }

    public override void Stop()
    {
        InputHandler.StopInputListening();
        Console.CursorVisible = true;

        if (Settings?.UseAlternateBuffer == true)
        {
            Print(SequenceProvider.DisableAlternateBuffer());
        }
        else
        {
            Print("\n");
        }

        Print(SequenceProvider.Reset());

        Running = false;
    }

    public override IConsoleInput? GetInput()
    {
        return InputHandler.GetInput();
    }

    public override void Print(string str)
    {
        Console.Write(str);
    }

    /// <remarks>
    /// <c>Console.Out.Write</c> is used because its implementation iterates over the chunks of the <c>StringBuilder</c>,  which avoids allocating a temporary string.
    /// This is not the case for <c>Console.Out.Write</c>, which simply calls the <c>ToString</c> method on the passed object.
    /// </remarks>
    /// <param name="stringBuilder"></param>
    public override void Print(StringBuilder stringBuilder)
    {
        Console.Out.Write(stringBuilder);
    }
}
