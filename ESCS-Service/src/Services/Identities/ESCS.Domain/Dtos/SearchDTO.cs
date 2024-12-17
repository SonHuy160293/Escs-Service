namespace ESCS.Domain.Dtos
{
    public class UserSearchRequest : Core.Domain.Base.BaseSearchRequest
    {
        public string? UserName { get; set; } = default!;

    }


}
