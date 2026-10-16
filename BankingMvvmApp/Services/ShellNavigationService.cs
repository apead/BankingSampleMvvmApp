namespace BankingApp.Services;

public class ShellNavigationService : INavigationService
{
    public Task GoToAsync(string route, IDictionary<string, object> parameters)
    {
        return Shell.Current.GoToAsync(route, new ShellNavigationQueryParameters(parameters));
    }

    public Task GoBackAsync() => Shell.Current.GoToAsync("..");
}
