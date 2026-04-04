namespace Shared
{
    public record GeneralResponse(bool Success = false, string Message = null!); // record type no need for body { }
}
