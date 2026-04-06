namespace BackEnd.API.Helpers
{
    public class JwtSection // What is the purpouse of JwtSection as a Helper ?
    {
        public string? Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
    }
}
