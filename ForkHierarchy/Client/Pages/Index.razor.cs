#nullable disable

using ForkHierarchy.Client.Api;
using ForkHierarchy.Client.Components;
using ForkHierarchy.Core.Models;
using ForkHierarchy.Core.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
using Octokit;
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

        public string SearchText { get; set; }
        public bool FromSource { get; set; }

        public Regex RepoFullNameRegEx { get; } = new Regex("^\\w+\\/\\w+$");

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public GitHubClient GitHubClient { get; set; }

        [Inject]
        public IOptions<GitHubOptions> GitHubOptions { get; set; }

        [Inject]
        public ISnackbar Snackbar { get; set; }

        [Inject]
        public IDialogService Dialog { get; set; }

        private RepositoryNodeModel _foundRepository;

        public async Task Search()
        {
            if (String.IsNullOrWhiteSpace(SearchText))
                return;
            
            if (!RepoFullNameRegEx.IsMatch(SearchText))
            {
                Snackbar.Add("Invalid Input", Severity.Error);
                return;
            }

            _foundRepository = null;

            var vals = SearchText.Split('/').TakeLast(2).ToArray();
            try
            {
                var repo = await GitHubClient.Repository.Get(vals[0], vals[1]);
                if (FromSource)
                    repo = repo.Source ?? repo;

                if (repo.ForksCount > GitHubOptions.Value.MaxForks)
                {
                    Snackbar.Add($"Target Repository Exceeds max allowed Forks of {GitHubOptions.Value.MaxForks}", Severity.Error);
                    return;
                }
                var client = new ForkHierarchyApiClient(NavigationManager.BaseUri, null);
                client.Git
                
                /*
                var dbo = await DbContext.GitHubRepositories.Include(x => x.Owner).FirstOrDefaultAsync(x => x.FullName == SearchText);
                var dto = dbo?.ToDto() ?? new GitHubRepository(repo);
                _foundRepository = new RepositoryNodeModel(dto, RepositoryNode.Size);
                */
                await OpenDialogAsync();
            }
            catch (NotFoundException)
            {
                Snackbar.Add($"Could not find the target repository", Severity.Error);
            }
        }

        private async Task OpenDialogAsync()
        {
            /*
            var databaseRepository = await DbContext.GitHubRepositories.Include(x => x.Owner).FirstOrDefaultAsync(x => x.FullName == _foundRepository.Item.FullName);
            var databaseQueue = await DbContext.QueuedRepositories.FirstOrDefaultAsync(x => x.Owner == _foundRepository.Item.Owner.Login && x.Name == _foundRepository.Item.Name);
            var repoState = State.Unknown;

            if (databaseRepository is not null)
                repoState = State.Known;
            else if (databaseQueue is not null)
                repoState = State.Queued;
            else
                repoState = State.Unknown;

            var parameters = new DialogParameters();
            parameters.Add("Node", _foundRepository);
            parameters.Add("DatabaseRepository", databaseRepository);
            parameters.Add("DatabaseQueue", databaseQueue);
            parameters.Add("RepoState", repoState);

            var dialog = await Dialog.Show<FoundRepositoryComponent>("Found Repository Dialog", parameters, MaxWidthOptions).Result;

            if (!dialog.Cancelled && repoState != State.Queued)
            {
                var queueItem = new Database.Models.QueuedRepository()
                {
                    Owner = _foundRepository.Item.Owner.Login,
                    Name = _foundRepository.Item.Name,
                    AddedAt = DateTime.UtcNow
                };

                DbContext.QueuedRepositories.Add(queueItem);
                DbContext.SaveChanges();
            }
            */
        }

        public void Dispose()
        {
            //DbContext?.Dispose();
        }
    }
}
