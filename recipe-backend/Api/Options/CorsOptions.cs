namespace Api.Options;

public sealed class CorsOptions
{
    public const string SectionName = "Cors";
    public const string PolicyName = "ConfiguredCors";

    public string[] AllowedOrigins { get; set; } = [];
}
