using ForkHierarchy.Services;
using Octokit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<GitHubClient>(x =>
{
    string? pat = builder.Configuration?.GetValue<string>("Secrets:GitHubPAT");

    var client = new GitHubClient(new ProductHeaderValue("AlmightyLks"));
    client.Credentials = new Credentials(pat);
    return client;
});
builder.Services.AddScoped<HierachyBuilder>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();