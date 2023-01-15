namespace ForkHierarchy.Server.Helper;

public static class HttpClientExtensions
{
    public static async Task<long> GetCommitCountAsync(this HttpClient client, string owner, string repo)
    {
        var response = await client.GetAsync($"https://api.github.com/repos/{owner}/{repo}/commits?per_page=1");
        var link = response.Headers.GetValues("link");
        return 1;
    }
}
