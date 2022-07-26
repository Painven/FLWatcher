using System;
using System.Windows.Input;

namespace FlWatcherDesktop;
public abstract class Command : ICommand
{
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public virtual bool CanExecute(object parameter) => true;

    public abstract void Execute(object parameter);
}

public class LambdaCommand : Command
{
    private readonly Action<object> action;
    private readonly Func<object, bool> canExecute;
    private ICommand? openInBrowser;
    private bool v;

    public LambdaCommand(Action<object> action, Func<object, bool> canExecute = null)
    {
        this.action = action ?? throw new ArgumentNullException(nameof(action));
        this.canExecute = canExecute;
    }

    public LambdaCommand(ICommand? openInBrowser, bool v)
    {
        this.openInBrowser = openInBrowser;
        this.v = v;
    }

    public override bool CanExecute(object parameter)
    {
        return canExecute?.Invoke(parameter) ?? true;
    }

    public override void Execute(object parameter)
    {
        action(parameter);
    }
}
