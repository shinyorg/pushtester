namespace PushEndToEnd.Services;

public interface IPushSender
{
    Task Send(string token, bool silent);
}