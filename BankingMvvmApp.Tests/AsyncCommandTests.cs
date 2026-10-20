using BankingApp.Commands;

namespace BankingMvvmApp.Tests;

public class AsyncCommandTests
{
    [Fact]
    public async Task Execute_WhenConditionIsFalse_DoesNotRun()
    {
        bool executed = false;
        var command = new AsyncCommand(() =>
        {
            executed = true;
            return Task.CompletedTask;
        }, () => false);

        await command.ExecuteAsync(null);

        Assert.False(command.CanExecute(null));
        Assert.False(executed);
    }

    [Fact]
    public async Task Execute_WhileRunning_PreventsDuplicateExecution()
    {
        int calls = 0;
        var pending = new TaskCompletionSource();
        var command = new AsyncCommand(async () =>
        {
            calls++;
            await pending.Task;
        });

        var firstExecution = command.ExecuteAsync(null);
        try
        {
            Assert.False(command.CanExecute(null));
            await command.ExecuteAsync(null);
            Assert.Equal(1, calls);
        }
        finally
        {
            pending.SetResult();
            await firstExecution;
        }

        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public async Task Execute_WhenActionThrows_ReenablesCommandAndPropagatesError()
    {
        var command = new AsyncCommand(() => Task.FromException(new InvalidOperationException("Test failure.")));

        await Assert.ThrowsAsync<InvalidOperationException>(() => command.ExecuteAsync(null));

        Assert.True(command.CanExecute(null));
    }
}
