namespace Shared // Namespace change : UI model such as Request and Response should not belong to Shared namespace.
                 // They should only be visble in the UI layer and used in Controllers for example; not the be shared across the solution 
{
    public record LoginResponse
        (bool Flag, string Message = null!, string Token = null!, string RefreshToken = null!);
}
