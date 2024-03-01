namespace WebApplication2.Exceptions;

public class SecurityException : Exception
{
    public SecurityException(string message) : base(message)
    {
    }
}