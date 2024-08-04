using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BIZNEWS_FREE.Models
{
    public class User : IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string? PhotoUrl { get; set; }

        [NotMapped] //propertinin sqle dusmeye icaze vermir
        public override string? PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }
    }
}
