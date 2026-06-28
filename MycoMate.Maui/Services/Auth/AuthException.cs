namespace MycoMate.Maui.Services.Auth;

public class AuthException(string message, Exception? innerException = null)
    : Exception(message, innerException);
