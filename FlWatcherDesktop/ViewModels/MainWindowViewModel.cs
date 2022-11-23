using FlWatcherDesktop.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WPFNotification.Model;
using WPFNotification.Services;

namespace FlWatcherDesktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public const int MAX_LOG_LINES = 25;

    private readonly FLParser parser;
    private readonly INotificationDialogService toasts;
    private readonly IPhraseStorage storage;
    private readonly SoundPlayer soundPlayer;

    public ICollectionView OrderedKeyPhrases { get; }

    public ObservableCollection<SearchPhraseViewModel> KeyPhrases { get; }
    public ObservableCollection<FlProjectViewModel> Projects { get; }
    public ObservableCollection<string> Logs { get; }

    int refreshIntervalInMinutes = 5;
    public int RefreshIntervalInMinutes
    {
        get => refreshIntervalInMinutes;
        set
        {
            if(value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if(Set(ref refreshIntervalInMinutes, value))
            {
                parser.Interval = TimeSpan.FromMinutes(value);
            }
        }
    }

    public bool IsRunning
    {
        get => parser?.IsRunning ?? false; 
    }

    bool isRefreshing;
    public bool IsRefreshing
    {
        get => isRefreshing;
        set => Set(ref isRefreshing, value);
    }

    string newKeyPhrase;
    public string NewKeyPhrase
    {
        get => newKeyPhrase;
        set => Set(ref newKeyPhrase, value);
    }

    public ICommand LoadedCommand { get; }

    public ICommand RefreshNowCommand { get; }
    public ICommand StartWatchingCommand { get; }
    public ICommand StopWatchingCommand { get; }

    public ICommand AddKeyPhraseCommand { get; }

    public MainWindowViewModel()
    {
        LoadedCommand = new LambdaCommand(Loaded);
        AddKeyPhraseCommand = new LambdaCommand(AddPhrase);

        RefreshNowCommand = new LambdaCommand(RefreshNow, e => !IsRefreshing);
        StartWatchingCommand = new LambdaCommand(async e => await Start(), e => !IsRunning);
        StopWatchingCommand = new LambdaCommand(Stop, e => IsRunning);

        Logs = new(new string[] { "старт", "не найдно" });
        Projects = new ObservableCollection<FlProjectViewModel>();
        KeyPhrases = new ObservableCollection<SearchPhraseViewModel>()
        {
            new SearchPhraseViewModel("C#"),
            new SearchPhraseViewModel("парсинг"),
            new SearchPhraseViewModel("CMS"),
            new SearchPhraseViewModel("OpenCart")
        };
        OrderedKeyPhrases = CollectionViewSource.GetDefaultView(KeyPhrases);
        OrderedKeyPhrases.SortDescriptions.Add(new SortDescription(nameof(SearchPhraseViewModel.TagsCount), ListSortDirection.Descending));

    }


    public MainWindowViewModel(FLParser parser, INotificationDialogService toasts, IPhraseStorage storage) : this()
    {
        soundPlayer = new SoundPlayer(File.OpenRead(@"D:\Programming\CSharp\Парсеры\FLWatcher\FlWatcherDesktop\Assets\BlinkBirth1.wav"));

        this.parser = parser;
        this.toasts = toasts;
        this.storage = storage;
        parser.OnProjectFound += Parser_OnProjectFound;
        parser.RunStatusChanged += () => RaisePropertyChanged(nameof(IsRunning));
        CommandManager.InvalidateRequerySuggested();
    }

    private async void RefreshNow(object obj)
    {
        IsRefreshing = true;
        try
        {
            await parser.RefreshNow();
        }
        finally
        {
            IsRefreshing = false;
        }
        
    }

    private void Stop(object obj)
    {
        parser.Stop();
        AddLogLine("Приостановка обхода по таймеру");
    }

    private async Task Start()
    {
        await parser.Start();       
        AddLogLine($"Запуск обхода по таймеру (период - {refreshIntervalInMinutes} минут)");
    }

    private void AddPhrase(object obj)
    {
        CreatePhraseItem(NewKeyPhrase);
        storage.AddPhrase(NewKeyPhrase);
    }

    private void Loaded(object obj)
    {
        KeyPhrases.Clear();
        Logs.Clear();

        AddLogLine("Запуск приложения");

        storage.GetAllPhrases().ToList().ForEach(phrase => CreatePhraseItem(phrase));
    }

    private async void Parser_OnProjectFound(object? sender, FLProjectItem e)
    {
        var findedKeyPhrases = KeyPhrases.Where(kp => e.Title.Contains(kp.Phrase, StringComparison.OrdinalIgnoreCase)).ToArray();
        if (findedKeyPhrases.Any())
        {
            findedKeyPhrases.ToList().ForEach(i => i.TagsCount++);

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                AddLogLine($"Найден новый проект '{e.Title}'");
                var item = FlProjectViewModel.FromModel(e, findedKeyPhrases.Select(p => p.Phrase).ToArray());
                Projects.Insert(0, item);
                OrderedKeyPhrases.Refresh();
            });

            soundPlayer.PlaySync();
        }
    }

    private void CreatePhraseItem(string phrase)
    {
        var item = new SearchPhraseViewModel(phrase);
        item.OnRemoveClicked += () =>
        {
            KeyPhrases.Remove(item);
            storage.RemovePhrase(phrase);
        };
        KeyPhrases.Add(item);
    }

    private void AddLogLine(string? messge)
    {
        Logs.Insert(0, $"[{DateTime.Now.ToShortTimeString()}] {messge ?? string.Empty}");
        if(Logs.Count > MAX_LOG_LINES)
        {
            Logs.RemoveAt(MAX_LOG_LINES - 1);
        }
    }

}
