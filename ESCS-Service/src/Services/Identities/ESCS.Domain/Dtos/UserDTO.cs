namespace ESCS.Domain.Dtos
{
    internal class UserDTO
    {
    }

    public class UserGetDto
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;

        public string? PhoneNumber { get; set; } = default!;

        public long RoleId { get; set; }

        public RoleGetDto Role { get; set; }
    }

    public class UserInServiceGetDto
    {
        public long UserId { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;

        public string? PhoneNumber { get; set; } = default!;


    }

    public class UserByServiceGetDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;

    }
}
