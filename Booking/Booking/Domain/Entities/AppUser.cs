
namespace Booking.Domain.Entities
{
    public class AppUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public required string Email { get; set; }
        public required string Password { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        public required string Username { get; set; }
        public required string Address { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
	}
}
