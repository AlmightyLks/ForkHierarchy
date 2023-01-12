using Database;
using ForkHierarchy.Core.Jobs;
using ForkHierarchy.Core.Options;
using ForkHierarchy.Core.Services;
using ForkHierarchy.Core.Sinks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MudBlazor;
using MudBlazor.Services;
using Octokit;
using Quartz;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<SecretsOptions>(builder.Configuration.GetSection(SecretsOptions.Secrets));
builder.Services.Configure<RateLimitOptions>(builder.Configuration.GetSection(RateLimitOptions.RateLimit));
builder.Services.Configure<GitHubOptions>(builder.Configuration.GetSection(GitHubOptions.GitHub));
builder.Services.Configure<DiscordOptions>(builder.Configuration.GetSection(DiscordOptions.Discord));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContext<ForkHierarchyContext>(x =>
{
    x.UseSqlite(builder.Configuration.GetValue<string>("ConnectionStrings:Default"));
    x.EnableSensitiveDataLogging(true);
});
builder.Services.AddScoped<GitHubHierarchyService>();
builder.Services.AddScoped<GitHubClient>(x =>
{
    string? pat = builder.Configuration?.GetValue<string>("Secrets:GitHubPAT");

    var client = new GitHubClient(new ProductHeaderValue("AlmightyLks"));
    if (!String.IsNullOrWhiteSpace(pat))
        client.Credentials = new Credentials(pat);
    return client;
});

builder.Services.AddQuartz(q =>
{
    q.UseInMemoryStore();

    // base Quartz scheduler, job and trigger configuration
    q.UseMicrosoftDependencyInjectionJobFactory();

    q.AddJob<ProcessQueuedRepositoriesJob>(opts => opts.WithIdentity("ProcessQueuedRepositoriesJob"));
    q.AddTrigger(opts =>
        opts.ForJob("ProcessQueuedRepositoriesJob")
            .WithIdentity("ProcessQueuedRepositoriesJob-Trigger")
            .WithSimpleSchedule(x =>
                x.WithInterval(TimeSpan.FromMinutes(5))
                 .RepeatForever()
                 )
            );
});

// ASP.NET Core hosting
builder.Services.AddQuartzServer(options =>
{
    // when shutting down we want jobs to complete gracefully
    options.WaitForJobsToComplete = true;
});
builder.Host.UseSerilog();

var app = builder.Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Discord(app.Services.GetService<IOptions<DiscordOptions>>()!)
    .CreateLogger();

using (var scope = app.Services.CreateScope())
{
    using var db = scope.ServiceProvider.GetRequiredService<ForkHierarchyContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();