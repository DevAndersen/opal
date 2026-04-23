namespace Opal.Events;

public interface ICancellationRequestHandler
{
    bool PreventCancellationRequest();
}
