using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlWatcherDesktop.Models;

public interface IPhraseStorage
{
    string[] GetAllPhrases();
    void AddPhrase(string phrase);
    void RemovePhrase(string phrase);

    string[] GetAllExcludePhrases();
    void AddExcludePhrase(string phrase);
    void RemoveExcludePhrase(string phrase);
}

public class FilePhraseStorage : IPhraseStorage
{
    public string FileNameInclude { get; }
    public string FileNameExclude { get; }

    public FilePhraseStorage(string fileNameInclude, string fileNameExclude)
    {
        FileNameInclude = fileNameInclude;
        FileNameExclude = fileNameExclude;
    }

    public string[] GetAllPhrases()
    {
        if (File.Exists(FileNameInclude))
        {
            return File.ReadAllLines(FileNameInclude);
        }
        return Enumerable.Empty<string>().ToArray();
    }

    public void AddPhrase(string phrase)
    {
        File.AppendAllLines(FileNameInclude, new string[] { phrase });
    }

    public void RemovePhrase(string phrase)
    {
        var phrases = GetAllPhrases();

        if (phrases.Contains(phrase))
        {
            var list = phrases.ToList();
            list.RemoveAll(i => i.Equals(phrase));

            File.WriteAllLines(FileNameInclude, list);
        }
    }

    public string[] GetAllExcludePhrases()
    {
        if (File.Exists(FileNameExclude))
        {
            return File.ReadAllLines(FileNameExclude);
        }
        return Enumerable.Empty<string>().ToArray();
    }

    public void AddExcludePhrase(string phrase)
    {
        File.AppendAllLines(FileNameExclude, new string[] { phrase });
    }

    public void RemoveExcludePhrase(string phrase)
    {
        var phrases = GetAllExcludePhrases();

        if (phrases.Contains(phrase))
        {
            var list = phrases.ToList();
            list.RemoveAll(i => i.Equals(phrase));

            File.WriteAllLines(FileNameExclude, list);
        }
    }
}
