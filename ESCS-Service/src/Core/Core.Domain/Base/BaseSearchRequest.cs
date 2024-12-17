namespace Core.Domain.Base
{
    public class BaseSearchRequest
    {

        public int? PageIndex { get; set; } = 0;
        public int? PageSize { get; set; } = 1;
    }
}
