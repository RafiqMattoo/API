using System.ComponentModel.DataAnnotations;

namespace API.DTO

{
    public class RegisterDTO
    {
        [Required(ErrorMessage="Username is Required")]
        public string Username { get; set; }
        
        [Required(ErrorMessage="Password is Required")]
        public string Password { get; set; }
    }
}