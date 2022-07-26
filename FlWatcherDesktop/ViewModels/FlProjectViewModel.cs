using System;
using System.Diagnostics;
using System.Windows.Input;

namespace FlWatcherDesktop.ViewModels;

public class FlProjectViewModel : ViewModelBase
{
    public string Id { get; init; }
    public string Uri { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public DateTime Created { get; init; }
    public string PriceString { get; init; }

    public ICommand OpenInBrowserCommand { get; }

    public FlProjectViewModel()
    {
        OpenInBrowserCommand = new LambdaCommand(OpenInBrowser, e => !string.IsNullOrWhiteSpace(Uri));
    }

    private void OpenInBrowser(object o)
    {
        Process.Start(Uri);
    }

    public static FlProjectViewModel FromModel(FLProjectItem e)
    {
        return new FlProjectViewModel()
        {
            Created = e.Created,
            Description = e.Description,
            Id = e.Id,
            PriceString = e.PriceString,
            Title = e.Title,
            Uri = e.Uri
        };
    }
}
