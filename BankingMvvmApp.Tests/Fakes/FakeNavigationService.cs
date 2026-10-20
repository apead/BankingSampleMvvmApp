using BankingApp.Services;

namespace BankingMvvmApp.Tests.Fakes;

internal class FakeNavigationService : INavigationService
{
    public string? LastRoute { get; private set; }
    public IDictionary<string, object> LastParameters { get; private set; } = new Dictionary<string, object>();
    public int GoBackCount { get; private set; }

    public Task GoToAsync(string route, IDictionary<string, object> parameters)
    {
        LastRoute = route;
        LastParameters = parameters;
        return Task.CompletedTask;
    }

    public Task GoBackAsync()
    {
        GoBackCount++;
        return Task.CompletedTask;
    }
}
