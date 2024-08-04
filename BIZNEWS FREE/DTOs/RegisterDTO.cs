using System.ComponentModel.DataAnnotations;

namespace BIZNEWS_FREE.DTOs
{
    public class RegisterDTO
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [Compare("Password")] //muqaise etmek ucun eger bereberdise buraxsin
        public string ConfirmPassword { get; set; }

    }
}
