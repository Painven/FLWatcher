using FlWatcherDesktop.Models;
using FlWatcherDesktop.ViewModels;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WPFNotification.Services;

namespace FlWatcherDesktop;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static Mutex mutex;

    protected override void OnStartup(StartupEventArgs e)
    {
        mutex = new Mutex(false, "FLWatcher");
        if (!mutex.WaitOne(500))
        {
            Application.Current.Shutdown();
        }

        INotificationDialogService toasts = new NotificationDialogService();
        IPhraseStorage phraseStorage = new FilePhraseStorage("phrases.txt", "exclude_phrase.txt");
        FLParser parser = new FLParser();

        var vm = new MainWindowViewModel(parser, toasts, phraseStorage);
        var mainWindow = new MainWindow();
        mainWindow.Closing += (o, e) =>
        {
            e.Cancel = true;
            mainWindow.Hide();
        };
        mainWindow.DataContext = vm;
        mainWindow.Show();

        AddNotifyIcon();
    }

    public static void AddNotifyIcon()
    {
        var tbi = new TaskbarIcon();

        var tbiMenu = new ContextMenu();
        var showMenuItem = new MenuItem() { Header = "Отобразить" };
        showMenuItem.Click += (o, e) => Application.Current.MainWindow.Show();
        var exitMenuItem = new MenuItem() { Header = "Выход" };
        exitMenuItem.Click += (o, e) =>
        {
            mutex.ReleaseMutex();
            Application.Current.Shutdown();
        };

        tbiMenu.Items.Add(showMenuItem);
        tbiMenu.Items.Add(new Separator()); // null = separator
        tbiMenu.Items.Add(exitMenuItem);

        tbi.Icon = new Icon("favicon.ico");
        tbi.ToolTipText = "FL парсер";
        tbi.ContextMenu = tbiMenu;
    }
}
