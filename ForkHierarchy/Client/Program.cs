using ForkHierarchy.Client;
using ForkHierarchy.Client.Api;
using ForkHierarchy.Core.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Refit;

internal class Program
{
    private static async global::System.Threading.Tasks.Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        builder.Services.AddMudServices();

        builder.Services.AddScoped<GitHubHierarchyService>();
        builder.Services.AddScoped<HierarchyViewModel>();
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddRefitClient<IGitHubRepository>();
        builder.Services.AddRefitClient<IQueuedRepositories>();
        //.ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.github.com"));
        builder.Services.AddScoped<ForkHierarchyApiClient>();

        await builder.Build().RunAsync();
    }
}
