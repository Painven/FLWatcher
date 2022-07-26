using FlWatcherDesktop.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WPFNotification.Model;
using WPFNotification.Services;

namespace FlWatcherDesktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly FLParser parser;
    private readonly INotificationDialogService toasts;
    private readonly IPhraseStorage storage;

    public ObservableCollection<string> KeyPhrases { get; }
    public ObservableCollection<FlProjectViewModel> Projects { get; }

    string newKeyPhrase;
    public string NewKeyPhrase
    {
        get => newKeyPhrase;
        set => Set(ref newKeyPhrase, value);
    }

    public ICommand LoadedCommand { get; }

    public ICommand EnterToAccountCommand { get; }
    public ICommand StartWatchingCommand { get; }
    public ICommand StopWatchingCommand { get; }

    public ICommand AddKeyPhraseCommand { get; }

    public MainWindowViewModel()
    {
        LoadedCommand = new LambdaCommand(Loaded);
        AddKeyPhraseCommand = new LambdaCommand(AddPhrase);

        EnterToAccountCommand = new LambdaCommand(e => { }, e => false);
        StartWatchingCommand = new LambdaCommand(e => parser.Start(), e => !parser.IsRunning);
        StopWatchingCommand = new LambdaCommand(e => parser.Stop(), e => parser.IsRunning);

        Projects = new ObservableCollection<FlProjectViewModel>();
        KeyPhrases = new ObservableCollection<string>()
        {
            "Test 1","Test 1","Test 1","Test 1","Test 1"
        };
    }

    private void AddPhrase(object obj)
    {
        KeyPhrases.Add(NewKeyPhrase);
        storage.AddPhrase(NewKeyPhrase);
    }

    private void Loaded(object obj)
    {
        KeyPhrases.Clear();

        foreach(var item in storage.GetAll())
        {
            KeyPhrases.Add(item);
        }
    }

    public MainWindowViewModel(FLParser parser, INotificationDialogService toasts, IPhraseStorage storage) : this()
    {        
        this.parser = parser;
        this.toasts = toasts;
        this.storage = storage;
        parser.OnProjectFound += Parser_OnProjectFound;
        CommandManager.InvalidateRequerySuggested();
    }

    private void Parser_OnProjectFound(object? sender, FLProjectItem e)
    {
        if(KeyPhrases.FirstOrDefault(kp => e.Title.Contains(kp)) != null)
        {
            Projects.Insert(0, FlProjectViewModel.FromModel(e));
            //toasts.ShowNotificationWindow(new Notification() { Title = "Новый проект FL", Message = e.Title });
        }
    }

}
