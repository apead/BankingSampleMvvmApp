using BankingApp.Services;

namespace BankingMvvmApp.Tests.Fakes;

internal class FakeDialogService : IDialogService
{
    public bool ConfirmResult { get; set; } = true;
    public int ConfirmationCount { get; private set; }
    public string? LastAlertTitle { get; private set; }
    public string? LastAlertMessage { get; private set; }

    public Task ShowAlertAsync(string title, string message, string cancel)
    {
        LastAlertTitle = title;
        LastAlertMessage = message;
        return Task.CompletedTask;
    }

    public Task<bool> ConfirmAsync(string title, string message, string accept, string cancel)
    {
        ConfirmationCount++;
        return Task.FromResult(ConfirmResult);
    }
}
