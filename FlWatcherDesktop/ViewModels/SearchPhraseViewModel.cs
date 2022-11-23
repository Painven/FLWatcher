using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FlWatcherDesktop.ViewModels;

public class SearchPhraseViewModel : ViewModelBase
{
    public event Action OnRemoveClicked;

    public string Phrase { get; init; }

    int tagsCount = 0;
    public int TagsCount
    {
        get => tagsCount;
        set => Set(ref tagsCount, value);
    }

    public ICommand RemovePhraseCommand { get; }

    public SearchPhraseViewModel(string phrase)
    {
        Phrase = phrase;
        RemovePhraseCommand = new LambdaCommand(e => OnRemoveClicked?.Invoke());
    }
}
