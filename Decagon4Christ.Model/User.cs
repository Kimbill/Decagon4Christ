using Decagon4Christ.Model.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Decagon4Christ.Model
{
    public class User : IdentityUser
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string TechStack { get; set; }
        public Squad Squad { get; set; }
        public string PlaceOfWork { get; set; }
        public string Role { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
    }
}