#nullable disable

using ForkHierarchy.Client.Api;
using ForkHierarchy.Client.Components;
using ForkHierarchy.Core.Models;
using ForkHierarchy.Core.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
using Octokit;
using Refit;
using System.Text.RegularExpressions;

namespace ForkHierarchy.Client.Pages
{
    public partial class Index : IDisposable
    {
        private static readonly DialogOptions MaxWidthOptions = new DialogOptions()
        {
            //MaxWidth = MaxWidth.Medium,
            //FullWidth = true,
            CloseButton = true,
            CloseOnEscapeKey = true
        };

        public string FullNameSearch { get; set; }

        public Regex RepoFullNameRegEx { get; } = new Regex("^\\w+\\/\\w+$");

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ForkHierarchyApiClient ApiClient { get; set; }

        [Inject]
        public IOptions<GitHubOptions> GitHubOptions { get; set; }

        [Inject]
        public ISnackbar Snackbar { get; set; }

        [Inject]
        public IDialogService Dialog { get; set; }

        private RepositoryNodeModel _foundRepository;

        public async Task Search()
        {
            if (String.IsNullOrWhiteSpace(FullNameSearch))
                return;

            if (!RepoFullNameRegEx.IsMatch(FullNameSearch))
            {
                Snackbar.Add("Invalid Input", Severity.Error);
                return;
            }

            _foundRepository = null;

            var ownerRepoSplit = FullNameSearch.Split('/').TakeLast(2).ToArray();

            // Search existing repos
            try
            {
                var repo = await ApiClient.GitHubRepository.GetGitHubRepositoryByFullNameAsync(ownerRepoSplit[0], ownerRepoSplit[1]);
                if (repo is not null)
                    _foundRepository = new RepositoryNodeModel(repo, RepositoryNode.Size);
            }
            catch (ValidationApiException validationException) when (validationException.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
            }

            // if repo isnt known, try queueing it
            if (_foundRepository is null)
            {
                try
                {
                    await ApiClient.QueuedRepositories.CreateQueuedRepositoryAsync(ownerRepoSplit[0], ownerRepoSplit[1]);

                    Snackbar.Add($"Repository Queued", Severity.Success);
                }
                catch(ValidationApiException validationException) when (validationException.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    // Already queued
                    Snackbar.Add($"Repository already queued", Severity.Warning);
                }
            }
            // if repo was found, display it
            else
            {
                await OpenDialogAsync();
            }
        }

        private async Task OpenDialogAsync()
        {
            var parameters = new DialogParameters();
            parameters.Add("Node", _foundRepository);

            await Dialog.Show<FoundRepositoryComponent>("Found Repository Dialog", parameters, MaxWidthOptions).Result;
        }

        public void Dispose()
        {
            //DbContext?.Dispose();
        }
    }
}
