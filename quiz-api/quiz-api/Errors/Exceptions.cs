public class BadRequestException : HttpException
{
    public BadRequestException(string message) : base(400, message) { }
}

public class NotFoundException : HttpException
{
    public NotFoundException(string message) : base(404, message) { }
}

public class UnauthorizedException : HttpException
{
    public UnauthorizedException(string message) : base(401, message) { }
}