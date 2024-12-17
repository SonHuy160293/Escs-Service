namespace ESCS.Domain.Dtos
{
    internal class RoleDTO
    {
    }

    public class RoleGetDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
    }
}
