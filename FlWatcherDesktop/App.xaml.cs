using FlWatcherDesktop.Models;
using FlWatcherDesktop.ViewModels;
using System.Windows;
using WPFNotification.Services;

namespace FlWatcherDesktop;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        INotificationDialogService toasts = new NotificationDialogService();
        IPhraseStorage phraseStorage = new FilePhraseStorage("phrases.txt");
        FLParser parser = new FLParser();

        var vm = new MainWindowViewModel(parser, toasts, phraseStorage);
        var mainWindow = new MainWindow();
        mainWindow.DataContext = vm;
        mainWindow.Show();
    }
}
