using System.Windows.Input;

namespace BankingApp.Commands;

public class AsyncCommand : ICommand
{
    private readonly Func<Task> execute;
    private readonly Func<bool>? canExecute;
    private readonly bool allowConcurrentExecutions;
    private int runningCount;

    public event EventHandler? CanExecuteChanged;

    public AsyncCommand(Func<Task> execute, Func<bool>? canExecute = null,
        bool allowConcurrentExecutions = false)
    {
        this.execute = execute;
        this.canExecute = canExecute;
        this.allowConcurrentExecutions = allowConcurrentExecutions;
    }

    public bool CanExecute(object? parameter)
    {
        return (allowConcurrentExecutions || runningCount == 0)
            && (canExecute?.Invoke() ?? true);
    }

    public async void Execute(object? parameter)
    {
        await ExecuteAsync(parameter);
    }

    public async Task ExecuteAsync(object? parameter)
    {
        if (!CanExecute(parameter))
        {
            return;
        }

        runningCount++;
        try
        {
            NotifyCanExecuteChanged();
            await execute();
        }
        finally
        {
            runningCount--;
            NotifyCanExecuteChanged();
        }
    }

    public void NotifyCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
