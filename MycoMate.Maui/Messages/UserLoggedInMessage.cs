namespace MycoMate.Maui.Messages;

public class UserLoggedInMessage(string eMail)
{
    public string EMail { get; } = eMail;
}