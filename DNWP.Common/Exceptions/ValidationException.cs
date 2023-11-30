namespace DNWP.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(string property) : base($"{property} already exists")
    {
    }
}
