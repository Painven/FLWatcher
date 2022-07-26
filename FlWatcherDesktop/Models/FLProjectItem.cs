using System;

namespace FlWatcherDesktop.ViewModels;

public record FLProjectItem()
{
    public string Id { get; init; }
    public string Uri { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public DateTime Created { get; init; }
    public string PriceString { get; init; }
}


