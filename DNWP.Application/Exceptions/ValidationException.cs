namespace DNWP.Application.Exceptions;

public class ValidationException(string property) : Exception($"{property} already exists");
