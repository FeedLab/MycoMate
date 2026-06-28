namespace MycoMate.Maui.Services;

public class ServiceException(string message, Exception? innerException = null)
    : Exception(message, innerException);
