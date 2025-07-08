using Microsoft.AspNetCore.Authentication.OAuth;

namespace ECommerce.API.Services.Interface
{
    public interface IGoogleAuthServices
    {
        Task HandleGoogleCallbackAsync(OAuthCreatingTicketContext context);
    }
}
