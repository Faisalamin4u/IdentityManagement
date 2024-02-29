using System.ComponentModel.DataAnnotations;

namespace IdentityManagement.API.Model.Requests
{
    public class RegisterRequest
    {
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }
        [Required,Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
