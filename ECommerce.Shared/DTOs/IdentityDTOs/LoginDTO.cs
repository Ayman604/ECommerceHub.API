using System.ComponentModel.DataAnnotations;

namespace ECommerce.Shared.DTOs.IdentityDTOs
{
    public record LoginDTO([EmailAddress] string Email, string Password);

}
