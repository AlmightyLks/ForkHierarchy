namespace ForkHierarchy.Core.Options;
public class SecretsOptions
{
    public const string Secrets = nameof(Secrets);

    public string? GitHubPAT { get; set; }
}
