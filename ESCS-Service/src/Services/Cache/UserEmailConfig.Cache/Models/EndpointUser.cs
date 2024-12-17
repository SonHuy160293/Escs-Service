namespace Identity.Cache.Models
{
    public class EndpointUser
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public IEnumerable<User> Users { get; set; }
    }

    public class User
    {
        public long Id { get; set; }
        public string Email { get; set; }
    }
}
