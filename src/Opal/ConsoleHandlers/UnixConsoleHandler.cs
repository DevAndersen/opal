using Opal.ConsoleHandlers.InputHandlers;
using Opal.Rendering;
using System.Diagnostics.CodeAnalysis;

namespace Opal.ConsoleHandlers;

/// <summary>
/// A console handler for UNIX-like systems.
/// </summary>
public class UnixConsoleHandler : CommonConsoleHandler<UnixInputHandler>
{
    [SetsRequiredMembers]
    public UnixConsoleHandler()
    {
        InputHandler = new UnixInputHandler(this);
    }

    public override void Start(OpalSettings settings)
    {
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

    public override void Print(string str)
    {
        Console.Write(str);
    }

    public override void Print(StringBuilder stringBuilder)
    {
        Console.Out.Write(stringBuilder);
    }
}
