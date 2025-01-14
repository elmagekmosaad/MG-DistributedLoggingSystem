namespace MGDistributedLoggingSystem.Configurations
{
    public class JwtConfig
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int DurationInDays { get; set; }
        public string Key { get; set; } = string.Empty;
    }
}
